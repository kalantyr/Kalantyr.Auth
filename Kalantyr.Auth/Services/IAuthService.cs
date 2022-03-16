using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.Services
{
    public interface IAuthService
    {
        Task<ResultDto<TokenInfo>> LoginAsync(LoginPasswordDto dto, CancellationToken cancellationToken);
        
        Task<ResultDto<bool>> LogoutAsync(string token, CancellationToken cancellationToken);

        Task<ResultDto<uint>> GetUserIdAsync(string userToken, string appKey, CancellationToken cancellationToken);
    }
}
