using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;

namespace Kalantyr.Auth.Services.Impl
{
    public class TokenStorage : ITokenStorage
    {
        private static readonly IDictionary<uint, TokenInfo> Tokens = new ConcurrentDictionary<uint, TokenInfo>();

        public Task AddAsync(uint userId, TokenInfo tokenInfo, CancellationToken cancellationToken)
        {
            if (tokenInfo == null) throw new ArgumentNullException(nameof(tokenInfo));

            Tokens.Add(userId, tokenInfo);

            return Task.CompletedTask;
        }

        public Task<TokenInfo> GetByUserIdAsync(uint userId, CancellationToken cancellationToken)
        {
            if (Tokens.TryGetValue(userId, out var tokenInfo))
                return Task.FromResult(tokenInfo);
            return Task.FromResult(default(TokenInfo));
        }

        public Task RemoveByUserIdAsync(uint userId, CancellationToken cancellationToken)
        {
            if (Tokens.ContainsKey(userId))
                Tokens.Remove(userId);

            return Task.CompletedTask;
        }

        public Task RemoveByTokenAsync(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            var userId = default(uint?);
            foreach (var pair in Tokens)
                if (pair.Value.Value.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                {
                    userId = pair.Key;
                    break;
                }

            if (userId != null)
                Tokens.Remove(userId.Value);

            return Task.CompletedTask;
        }

        public Task<uint?> GetUserIdByTokenAsync(string token, CancellationToken cancellationToken)
        {
            foreach (var pair in Tokens)
                if (pair.Value.Value.Equals(token, StringComparison.InvariantCultureIgnoreCase))
                    return Task.FromResult<uint?>(pair.Key);

            return Task.FromResult(default(uint?));
        }

        public Task<TokenInfo> GetByTokenAsync(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token)) throw new ArgumentNullException(nameof(token));

            var result = Tokens.Values
                .FirstOrDefault(tokenInfo => tokenInfo.Value.Equals(token, StringComparison.InvariantCultureIgnoreCase));
            return Task.FromResult(result);
        }
    }
}
