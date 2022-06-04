using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public interface IAdminAuthClient
    {
        Task<ResultDto<uint>> CreateUserWithPasswordAsync(string login, string password, string userToken, CancellationToken cancellationToken);

        Task<ResultDto<bool>> MigrateAsync(string userToken, CancellationToken cancellationToken);
    }
}
