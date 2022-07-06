using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public interface IAuthClient
    {
        Task<ResultDto<TokenInfo>> LoginByPasswordAsync(LoginPasswordDto loginPasswordDto, CancellationToken cancellationToken);

        Task<ResultDto<bool>> SetPasswordAsync(string userToken, string oldPassword, string newPassword, CancellationToken cancellationToken);

        Task<ResultDto<bool>> LogoutAsync(string userToken, CancellationToken cancellationToken);
        
        Task<ResultDto<uint>> CreateUserWithPasswordAsync(string login, string password, string userToken, CancellationToken cancellationToken);
    }
}