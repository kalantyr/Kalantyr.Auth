using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Auth.Services;
using Kalantyr.Auth.Services.Impl;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Kalantyr.Auth.Tests
{
    public class AuthService_Tests
    {
        private readonly Mock<IUserStorageReadonly> _userStorage = new();
        private readonly Mock<IHashCalculator> _hashCalculator = new();
        private readonly Mock<ITokenStorage> _tokenStorage = new();
        private readonly Mock<IOptions<AuthServiceConfig>> _config = new();

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
                .ReturnsAsync(default(uint?));

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object);
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
                .ReturnsAsync<IUserStorageReadonly, uint?>(123);
            _userStorage
                .Setup(us => us.GetPasswordRecordAsync(It.IsAny<uint>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new PasswordRecord { PasswordHash = "1234567890" });

            _hashCalculator
                .Setup(hc => hc.GetHash(It.IsAny<string>(), It.IsAny<string>()))
                .Returns(password == "qwerty" ? "1234567890" : "00000");

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object);
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
                .ReturnsAsync<IUserStorageReadonly, uint?>(123);

            var tokenInfo = new TokenInfo { ExpirationDate = DateTimeOffset.Now.AddHours(1) };
            _tokenStorage
                .Setup(ts => ts.GetByUserIdAsync(It.IsAny<uint>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tokenInfo);

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object);
            var data = new LoginPasswordDto();
            var result1 = await authService.LoginAsync(data, CancellationToken.None);
            var result2 = await authService.LoginAsync(data, CancellationToken.None);
            Assert.AreEqual(result1.Result.Value, result2.Result.Value);
            Assert.AreEqual(result1.Result.ExpirationDate, result2.Result.ExpirationDate);
        }

        [Test]
        public async Task Logout_WithoutToken_Test()
        {
            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object);
            var result = await authService.LogoutAsync(" ", CancellationToken.None);
            Assert.AreEqual(Errors.TokenNotFound.Code, result.Error.Code);
        }

        [Test]
        public async Task Logout_Test()
        {
            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object);
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

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object);

            var result = await authService.GetUserIdAsync("fgjkgjkb", "app-666", CancellationToken.None);
            Assert.AreEqual(Errors.WrongAppKey.Code, result.Error.Code);

            result = await authService.GetUserIdAsync("666", "app-123", CancellationToken.None);
            Assert.AreEqual(Errors.TokenNotFound.Code, result.Error.Code);

            result = await authService.GetUserIdAsync("777", "app-123", CancellationToken.None);
            Assert.AreEqual(777, result.Result);
        }
    }
}
