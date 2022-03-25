using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public class AuthClient: HttpClientBase
    {
        public AuthClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory, null)
        {
        }

        public async Task<ResultDto<TokenInfo>> ByLoginPasswordAsync(LoginPasswordDto loginPasswordDto, CancellationToken cancellationToken)
        {
            return await Post<ResultDto<TokenInfo>>("/login/byLoginPassword", JsonSerializer.Serialize(loginPasswordDto), cancellationToken);
        }
    }
}
