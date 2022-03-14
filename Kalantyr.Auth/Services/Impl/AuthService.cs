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
    public class AuthService: IAuthService
    {
        private static readonly ResultDto<TokenInfo> LoginNotFound = new ResultDto<TokenInfo> { Error = Errors.LoginNotFound };
        private static readonly ResultDto<TokenInfo> WrongPassword = new ResultDto<TokenInfo> { Error = Errors.WrongPassword };
        private static readonly IDictionary<uint, TokenInfo> Tokens = new ConcurrentDictionary<uint, TokenInfo>();

        private readonly IUserStorageReadonly _userStorage;
        private readonly IHashCalculator _hashCalculator;
        private readonly AuthServiceConfig _config;

        public AuthService(IUserStorageReadonly userStorage, IHashCalculator hashCalculator, IOptions<AuthServiceConfig> config)
        {
            _userStorage = userStorage ?? throw new ArgumentNullException(nameof(userStorage));
            _hashCalculator = hashCalculator ?? throw new ArgumentNullException(nameof(hashCalculator));
            _config = config.Value;
        }

        /// <inheritdoc/>
        public async Task<ResultDto<TokenInfo>> LoginAsync(LoginPasswordDto dto, CancellationToken cancellationToken)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var userRecord = await _userStorage.GetUserByLoginAsync(dto.Login, cancellationToken);

            if (userRecord == null)
                return LoginNotFound;

            var hash = _hashCalculator.GetHash(dto.Password, userRecord.Salt);
            if (hash != userRecord.PasswordHash)
                return WrongPassword;

            var tokenInfo = new TokenInfo
            {
                Value = GenerateToken(),
                ExpirationDate = DateTimeOffset.Now.Add(_config.TokenLifetime)
            };
            Tokens.Add(userRecord.Id, tokenInfo);
            return new ResultDto<TokenInfo> { Result = tokenInfo };
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
