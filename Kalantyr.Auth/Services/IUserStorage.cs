using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.Services
{
    public interface IUserStorageReadonly
    {
        Task<UserRecord> GetUserByLoginAsync(string login, CancellationToken cancellationToken);
    }

    public interface IUserStorage: IUserStorageReadonly
    {
    }
}
