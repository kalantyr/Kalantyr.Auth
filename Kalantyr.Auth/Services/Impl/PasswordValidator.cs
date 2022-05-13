using Kalantyr.Auth.Models;
using Kalantyr.Web;

namespace Kalantyr.Auth.Services.Impl
{
    public class PasswordValidator: IPasswordValidator
    {
        public ResultDto<bool> Validate(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return new ResultDto<bool> { Error = Errors.WrongPasswordFormat };

            if (password.Length < 5)
                return new ResultDto<bool> { Error = Errors.WrongPasswordFormat };

            return ResultDto<bool>.Ok;
        }
    }
}
