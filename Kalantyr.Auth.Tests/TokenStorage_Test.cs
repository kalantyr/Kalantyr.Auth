using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Services.Impl;
using NUnit.Framework;

namespace Kalantyr.Auth.Tests
{
    public class TokenStorage_Test
    {
        [Test]
        public async Task Add_Remove_Get_Test()
        {
            const int userId = 123;

            var tokenStorage = new TokenStorage();
            var tokenInfo = new TokenInfo
            {
                Value = "1234567890"
            };

            await tokenStorage.AddAsync(userId, tokenInfo, CancellationToken.None);
            var result = await tokenStorage.GetByUserIdAsync(userId, CancellationToken.None);
            Assert.AreSame(tokenInfo, result);

            await tokenStorage.RemoveByUserIdAsync(userId, CancellationToken.None);
            result = await tokenStorage.GetByUserIdAsync(userId, CancellationToken.None);
            Assert.IsNull(result);

            await tokenStorage.AddAsync(userId, tokenInfo, CancellationToken.None);
            result = await tokenStorage.GetByUserIdAsync(userId, CancellationToken.None);
            Assert.AreSame(tokenInfo, result);

            await tokenStorage.RemoveByTokenAsync(tokenInfo.Value, CancellationToken.None);
            result = await tokenStorage.GetByUserIdAsync(userId, CancellationToken.None);
            Assert.IsNull(result);
        }
    }
}
