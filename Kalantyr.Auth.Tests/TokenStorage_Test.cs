using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Auth.Services.Impl;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace Kalantyr.Auth.Tests
{
    public class TokenStorage_Test
    {
        [Test]
        public async Task Add_Remove_Get_Test()
        {
            var tokenStorage = new TokenStorage(new Mock<IOptions<AuthServiceConfig>>().Object);
            var tokenInfo = new TokenInfo();
            
            await tokenStorage.AddAsync(123, tokenInfo, CancellationToken.None);
            var result = await tokenStorage.GetAsync(123, CancellationToken.None);
            Assert.AreSame(tokenInfo, result);
            
            await tokenStorage.RemoveAsync(123, CancellationToken.None);
            result = await tokenStorage.GetAsync(123, CancellationToken.None);
            Assert.IsNull(result);
        }
    }
}
