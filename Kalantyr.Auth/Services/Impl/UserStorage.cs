using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
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

        public Task<uint> CreateAsync(string login, CancellationToken cancellationToken)
        {
            var userRecord = new UserRecord
            {
                Login = login,
                Id = _config.Users.Max(u => u.Id) + 1
            };
            var list = _config.Users.ToList();
            list.Add(userRecord);
            _config.Users = list.ToArray();
            return Task.FromResult(userRecord.Id);
        }

        public Task SetPasswordAsync(uint userId, PasswordRecord passwordRec, CancellationToken cancellationToken)
        {
            var passwordRecord = _config.Passwords.FirstOrDefault(p => p.UserId == userId);
            if (passwordRecord == null)
            {
                passwordRecord = new PasswordRecord { UserId = userId };
                var list = _config.Passwords.ToList();
                list.Add(passwordRecord);
                _config.Passwords = list.ToArray();
            }
            passwordRecord.PasswordHash = passwordRec.PasswordHash;
            passwordRecord.Salt = passwordRec.Salt;

            return Task.CompletedTask;
        }
    }
}
