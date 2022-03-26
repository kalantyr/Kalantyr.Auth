using System.Net.Http.Headers;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public class TokenRequestEnricher: IRequestEnricher
    {
        public string Token { get; set; }

        public void Enrich(HttpRequestHeaders requestHeaders)
        {
            if (string.IsNullOrEmpty(Token))
                return;

            requestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }
    }
}
