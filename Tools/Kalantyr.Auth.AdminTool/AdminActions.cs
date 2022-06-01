using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Client;

namespace Kalantyr.Auth.AdminTool
{
    internal class AdminActions
    {
        private readonly Environment _environment;
        private readonly HttpClientFactory _httpClientFactory;

        public AdminActions(Environment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _httpClientFactory = new HttpClientFactory(_environment);
        }

        public async Task MigrateAsync(CancellationToken cancellationToken)
        {
            IAdminAuthClient client = new AuthClient(_httpClientFactory);
            var result = await client.MigrateAsync(App.Tokens[_environment].Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
        }
    }
}
