using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Web;

namespace Kalantyr.Auth.Services.Impl
{
    public class AuthorizationService: IAuthorizationService
    {
        private readonly ITokenStorage _tokenStorage;
        private readonly AuthServiceConfig _config;

        public AuthorizationService(ITokenStorage tokenStorage, AuthServiceConfig config)
        {
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
            _config = config ?? throw new ArgumentNullException(nameof(config));
        }

        public async Task<ResultDto<bool>> IsAdminAsync(string userToken, CancellationToken cancellationToken)
        {
            var adminId = await _tokenStorage.GetUserIdByTokenAsync(userToken, cancellationToken);
            if (adminId == null)
                return new ResultDto<bool> { Error = Errors.TokenNotFound };

            cancellationToken.ThrowIfCancellationRequested();

            if (_config.Users.All(u => u.Id != adminId.Value))
                return new ResultDto<bool> { Error = Errors.AdminOnlyAccess };

            return ResultDto<bool>.Ok;
        }
    }
}
