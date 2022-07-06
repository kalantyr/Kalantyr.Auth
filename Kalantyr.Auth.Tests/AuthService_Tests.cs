using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Auth.Services;
using Kalantyr.Auth.Services.Impl;
using Kalantyr.Web;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using UserRecord = Kalantyr.Auth.Models.UserRecord;

namespace Kalantyr.Auth.Tests
{
    public class AuthService_Tests
    {
        private readonly Mock<IUserStorage> _userStorage = new();
        private readonly Mock<IHashCalculator> _hashCalculator = new();
        private readonly Mock<ITokenStorage> _tokenStorage = new();
        private readonly Mock<IOptions<AuthServiceConfig>> _config = new();
        private readonly Mock<ILoginValidator> _loginValidator = new();
        private readonly Mock<IPasswordValidator> _passwordValidator = new();
        private readonly Mock<IAuthorizationService> _authorizationService = new();

        public AuthService_Tests()
        {
            _config
                .Setup(c => c.Value)
                .Returns(new AuthServiceConfig());
        }

        [Test]
        public async Task Login_NotFound_Test()
        {
            _userStorage
                .Setup(us => us.GetUserIdByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(InternalModels.UserRecord));

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);
            var data = new LoginPasswordDto();
            var result = await authService.LoginAsync(data, CancellationToken.None);
            Assert.AreEqual(Errors.LoginNotFound.Code, result.Error.Code);
        }

