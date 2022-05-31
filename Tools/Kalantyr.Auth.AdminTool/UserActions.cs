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

        public UserActions(Environment environment)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task LoginByPasswordAsync(string login, string password, CancellationToken cancellationToken)
        {
            IAuthClient client = new AuthClient(new HttpClientFactory(_environment));
            var dto = new LoginPasswordDto { Login = login, Password = password };
            var result = await client.LoginByPasswordAsync(dto, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            App.Tokens[_environment] = result.Result;
        }

        public async Task LogoutAsync(CancellationToken cancellationToken)
        {
            IAuthClient client = new AuthClient(new HttpClientFactory(_environment));
            var result = await client.LogoutAsync(App.Tokens[_environment].Value, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            App.Tokens.Remove(_environment);
        }

        public async Task SetPasswordAsync(string oldPassword, string newPAssword, CancellationToken cancellationToken)
        {
            IAuthClient client = new AuthClient(new HttpClientFactory(_environment));
            var result = await client.SetPasswordAsync(App.Tokens[_environment].Value, oldPassword, newPAssword, cancellationToken);
            if (result.Error != null)
                throw new Exception(result.Error.Message);
            App.Tokens.Remove(_environment);
        }
    }
}
