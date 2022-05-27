using System.Threading;
using System.Threading.Tasks;

namespace Kalantyr.Auth.InternalModels
{
    public interface IUserStorageReadonly
    {
        Task<UserRecord> GetUserIdByLoginAsync(string login, CancellationToken cancellationToken);

        Task<PasswordRecord> GetPasswordRecordAsync(uint userId, CancellationToken cancellationToken);
    }

    public interface IUserStorage : IUserStorageReadonly
    {
        Task<uint> CreateAsync(string login, CancellationToken cancellationToken);

        Task SetPasswordAsync(uint userId, PasswordRecord passwordRecord, CancellationToken cancellationToken);
    }

    public interface IUserStorageAdmin
    {
        Task MigrateAsync(CancellationToken cancellationToken);
    }
}
