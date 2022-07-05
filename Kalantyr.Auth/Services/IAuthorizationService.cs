using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;

namespace Kalantyr.Auth.Services
{
    public interface IAuthorizationService
    {
        Task<ResultDto<bool>> IsAdminAsync(string userToken, CancellationToken cancellationToken);
    }
}
