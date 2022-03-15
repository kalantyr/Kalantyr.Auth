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
                PasswordHash = HashCalculator.GetHashImpl("qwerty1", "11111"),
                Salt = "11111"
            },
            new UserRecord
            {
                Id = 2,
                Login = "user2",
                PasswordHash = HashCalculator.GetHashImpl("qwerty2", "22222"),
                Salt = "22222"
            },
        };

        public async Task<UserRecord> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
        {
            return Users
                .FirstOrDefault(u => u.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
