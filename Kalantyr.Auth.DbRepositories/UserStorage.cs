using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.DbRepositories.Entities;
using Kalantyr.Auth.InternalModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kalantyr.Auth.DbRepositories
{
    public class UserStorage: IUserStorage, IUserStorageAdmin, IHealthCheck
    {
        private readonly string _connectionString;

        public UserStorage(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("AuthDB");
        }

        public async Task MigrateAsync(CancellationToken cancellationToken)
        {
            await using var ctx = new AuthDbContext(_connectionString);
            await ctx.Database.MigrateAsync(cancellationToken);
        }

        public async Task<UserRecord> GetUserIdByLoginAsync(string login, CancellationToken cancellationToken)
        {
            await using var ctx = new AuthDbContext(_connectionString);
            var records = await ctx.Users
                .AsNoTracking()
                .Where(u => EF.Functions.Like(u.Login, "%"+login+"%"))
                .ToArrayAsync(cancellationToken);

            foreach (var record in records)
                if (record.Login.Equals(login, StringComparison.InvariantCultureIgnoreCase))
                    return Map(record);

            return null;
        }

        private static UserRecord Map(User record)
        {
            return new UserRecord
            {
                Id = record.Id,
                IsDisabled = record.IsDisabled,
                Login = record.Login
            };
        }

        public async Task<PasswordRecord> GetPasswordRecordAsync(uint userId, CancellationToken cancellationToken)
        {
            await using var ctx = new AuthDbContext(_connectionString);
            var record = await ctx.Passwords.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);

            if (record == null)
                return null;

            return new PasswordRecord
            {
                UserId = userId,
                PasswordHash = record.PasswordHash,
                Salt = record.Salt
            };
        }

        public async Task<uint> CreateAsync(string login, CancellationToken cancellationToken)
        {
            var user = new User { Login = login };
            await using var ctx = new AuthDbContext(_connectionString);
            await ctx.Users.AddAsync(user, cancellationToken);
            await ctx.SaveChangesAsync(cancellationToken);
            return user.Id;
        }

        public async Task SetPasswordAsync(uint userId, PasswordRecord passwordRecord, CancellationToken cancellationToken)
        {
            await using var ctx = new AuthDbContext(_connectionString);
            var record = await ctx.Passwords.FirstOrDefaultAsync(p => p.UserId == userId, cancellationToken);
            if (record == null)
            {
                record = new Password { UserId = userId };
                await ctx.Passwords.AddAsync(record, cancellationToken);
            }
            
            record.PasswordHash = passwordRecord.PasswordHash;
            record.Salt = passwordRecord.Salt;
            
            cancellationToken.ThrowIfCancellationRequested();
            await ctx.SaveChangesAsync(cancellationToken);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            try
            {
                await using var ctx = new AuthDbContext(_connectionString);
                await ctx.Users.FirstOrDefaultAsync(cancellationToken);
                return HealthCheckResult.Healthy();
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy(nameof(UserStorage), e);
            }
        }
    }
}
