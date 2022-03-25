using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Web;
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

            var existingToken = await _tokenStorage.GetByUserIdAsync(userRecord.Id, cancellationToken);
            if (existingToken != null)
            {
                if (existingToken.ExpirationDate > DateTimeOffset.Now.Add(_config.TokenLifetime / 2))
                    return new ResultDto<TokenInfo> { Result = existingToken };

                await _tokenStorage.RemoveByUserIdAsync(userRecord.Id, cancellationToken);
            }

            var tokenInfo = new TokenInfo
            {
                Value = GenerateToken(),
                ExpirationDate = DateTimeOffset.Now.Add(_config.TokenLifetime)
            };
            await _tokenStorage.AddAsync(userRecord.Id, tokenInfo, cancellationToken);
            return new ResultDto<TokenInfo> { Result = tokenInfo };
        }

        public async Task<ResultDto<bool>> LogoutAsync(string token, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(token))
                return new ResultDto<bool> { Error = Errors.TokenNotFound };

            var tokenInfo = await _tokenStorage.GetByTokenAsync(token, cancellationToken);
            if (tokenInfo != null)
                await _tokenStorage.RemoveByTokenAsync(token, cancellationToken);

            return ResultDto<bool>.Ok;
        }

        public async Task<ResultDto<uint>> GetUserIdAsync(string userToken, string appKey, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(userToken)) throw new ArgumentNullException(nameof(userToken));
            if (string.IsNullOrEmpty(appKey)) throw new ArgumentNullException(nameof(appKey));

            var appConfig = _config.AppKeys
                .FirstOrDefault(ak => ak.Key.Equals(appKey, StringComparison.InvariantCultureIgnoreCase));
            if (appConfig == null)
                return new ResultDto<uint> { Error = Errors.WrongAppKey };

            var userId = await _tokenStorage.GetUserIdByTokenAsync(userToken, cancellationToken);
            if (userId == null)
                return new ResultDto<uint> { Error = Errors.TokenNotFound };

            return new ResultDto<uint> { Result = userId.Value };
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
