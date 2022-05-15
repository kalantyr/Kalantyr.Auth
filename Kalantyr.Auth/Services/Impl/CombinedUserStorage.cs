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
    public class CombinedUserStorage: IUserStorage
    {
        private readonly IUserStorage _userStorage;
        private readonly AuthServiceConfig _config;

        public CombinedUserStorage(IOptions<AuthServiceConfig> config, IUserStorage userStorage)
        {
            _userStorage = userStorage ?? throw new ArgumentNullException(nameof(userStorage));
            _config = config.Value;
        }

        public async Task<uint?> GetUserIdByLoginAsync(string login, CancellationToken cancellationToken)
        {
            var userRecord = _config.Users
                .FirstOrDefault(u => u.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase));

            return userRecord == null
                ? await _userStorage.GetUserIdByLoginAsync(login, cancellationToken)
                : userRecord.Id;
        }

        public async Task<PasswordRecord> GetPasswordRecordAsync(uint userId, CancellationToken cancellationToken)
        {
            var passwordRecord = _config.Passwords
                .FirstOrDefault(u => u.UserId == userId);

            if (passwordRecord == null)
                return await _userStorage.GetPasswordRecordAsync(userId, cancellationToken);

            return new PasswordRecord
            {
                UserId = userId,
                PasswordHash = passwordRecord.PasswordHash,
                Salt = passwordRecord.Salt
            };
        }

        public async Task<uint> CreateAsync(string login, CancellationToken cancellationToken)
        {
            return await _userStorage.CreateAsync(login, cancellationToken);
        }

        public async Task SetPasswordAsync(uint userId, PasswordRecord passwordRec, CancellationToken cancellationToken)
        {
            var passwordRecord = _config.Passwords.FirstOrDefault(p => p.UserId == userId);

            if (passwordRecord == null)
                await _userStorage.SetPasswordAsync(userId, passwordRec, cancellationToken);
            else
                throw new NotSupportedException();
        }
    }
}
