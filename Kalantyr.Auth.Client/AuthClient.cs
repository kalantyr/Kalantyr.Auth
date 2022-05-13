using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public class AuthClient: HttpClientBase, IAuthClient
    {
        private readonly string _appKey;

        public TokenInfo TokenInfo { get; private set; }

        public AuthClient(IHttpClientFactory httpClientFactory, string appKey = null) : base(httpClientFactory, new RequestEnricher())
        {
            _appKey = appKey;
        }

        public async Task<ResultDto<TokenInfo>> LoginByPasswordAsync(LoginPasswordDto loginPasswordDto, CancellationToken cancellationToken)
        {
            var result = await Post<ResultDto<TokenInfo>>("/login/byPassword", JsonSerializer.Serialize(loginPasswordDto), cancellationToken);
            
            TokenInfo = result.Result;
            if (TokenInfo != null)
                ((RequestEnricher)base.RequestEnricher).TokenEnricher.Token = TokenInfo.Value;

            return result;
        }

        public async Task<ResultDto<bool>> LogoutAsync(CancellationToken cancellationToken)
        {
            return await Post<ResultDto<bool>>("/logout", null, cancellationToken);
        }

        public async Task<ResultDto<uint>> GetUserIdAsync(string userToken, CancellationToken cancellationToken)
        {
            var enricher = (RequestEnricher)base.RequestEnricher;
            enricher.TokenEnricher.Token = userToken;
            enricher.AppKeyEnricher.AppKey = _appKey;

            return await Get<ResultDto<uint>>("/user/id", cancellationToken);
        }

        protected class RequestEnricher: IRequestEnricher
        {
            public TokenRequestEnricher TokenEnricher { get; } = new TokenRequestEnricher();

            public AppKeyRequestEnricher AppKeyEnricher { get; } = new AppKeyRequestEnricher();

            public void Enrich(HttpRequestHeaders requestHeaders)
            {
                TokenEnricher.Enrich(requestHeaders);
                AppKeyEnricher.Enrich(requestHeaders);
            }
        }
    }
}
