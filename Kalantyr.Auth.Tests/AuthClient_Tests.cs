using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Client;
using Kalantyr.Auth.Models;
using Moq;
using NUnit.Framework;

namespace Kalantyr.Auth.Tests
{
    [Explicit]
    public class AuthClient_Tests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory = new Mock<IHttpClientFactory>();

        [Test]
        public async Task AuthClient_Test()
        {
            _httpClientFactory
                .Setup(hcf => hcf.CreateClient(It.IsAny<string>()))
                .Returns(new HttpClient
                {
                    BaseAddress = new Uri("http://u1628270.plsk.regruhosting.ru/auth")
                });

            var authClient = new AuthClient(_httpClientFactory.Object);
            var result = await authClient.ByLoginPasswordAsync(new LoginPasswordDto { Login = "user1", Password = "qwerty1" }, CancellationToken.None);
            Assert.IsNotNull(result);
        }
    }
}
