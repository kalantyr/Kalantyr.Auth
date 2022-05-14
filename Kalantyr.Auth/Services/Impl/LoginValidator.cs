using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.InternalModels;
using Kalantyr.Auth.Models;
using Kalantyr.Web;

namespace Kalantyr.Auth.Services.Impl
{
    public class LoginValidator: ILoginValidator
    {
        private static readonly ResultDto<bool> WrongLoginFormat = new() { Error = Errors.WrongLoginFormat };

        private readonly IUserStorageReadonly _userStorage;

        public LoginValidator(IUserStorageReadonly userStorage)
        {
            _userStorage = userStorage ?? throw new ArgumentNullException(nameof(userStorage));
        }

        public async Task<ResultDto<bool>> ValidateAsync(string login, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(login))
                return WrongLoginFormat;

            login = login.Trim();

            if (!login.Any(char.IsLetter))
                return WrongLoginFormat;

            if (!login.All(char.IsLetterOrDigit))
                return WrongLoginFormat;

            var id = await _userStorage.GetUserIdByLoginAsync(login, cancellationToken);
            if (id != null)
                return new ResultDto<bool> { Error = Errors.LoginExists };

            return ResultDto<bool>.Ok;
        }
    }
}
