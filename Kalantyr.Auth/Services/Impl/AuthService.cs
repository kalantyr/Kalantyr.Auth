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
        private static readonly ResultDto<TokenInfo> LoginNotFound = new() { Error = Errors.LoginNotFound };
        private static readonly ResultDto<TokenInfo> WrongPassword = new() { Error = Errors.WrongPassword };

        private readonly IUserStorageReadonly _userStorage;
        private readonly IHashCalculator _hashCalculator;
        private readonly ITokenStorage _tokenStorage;
        private readonly ILoginValidator _loginValidator;
        private readonly AuthServiceConfig _config;

        public AuthService(IUserStorageReadonly userStorage, IHashCalculator hashCalculator, ITokenStorage tokenStorage, IOptions<AuthServiceConfig> config, ILoginValidator loginValidator)
        {
            _userStorage = userStorage ?? throw new ArgumentNullException(nameof(userStorage));
            _hashCalculator = hashCalculator ?? throw new ArgumentNullException(nameof(hashCalculator));
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
            _loginValidator = loginValidator ?? throw new ArgumentNullException(nameof(loginValidator));
            _config = config.Value;
        }

        /// <inheritdoc/>
        public async Task<ResultDto<TokenInfo>> LoginAsync(LoginPasswordDto dto, CancellationToken cancellationToken)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var userRecord = await _userStorage.GetUserIdByLoginAsync(dto.Login, cancellationToken);

            if (userRecord == null)
                return LoginNotFound;

            var userId = userRecord.Value;

            cancellationToken.ThrowIfCancellationRequested();

            var passwordRecord = await _userStorage.GetPasswordRecordAsync(userId, cancellationToken);

            var hash = _hashCalculator.GetHash(dto.Password, passwordRecord.Salt);
            if (hash != passwordRecord.PasswordHash)
                return WrongPassword;

            cancellationToken.ThrowIfCancellationRequested();

            var existingToken = await _tokenStorage.GetByUserIdAsync(userId, cancellationToken);
            if (existingToken != null)
            {
                if (existingToken.ExpirationDate > DateTimeOffset.Now.Add(_config.TokenLifetime / 2))
                    return new ResultDto<TokenInfo> { Result = existingToken };

                await _tokenStorage.RemoveByUserIdAsync(userId, cancellationToken);
            }

            var tokenInfo = new TokenInfo
            {
                Value = GenerateToken(),
                ExpirationDate = DateTimeOffset.Now.Add(_config.TokenLifetime)
            };
            await _tokenStorage.AddAsync(userId, tokenInfo, cancellationToken);
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

        public async Task<ResultDto<uint>> CreateUserWithPasswordAsync(string token, string login, string password, CancellationToken cancellationToken)
        {
            var adminId = await _tokenStorage.GetUserIdByTokenAsync(token, cancellationToken);
            if (adminId == null)
                return new ResultDto<uint> { Error = Errors.TokenNotFound };

            cancellationToken.ThrowIfCancellationRequested();

            if (_config.Users.All(u => u.Id != adminId.Value))
                return new ResultDto<uint> { Error = Errors.AdminOnlyAccess };

            var loginCheckResult = await _loginValidator.ValidateAsync(login, cancellationToken);
            if (loginCheckResult.Error != null)
                return new ResultDto<uint> { Error = loginCheckResult.Error };

            throw new NotImplementedException();
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
