using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Models.Config;
using Kalantyr.Web;
using Microsoft.Extensions.Options;

namespace Kalantyr.Auth.Services.Impl
{
    public class AdminService: IAdminService
    {
        private readonly IUserStorageAdmin _userStorage;
        private readonly ITokenStorage _tokenStorage;
        private readonly AuthServiceConfig _config;

        public AdminService(IUserStorageAdmin userStorage, ITokenStorage tokenStorage, IOptions<AuthServiceConfig> config)
        {
            _userStorage = userStorage ?? throw new ArgumentNullException(nameof(userStorage));
            _tokenStorage = tokenStorage ?? throw new ArgumentNullException(nameof(tokenStorage));
            _config = config.Value;
        }

        public async Task<ResultDto<bool>> MigrateAsync(string token, CancellationToken cancellationToken)
        {
            #region TODO: выделить проверятор роли

            if (string.IsNullOrWhiteSpace(token))
                return new ResultDto<bool> { Error = Errors.TokenNotFound };

            var adminId = await _tokenStorage.GetUserIdByTokenAsync(token, cancellationToken);
            if (adminId == null)
                return new ResultDto<bool> { Error = Errors.TokenNotFound };

            cancellationToken.ThrowIfCancellationRequested();

            if (_config.Users.All(u => u.Id != adminId.Value))
                return new ResultDto<bool> { Error = Errors.AdminOnlyAccess };

            #endregion

            await _userStorage.MigrateAsync(cancellationToken);
            
            return ResultDto<bool>.Ok;
        }
    }
}
