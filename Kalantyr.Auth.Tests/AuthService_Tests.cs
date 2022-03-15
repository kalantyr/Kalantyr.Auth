using System;
using System.Threading;
using System.Threading.Tasks;
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
        private readonly Mock<IUserStorageReadonly> _userStorage = new Mock<IUserStorageReadonly>();
        private readonly Mock<IHashCalculator> _hashCalculator = new Mock<IHashCalculator>();
        private readonly Mock<ITokenStorage> _tokenStorage = new Mock<ITokenStorage>();
        private readonly Mock<IOptions<AuthServiceConfig>> _config = new Mock<IOptions<AuthServiceConfig>>();

        public AuthService_Tests()
        {
            _config
                .Setup(c => c.Value)
                .Returns(new AuthServiceConfig());

            _hashCalculator
                .Setup(hc => hc.GetHash(It.IsAny<string>(), It.IsAny<string>()))
                .Returns("12345");
        }

        [Test]
        public async Task Login_NotFound_Test()
        {
            _userStorage
                .Setup(us => us.GetUserByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(UserRecord));

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
                .Setup(us => us.GetUserByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new UserRecord { Login = "test_user", PasswordHash = "1234567890" });

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
            var userRecord = new UserRecord { Id = 123, Login = "user123", PasswordHash = "12345" };
            _userStorage
                .Setup(us => us.GetUserByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userRecord);

            var tokenInfo = new TokenInfo
            {
                ExpirationDate = DateTimeOffset.Now.AddHours(1)
            };
            _tokenStorage
                .Setup(ts => ts.GetAsync(It.IsAny<uint>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(tokenInfo);

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _tokenStorage.Object, _config.Object);
            var data = new LoginPasswordDto();
            var result1 = await authService.LoginAsync(data, CancellationToken.None);
            var result2 = await authService.LoginAsync(data, CancellationToken.None);
            Assert.AreEqual(result1.Result.Value, result2.Result.Value);
            Assert.AreEqual(result1.Result.ExpirationDate, result2.Result.ExpirationDate);
        }
    }
}
