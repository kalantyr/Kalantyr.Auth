using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public interface IAdminAuthClient
    {
        Task<ResultDto<uint>> CreateUserWithPasswordAsync(string userToken, string login, string password, CancellationToken cancellationToken);

        Task<ResultDto<bool>> MigrateAsync(string userToken, CancellationToken cancellationToken);
    }
}
