using System;
using System.Net.Http;

namespace Kalantyr.Auth.AdminTool
{
    internal class HttpClientFactory: IHttpClientFactory
    {
        private readonly Environment _environment;

        public HttpClientFactory(Environment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public HttpClient CreateClient(string name)
        {
            return new HttpClient
            {
                BaseAddress = new Uri(_environment.AuthApiUrl)
            };
        }
    }
}
