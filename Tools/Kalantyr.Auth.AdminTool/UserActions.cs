using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Client;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.AdminTool
{
    internal class UserActions
    {
        private readonly Environment _environment;
        private readonly HttpClientFactory _httpClientFactory;

        public UserActions(Environment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _httpClientFactory = new HttpClientFactory(_environment);
        }

        public async Task LoginByPasswordAsync(string login, string password, CancellationToken cancellationToken)
        {
            IAuthClient client = new AuthClient(_httpClientFactory);
            var dto = new LoginPasswordDto { Login = login, Password = password };
            var result = await client.LoginByPasswordAsync(dto, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            App.Tokens[_environment] = result.Result;
        }

        public async Task LogoutAsync(CancellationToken cancellationToken)
        {
            IAuthClient client = new AuthClient(_httpClientFactory);
            var result = await client.LogoutAsync(App.Tokens[_environment].Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            App.Tokens.Remove(_environment);
        }

        public async Task SetPasswordAsync(string oldPassword, string newPassword, CancellationToken cancellationToken)
        {
            IAuthClient client = new AuthClient(_httpClientFactory);
            var result = await client.SetPasswordAsync(App.Tokens[_environment].Value, oldPassword, newPassword, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            App.Tokens.Remove(_environment);
        }

        public async Task CreateAsync(string userName, string password, CancellationToken cancellationToken)
        {
            IAuthClient client = new AuthClient(_httpClientFactory);
            var tokenInfo = App.Tokens.ContainsKey(_environment) ? App.Tokens[_environment] : null;
            var result = await client.CreateUserWithPasswordAsync(userName, password, tokenInfo?.Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
        }
    }
}
