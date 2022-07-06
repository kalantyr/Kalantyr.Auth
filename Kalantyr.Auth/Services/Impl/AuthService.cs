using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Web;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Kalantyr.Auth.Services.Impl
{
    public class AuthService: IAuthService, IHealthCheck
    {
        private static readonly ResultDto<TokenInfo> LoginNotFound = new() { Error = Errors.LoginNotFound };
        private static readonly ResultDto<TokenInfo> WrongPassword = new() { Error = Errors.WrongPassword };

        private readonly IUserStorage _userStorage;
        private readonly IHashCalculator _hashCalculator;
        private readonly ITokenStorage _tokenStorage;
        private readonly ILoginValidator _loginValidator;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IAuthorizationService _authorizationService;
        private readonly AuthServiceConfig _config;

        public AuthService(IUserStorage userStorage, IHashCalculator hashCalculator, ITokenStorage tokenStorage, IOptions<AuthServiceConfig> config, ILoginValidator loginValidator, IPasswordValidator passwordValidator, IAuthorizationService authorizationService)
        {
            _userStorage = userStorage ?? throw new ArgumentNullException(nameof(userStorage));
            _hashCalculator = hashCalculator ?? throw new ArgumentNullException(nameof(hashCalculator));
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
            _loginValidator = loginValidator ?? throw new ArgumentNullException(nameof(loginValidator));
            _passwordValidator = passwordValidator ?? throw new ArgumentNullException(nameof(passwordValidator));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
            _config = config.Value;
        }

        /// <inheritdoc/>
        public async Task<ResultDto<TokenInfo>> LoginAsync(LoginPasswordDto dto, CancellationToken cancellationToken)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));

            var userRecord = await _userStorage.GetUserIdByLoginAsync(dto.Login, cancellationToken);

            if (userRecord == null)
                return LoginNotFound;

            if (userRecord.IsDisabled)
                return new ResultDto<TokenInfo> { Error = Errors.UserIsInactive };

            var userId = userRecord.Id;

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
            if (!_config.SelfCreationOfAccounts)
            {
                var isAdminResult = await _authorizationService.IsAdminAsync(token, cancellationToken);
                if (isAdminResult.Error != null)
                    return ResultDto<uint>.ErrorFrom(isAdminResult);
                if (!isAdminResult.Result)
                    return new ResultDto<uint> { Error = Errors.AdminOnlyAccess };
            }

            var loginCheckResult = await _loginValidator.ValidateAsync(login, cancellationToken);
            if (loginCheckResult.Error != null)
                return ResultDto<uint>.ErrorFrom(loginCheckResult);

            cancellationToken.ThrowIfCancellationRequested();

            var passwordCheckResult = _passwordValidator.Validate(password);
            if (passwordCheckResult.Error != null)
                return ResultDto<uint>.ErrorFrom(passwordCheckResult);

            var userId = await _userStorage.CreateAsync(login, cancellationToken);

            var salt = GenerateToken();
            var passwordRecord = new PasswordRecord
            {
                UserId = userId,
                Salt = salt,
                PasswordHash = _hashCalculator.GetHash(password, salt)
            };
            await _userStorage.SetPasswordAsync(passwordRecord, cancellationToken);

            return new ResultDto<uint> { Result = userId };
        }

        public async Task<ResultDto<bool>> SetPasswordAsync(string token, string oldPassword, string newPassword, CancellationToken cancellationToken)
        {
            var userId = await _tokenStorage.GetUserIdByTokenAsync(token, cancellationToken);
            if (userId == null)
                return new ResultDto<bool> { Error = Errors.TokenNotFound };

            cancellationToken.ThrowIfCancellationRequested();

            var passwordCheckResult = _passwordValidator.Validate(newPassword);
            if (passwordCheckResult.Error != null)
                return ResultDto<bool>.ErrorFrom(passwordCheckResult);

            cancellationToken.ThrowIfCancellationRequested();

            var userRecord = await _userStorage.GetUserRecordAsync(userId.Value, cancellationToken);
            if (userRecord.IsDisabled)
                return new ResultDto<bool> { Error = Errors.UserIsInactive };

            cancellationToken.ThrowIfCancellationRequested();

            var passwordRecord = await _userStorage.GetPasswordRecordAsync(userId.Value, cancellationToken);
            var oldHash = _hashCalculator.GetHash(oldPassword, passwordRecord.Salt);
            if (oldHash != passwordRecord.PasswordHash)
                return new ResultDto<bool> { Error = Errors.WrongPassword };

            cancellationToken.ThrowIfCancellationRequested();

            var salt = GenerateToken();
            passwordRecord = new PasswordRecord
            {
                UserId = userId.Value,
                Salt = salt,
                PasswordHash = _hashCalculator.GetHash(newPassword, salt)
            };
            await _userStorage.SetPasswordAsync(passwordRecord, cancellationToken);

            return ResultDto<bool>.Ok;
        }

        private static string GenerateToken()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = new CancellationToken())
        {
            try
            {
                if (_userStorage is IHealthCheck hc)
                {
                    var res = await hc.CheckHealthAsync(context, cancellationToken);
                    if (res.Status != HealthStatus.Healthy)
                        return res;
                }

                return HealthCheckResult.Healthy();
            }
            catch (Exception e)
            {
                return HealthCheckResult.Unhealthy(nameof(AuthService), e);
            }
        }
    }
}
