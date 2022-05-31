using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
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

        public async Task<UserRecord> GetUserIdByLoginAsync(string login, CancellationToken cancellationToken)
        {
            var userRecord = _config.Users
                .FirstOrDefault(u => u.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase));

            return userRecord == null
                ? await _userStorage.GetUserIdByLoginAsync(login, cancellationToken)
                : new UserRecord { Id = userRecord.Id, Login = userRecord.Login };
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

        public Task<UserRecord> GetUserRecordAsync(uint userId, CancellationToken cancellationToken)
        {
            var userRecord = _config.Users
                .FirstOrDefault(u => u.Id== userId);

            return userRecord != null
                ? Task.FromResult(new UserRecord { Id = userId, Login = userRecord.Login })
                : _userStorage.GetUserRecordAsync(userId, cancellationToken);
        }

        public async Task<uint> CreateAsync(string login, CancellationToken cancellationToken)
        {
            return await _userStorage.CreateAsync(login, cancellationToken);
        }

        public async Task SetPasswordAsync(PasswordRecord passwordRec, CancellationToken cancellationToken)
        {
            var passwordRecord = _config.Passwords.FirstOrDefault(p => p.UserId == passwordRec.UserId);

            if (passwordRecord == null)
                await _userStorage.SetPasswordAsync(passwordRec, cancellationToken);
            else
                throw new NotSupportedException();
        }
    }
}
