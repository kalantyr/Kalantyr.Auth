using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;

namespace Kalantyr.Auth.DbRepositories
{
    public class UserStorage: IUserStorage
    {
        public Task<uint?> GetUserIdByLoginAsync(string login, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<PasswordRecord> GetPasswordRecordAsync(uint userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<uint> CreateAsync(string login, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordAsync(uint userId, PasswordRecord passwordRecord, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
