using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.Services.Impl
{
    public class UserStorage: IUserStorage
    {
        private static readonly IReadOnlyCollection<UserRecord> Users = new[]
        {
            new UserRecord
            {
                Id = 1,
                Login = "user1",
                PasswordHash = "???"
            },
            new UserRecord
            {
                Id = 2,
                Login = "user2",
                PasswordHash = "???"
            },
        };

        public async Task<UserRecord> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
        {
            return Users
                .FirstOrDefault(u => u.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
