using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Microsoft.Extensions.Options;

namespace Kalantyr.Auth.Services.Impl
{
    public class UserStorage: IUserStorage
    {
        private readonly AuthServiceConfig _config;

        public UserStorage(IOptions<AuthServiceConfig> config)
        {
            _config = config.Value;
        }

        public Task<UserRecord> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
        {
            var userRecord = _config.Users
                .FirstOrDefault(u => u.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase));
            return Task.FromResult(userRecord);
        }
    }
}
