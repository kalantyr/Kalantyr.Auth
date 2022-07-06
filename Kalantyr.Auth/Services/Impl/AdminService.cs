using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
using Kalantyr.Auth.Models;
using Kalantyr.Web;

namespace Kalantyr.Auth.Services.Impl
{
    public class AdminService: IAdminService
    {
        private readonly IUserStorageAdmin _userStorage;
        private readonly IAuthorizationService _authorizationService;

        public AdminService(IUserStorageAdmin userStorage, IAuthorizationService authorizationService)
        {
            _userStorage = userStorage ?? throw new ArgumentNullException(nameof(userStorage));
            _authorizationService = authorizationService ?? throw new ArgumentNullException(nameof(authorizationService));
        }

        public async Task<ResultDto<bool>> MigrateAsync(string token, CancellationToken cancellationToken)
        {
            var isAdminResult = await _authorizationService.IsAdminAsync(token, cancellationToken);
            if (isAdminResult.Error != null)
                return ResultDto<bool>.ErrorFrom(isAdminResult);

            if (!isAdminResult.Result)
                return new ResultDto<bool> { Error = Errors.AdminOnlyAccess };

            cancellationToken.ThrowIfCancellationRequested();

            await _userStorage.MigrateAsync(cancellationToken);
            
            return ResultDto<bool>.Ok;
        }
    }
}
