using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Web;
using Kalantyr.Web.Impl;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kalantyr.Auth.Client
{
    public class AuthClient: HttpClientBase, IAuthClient, IAppAuthClient, IAdminAuthClient, IHealthCheck
    {
        private readonly string _appKey;

        private static readonly IDictionary<string, ResultDto<uint>> UserIds = new ConcurrentDictionary<string, ResultDto<uint>>();

        public TokenInfo TokenInfo { get; private set; }

        public AuthClient(IHttpClientFactory httpClientFactory, string appKey = null) : base(httpClientFactory, new RequestEnricher())
        {
            _appKey = appKey;
        }

        public async Task<ResultDto<uint>> CreateUserWithPasswordAsync(string login, string password, string userToken, CancellationToken cancellationToken)
        {
            var enricher = (RequestEnricher)RequestEnricher;
            enricher.TokenEnricher.Token = userToken;
            return await Post<ResultDto<uint>>("/user/createWithPassword?login=" + login, Serialize(password), cancellationToken);
        }

        public async Task<ResultDto<bool>> MigrateAsync(string userToken, CancellationToken cancellationToken)
        {
            var enricher = (RequestEnricher)base.RequestEnricher;
            enricher.TokenEnricher.Token = userToken;
            return await Post<ResultDto<bool>>("/admin/migrate", null, cancellationToken);
        }

        public async Task<ResultDto<TokenInfo>> LoginByPasswordAsync(LoginPasswordDto loginPasswordDto, CancellationToken cancellationToken)
        {
            var result = await Post<ResultDto<TokenInfo>>("/login/byPassword", JsonSerializer.Serialize(loginPasswordDto), cancellationToken);
            
            TokenInfo = result.Result;
            if (TokenInfo != null)
                ((RequestEnricher)base.RequestEnricher).TokenEnricher.Token = TokenInfo.Value;

            return result;
        }

        public async Task<ResultDto<bool>> SetPasswordAsync(string userToken, string oldPassword, string newPassword, CancellationToken cancellationToken)
        {
            var enricher = (RequestEnricher)base.RequestEnricher;
            enricher.TokenEnricher.Token = userToken;
            var body = new[] { oldPassword, newPassword };
            return await Post<ResultDto<bool>>("/user/setPassword", Serialize(body), cancellationToken);
        }

        public async Task<ResultDto<bool>> LogoutAsync(string userToken, CancellationToken cancellationToken)
        {
            var enricher = (RequestEnricher)base.RequestEnricher;
            enricher.TokenEnricher.Token = userToken;
            return await Post<ResultDto<bool>>("/logout", null, cancellationToken);
        }

        public async Task<ResultDto<uint>> GetUserIdAsync(string userToken, CancellationToken cancellationToken)
        {
            if (UserIds.TryGetValue(userToken, out var res))
                return res;

            var enricher = (RequestEnricher)RequestEnricher;
            enricher.TokenEnricher.Token = userToken;
            enricher.AppKeyEnricher.AppKey = _appKey;
            var result = await Get<ResultDto<uint>>("/user/id", cancellationToken);

            if (result.Error == null)
                UserIds.Add(userToken, result);

            return result;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new())
        {
            await Get<Version>("/tech/version", cancellationToken);
            return HealthCheckResult.Healthy();
        }
    }
}
