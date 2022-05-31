using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public interface IAppAuthClient
    {
        Task<ResultDto<uint>> GetUserIdAsync(string userToken, CancellationToken cancellationToken);
    }
}
