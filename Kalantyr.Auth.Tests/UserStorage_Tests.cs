using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Auth.Services.Impl;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Kalantyr.Auth.Tests
{
    public class UserStorage_Tests
    {
        private readonly Mock<IOptions<AuthServiceConfig>> _config = new Mock<IOptions<AuthServiceConfig>>();

        public UserStorage_Tests()
        {
            _config
                .Setup(c => c.Value)
                .Returns(new AuthServiceConfig
                {
                    Users = new[]
                    {
                        new UserRecord
                        {
                            Id = 1,
                            Login = "user1"
                        },
                        new UserRecord
                        {
                            Id = 2,
                            Login = "user2"
                        },
                    },
                    Passwords = new []
                    {
                        new PasswordRecord
                        {
                            UserId = 1,
                            PasswordHash = HashCalculator.GetHashImpl("qwerty1", "11111"),
                            Salt = "11111"
                        },
                        new PasswordRecord
                        {
                            UserId = 2,
                            PasswordHash = HashCalculator.GetHashImpl("qwerty2", "22222"),
                            Salt = "22222"
                        }
                    }
                });
        }

        [Test]
        public async Task GetUserByLoginAsync_Test()
        {
            var storage = new UserStorage(_config.Object);
            var userId = await storage.GetUserIdByLoginAsync("user1", CancellationToken.None);
            Assert.IsNotNull(userId);
        }
    }
}
