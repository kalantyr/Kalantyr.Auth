using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
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

        public Task<uint?> GetUserIdByLoginAsync(string login, CancellationToken cancellationToken)
        {
            var userRecord = _config.Users
                .FirstOrDefault(u => u.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase));

            return userRecord == null
                ? Task.FromResult<uint?>(null)
                : Task.FromResult<uint?>(userRecord.Id);
        }

        public Task<PasswordRecord> GetPasswordRecordAsync(uint userId, CancellationToken cancellationToken)
        {
            var passwordRecord = _config.Passwords
                .FirstOrDefault(u => u.UserId == userId);

            if (passwordRecord == null)
                return Task.FromResult<PasswordRecord>(null);

            return Task.FromResult(new PasswordRecord
            {
                UserId = userId,
                PasswordHash = passwordRecord.PasswordHash,
                Salt = passwordRecord.Salt
            });
        }
    }
}
