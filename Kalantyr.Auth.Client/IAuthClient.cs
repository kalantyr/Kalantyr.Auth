using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public interface IAuthClient
    {
        Task<ResultDto<TokenInfo>> LoginByPasswordAsync(LoginPasswordDto loginPasswordDto, CancellationToken cancellationToken);
        
        Task<ResultDto<bool>> LogoutAsync(CancellationToken cancellationToken);
    }
}