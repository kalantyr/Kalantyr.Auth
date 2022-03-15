using System;
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

        private readonly IUserStorageReadonly _userStorage;
        private readonly IHashCalculator _hashCalculator;
        private readonly ITokenStorage _tokenStorage;
        private readonly AuthServiceConfig _config;

        public AuthService(IUserStorageReadonly userStorage, IHashCalculator hashCalculator, ITokenStorage tokenStorage, IOptions<AuthServiceConfig> config)
        {
            _userStorage = userStorage ?? throw new ArgumentNullException(nameof(userStorage));
            _hashCalculator = hashCalculator ?? throw new ArgumentNullException(nameof(hashCalculator));
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
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

            cancellationToken.ThrowIfCancellationRequested();

            var existingToken = await _tokenStorage.GetAsync(userRecord.Id, cancellationToken);
            if (existingToken != null)
            {
                if (existingToken.ExpirationDate > DateTimeOffset.Now.Add(_config.TokenLifetime / 2))
                    return new ResultDto<TokenInfo> { Result = existingToken };

                await _tokenStorage.RemoveAsync(userRecord.Id, cancellationToken);
            }

            var tokenInfo = new TokenInfo
            {
                Value = GenerateToken(),
                ExpirationDate = DateTimeOffset.Now.Add(_config.TokenLifetime)
            };
            await _tokenStorage.AddAsync(userRecord.Id, tokenInfo, cancellationToken);
            return new ResultDto<TokenInfo> { Result = tokenInfo };
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
