using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.Services
{
    public interface ITokenStorage
    {
        Task AddAsync(uint userId, TokenInfo tokenInfo, CancellationToken cancellationToken);
        
        Task<TokenInfo> GetAsync(uint userId, CancellationToken cancellationToken);
        
        Task RemoveAsync(uint userId, CancellationToken cancellationToken);
    }
}
