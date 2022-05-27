using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;

namespace Kalantyr.Auth.Services
{
    public interface IAdminService
    {
        Task<ResultDto<bool>> MigrateAsync(string token, CancellationToken cancellationToken);
    }
}
