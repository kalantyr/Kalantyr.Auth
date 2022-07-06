using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public interface IAdminAuthClient
    {
        Task<ResultDto<bool>> MigrateAsync(string userToken, CancellationToken cancellationToken);
    }
}
