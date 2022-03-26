using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public class AuthClient: HttpClientBase, IAuthClient
    {
        public TokenInfo TokenInfo { get; private set; }

        public AuthClient(IHttpClientFactory httpClientFactory) : base(httpClientFactory, new TokenRequestEnricher())
        {
        }

        public async Task<ResultDto<TokenInfo>> LoginByPasswordAsync(LoginPasswordDto loginPasswordDto, CancellationToken cancellationToken)
        {
            var result = await Post<ResultDto<TokenInfo>>("/login/byLoginPassword", JsonSerializer.Serialize(loginPasswordDto), cancellationToken);
            
            TokenInfo = result.Result;
            if (TokenInfo != null)
                ((TokenRequestEnricher) RequestEnricher).Token = TokenInfo.Value;

            return result;
        }

        public async Task<ResultDto<bool>> LogoutAsync(CancellationToken cancellationToken)
        {
            return await Post<ResultDto<bool>>("/logout", null, cancellationToken);
        }
    }
}
