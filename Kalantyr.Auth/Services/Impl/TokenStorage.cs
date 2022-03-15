using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Microsoft.Extensions.Options;

namespace Kalantyr.Auth.Services.Impl
{
    public class TokenStorage : ITokenStorage
    {
        private static readonly IDictionary<uint, TokenInfo> Tokens = new ConcurrentDictionary<uint, TokenInfo>();
        private readonly AuthServiceConfig _config;

        public TokenStorage(IOptions<AuthServiceConfig> config)
        {
            _config = config.Value;
        }

        public async Task AddAsync(uint userId, TokenInfo tokenInfo, CancellationToken cancellationToken)
        {
            if (tokenInfo == null) throw new ArgumentNullException(nameof(tokenInfo));

            Tokens.Add(userId, tokenInfo);
        }

        public async Task<TokenInfo> GetAsync(uint userId, CancellationToken cancellationToken)
        {
            if (Tokens.TryGetValue(userId, out var tokenInfo))
                return tokenInfo;
            return null;
        }

        public async Task RemoveAsync(uint userId, CancellationToken cancellationToken)
        {
            Tokens.Remove(userId);
        }
    }
}
