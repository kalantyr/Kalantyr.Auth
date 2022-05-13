using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Web;

namespace Kalantyr.Auth.Services
{
    public interface IAuthService
    {
        Task<ResultDto<TokenInfo>> LoginAsync(LoginPasswordDto dto, CancellationToken cancellationToken);
        
        Task<ResultDto<bool>> LogoutAsync(string token, CancellationToken cancellationToken);

        Task<ResultDto<uint>> GetUserIdAsync(string userToken, string appKey, CancellationToken cancellationToken);
        
        Task<ResultDto<uint>> CreateUserWithPasswordAsync(string token, string login, string password, CancellationToken cancellationToken);
    }
}
