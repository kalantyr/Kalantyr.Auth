using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Services.Impl;
using Moq;
using NUnit.Framework;

namespace Kalantyr.Auth.Tests
{
    public class LoginValidator_Tests
    {
        private readonly Mock<IUserStorageReadonly> _userStorage = new();

        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("   ")]
        [TestCase("-")]
        [TestCase("...")]
        [TestCase("123-456")]
        [TestCase("a-a-a")]
        public async Task ValidateFormat_Test(string login)
        {
            var validator = new LoginValidator(_userStorage.Object);
            var result = await validator.ValidateAsync(login, CancellationToken.None);
            Assert.AreEqual(Errors.WrongLoginFormat.Code, result.Error.Code);
        }

        [Test]
        public async Task Validate_Exists_Test()
        {
            _userStorage
                .Setup(us => us.GetUserIdByLoginAsync("user1", It.IsAny<CancellationToken>()))
                .ReturnsAsync(new InternalModels.UserRecord { Id = 123u });
            var validator = new LoginValidator(_userStorage.Object);
            var result = await validator.ValidateAsync(" user1 ", CancellationToken.None);
            Assert.AreEqual(Errors.LoginExists.Code, result.Error.Code);
        }
    }
}
