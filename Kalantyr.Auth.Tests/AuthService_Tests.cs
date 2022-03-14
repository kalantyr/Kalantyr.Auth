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
        private readonly Mock<IOptions<AuthServiceConfig>> _config = new Mock<IOptions<AuthServiceConfig>>();

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
                .Setup(us => us.GetUserByLoginAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(default(UserRecord));

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _config.Object);
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

            var authService = new AuthService(_userStorage.Object, _hashCalculator.Object, _config.Object);
            var data = new LoginPasswordDto();
            var result = await authService.LoginAsync(data, CancellationToken.None);
            Assert.AreEqual(errorCode, result?.Error?.Code);
        }
    }
}
