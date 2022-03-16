using Microsoft.AspNetCore.Http;

namespace Kalantyr.Auth.Utils
{
    public static class HttpRequestExtensions
    {
        public static string GetAuthToken(this HttpRequest request)
        {
            if (request.Headers.TryGetValue("Authorization", out var token))
                return token.ToString().Replace("Bearer ", string.Empty);
            return null;
        }

        public static string GetAppKey(this HttpRequest request)
        {
            if (request.Headers.TryGetValue("App-Key", out var token))
                return token;
            return null;
        }
    }
}
