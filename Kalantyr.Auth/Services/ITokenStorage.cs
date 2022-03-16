using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.Services
{
    public interface ITokenStorage
    {
        Task AddAsync(uint userId, TokenInfo tokenInfo, CancellationToken cancellationToken);
        
        Task<TokenInfo> GetByUserIdAsync(uint userId, CancellationToken cancellationToken);

        Task<TokenInfo> GetByTokenAsync(string token, CancellationToken cancellationToken);

        Task RemoveByUserIdAsync(uint userId, CancellationToken cancellationToken);

        Task RemoveByTokenAsync(string token, CancellationToken cancellationToken);
    }
}