        [TestCase("wrong_pass", nameof(Errors.WrongPassword))]
        [TestCase("qwerty", null)]
        public async Task Login_Password_Test(string password, string errorCode)
        {
            _userStorage
                .Setup(us => us.GetUserIdByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InternalModels.UserRecord { Id = 123u });
            _userStorage
                .Setup(us => us.GetPasswordRecordAsync(It.IsAny<uint>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PasswordRecord { PasswordHash = "1234567890" });

            _hashCalculator
                .Setup(hc => hc.GetHash(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(password == "qwerty" ? "1234567890" : "00000");

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);
            var data = new LoginPasswordDto();
            var result = await authService.LoginAsync(data, CancellationToken.None);
            Assert.AreEqual(errorCode, result?.Error?.Code);
        }

        [Test]
        public async Task Login_Test()
        {
            _hashCalculator
                .Setup(hc => hc.GetHash(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("12345");

            _userStorage
                .Setup(us => us.GetUserIdByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InternalModels.UserRecord { Id = 123u });
            _userStorage
                .Setup(us => us.GetPasswordRecordAsync(It.IsAny<uint>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PasswordRecord { PasswordHash = "12345" });

            var tokenInfo = new TokenInfo { ExpirationDate = DateTimeOffset.Now.AddHours(1) };
            _tokenStorage
                .Setup(ts => ts.GetByUserIdAsync(It.IsAny<uint>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tokenInfo);

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);
            var data = new LoginPasswordDto();
            var result1 = await authService.LoginAsync(data, CancellationToken.None);
            var result2 = await authService.LoginAsync(data, CancellationToken.None);
            Assert.AreEqual(result1.Result.Value, result2.Result.Value);
            Assert.AreEqual(result1.Result.ExpirationDate, result2.Result.ExpirationDate);
        }

        [Test]
        public async Task Logout_WithoutToken_Test()
        {
            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);
            var result = await authService.LogoutAsync(" ", CancellationToken.None);
            Assert.AreEqual(Errors.TokenNotFound.Code, result.Error.Code);
        }

        [Test]
        public async Task Logout_Test()
        {
            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);
            var result = await authService.LogoutAsync("fgjkgjkb", CancellationToken.None);
            Assert.IsTrue(result.Result);
            Assert.IsNull(result.Error);
        }

        [Test]
        public async Task GetUserId_Test()
        {
            _config
                .Setup(c => c.Value)
                .Returns(new AuthServiceConfig
                {
                    AppKeys = new [] { new AuthServiceConfig.AppKeyConfig { Key = "app-123" } }
                });

            _tokenStorage
                .Setup(ts => ts.GetUserIdByTokenAsync("777", It.IsAny<CancellationToken>()))
                .ReturnsAsync(777u);

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);

            var result = await authService.GetUserIdAsync("fgjkgjkb", "app-666", CancellationToken.None);
            Assert.AreEqual(Errors.WrongAppKey.Code, result.Error.Code);

            result = await authService.GetUserIdAsync("666", "app-123", CancellationToken.None);
            Assert.AreEqual(Errors.TokenNotFound.Code, result.Error.Code);

            result = await authService.GetUserIdAsync("777", "app-123", CancellationToken.None);
            Assert.AreEqual(777, result.Result);
        }

        [Test]
        public async Task CreateUserWithPassword_Test()
        {
            _config
                .Setup(c => c.Value)
                .Returns(new AuthServiceConfig
                {
                    Users = new []
                    {
                        new UserRecord { Id = 1 }
                    }
                });

            _loginValidator
                .Setup(lv => lv.ValidateAsync("wrongLogin", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResultDto<bool> { Error = new Error { Code = "Error1" } });
            _loginValidator
                .Setup(lv => lv.ValidateAsync("trueLogin", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResultDto<bool> { Result = true });

            _passwordValidator
                .Setup(lv => lv.Validate("111"))
                .Returns(new ResultDto<bool> { Error = new Error { Code = "Error2" } });
            _passwordValidator
                .Setup(lv => lv.Validate("qwerty"))
                .Returns(ResultDto<bool>.Ok);

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);

            _authorizationService
                .Setup(a => a.IsAdminAsync("12345", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ResultDto<bool> { Result = false });
            _authorizationService
                .Setup(a => a.IsAdminAsync("11111", It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultDto<bool>.Ok);

            var result = await authService.CreateUserWithPasswordAsync("12345", "newUser", "qwerty", CancellationToken.None);
            Assert.AreEqual(Errors.AdminOnlyAccess.Code, result.Error.Code);

            result = await authService.CreateUserWithPasswordAsync("11111", "wrongLogin", "qwerty", CancellationToken.None);
            Assert.AreEqual("Error1", result.Error.Code);

            result = await authService.CreateUserWithPasswordAsync("11111", "trueLogin", "111", CancellationToken.None);
            Assert.AreEqual("Error2", result.Error.Code);
        }

        [Test]
        public async Task CreateUserWithPassword_2_Test()
        {
            _config
                .Setup(c => c.Value)
                .Returns(new AuthServiceConfig
                {
                    SelfCreationOfAccounts = true
                });
            _loginValidator
                .Setup(lv => lv.ValidateAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(ResultDto<bool>.Ok);
            _passwordValidator
                .Setup(lv => lv.Validate(It.IsAny<string>()))
                .Returns(ResultDto<bool>.Ok);

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);

            var result = await authService.CreateUserWithPasswordAsync("12345", "newUser", "qwerty", CancellationToken.None);
            Assert.IsNull(result.Error);
        }

        [Test]
        public async Task UserDisabled_Test()
        {
            _userStorage
                .Setup(us => us.GetUserIdByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InternalModels.UserRecord
                {
                    Id = 123u,
                    IsDisabled = true
                });

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);
            var result = await authService.LoginAsync(new LoginPasswordDto(), CancellationToken.None);
            Assert.AreEqual(Errors.UserIsInactive.Code, result.Error.Code);
        }

        [Test]
        public async Task SetPassword_Test()
        {
            _tokenStorage
                .Setup(ts => ts.GetUserIdByTokenAsync("1234", It.IsAny<CancellationToken>()))
                .ReturnsAsync(1234u);
            _tokenStorage
                .Setup(ts => ts.GetUserIdByTokenAsync("12345", It.IsAny<CancellationToken>()))
                .ReturnsAsync(12345u);

            _passwordValidator
                .Setup(lv => lv.Validate(""))
                .Returns(new ResultDto<bool> { Error = Errors.WrongPasswordFormat });
            _passwordValidator
                .Setup(lv => lv.Validate("qwerty"))
                .Returns(ResultDto<bool>.Ok);

            _userStorage
                .Setup(us => us.GetUserRecordAsync(1234, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InternalModels.UserRecord());
            _userStorage
                .Setup(us => us.GetUserRecordAsync(12345, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InternalModels.UserRecord { IsDisabled = true });
            _userStorage
                .Setup(us => us.GetPasswordRecordAsync(1234, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PasswordRecord { PasswordHash = "111" });

            _hashCalculator
                .Setup(hc => hc.GetHash("oldPwd", It.IsAny<string>()))
                .Returns("111");
            _hashCalculator
                .Setup(hc => hc.GetHash("wrongOldPwd", It.IsAny<string>()))
                .Returns("000");

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object, _loginValidator.Object, _passwordValidator.Object, _authorizationService.Object);

            var result = await authService.SetPasswordAsync("54321", "oldPwd", "", CancellationToken.None);
            Assert.AreEqual(Errors.TokenNotFound.Code, result.Error.Code);

            result = await authService.SetPasswordAsync("12345", "oldPwd", "", CancellationToken.None);
            Assert.AreEqual(Errors.WrongPasswordFormat.Code, result.Error.Code);

            result = await authService.SetPasswordAsync("12345", "oldPwd", "qwerty", CancellationToken.None);
            Assert.AreEqual(Errors.UserIsInactive.Code, result.Error.Code);

            result = await authService.SetPasswordAsync("1234", "wrongOldPwd", "qwerty", CancellationToken.None);
            Assert.AreEqual(Errors.WrongPassword.Code, result.Error.Code);

            result = await authService.SetPasswordAsync("1234", "oldPwd", "qwerty", CancellationToken.None);
            Assert.IsTrue(result.Result);
        }
    }
}
