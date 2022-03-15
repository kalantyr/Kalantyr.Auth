using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Services.Impl;
using NUnit.Framework;

namespace Kalantyr.Auth.Tests
{
    public class UserStorage_Tests
    {
        [Test]
        public async Task Test()
        {
            var storage = new UserStorage();
            var result = await storage.GetUserByLoginAsync("user1", CancellationToken.None);
            Assert.IsNotNull(result.Salt);
        }
    }
}
