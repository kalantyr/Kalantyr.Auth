using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;

namespace Kalantyr.Auth.Services
{
    public interface IUserStorageReadonly
    {
        Task<uint?> GetUserIdByLoginAsync(string login, CancellationToken cancellationToken);

        Task<PasswordRecord> GetPasswordRecordAsync(uint userId, CancellationToken cancellationToken);
    }

    public interface IUserStorage: IUserStorageReadonly
    {
    }
}
