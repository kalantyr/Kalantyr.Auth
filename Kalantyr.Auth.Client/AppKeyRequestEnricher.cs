using System.Net.Http.Headers;
using Kalantyr.Web;

namespace Kalantyr.Auth.Client
{
    public class AppKeyRequestEnricher: IRequestEnricher
    {
        public string AppKey { get; set; }

        public void Enrich(HttpRequestHeaders requestHeaders)
        {
            if (string.IsNullOrEmpty(AppKey))
                return;

            requestHeaders.Add("App-Key", AppKey);
        }
    }
}
