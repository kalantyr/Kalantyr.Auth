using Kalantyr.Web;

namespace Kalantyr.Auth.Models
{
    public static class Errors
    {
        public static Error WrongPassword { get; } = new()
        {
            Code = nameof(WrongPassword),
            Message = "Incorrect password"
        };

        public static Error LoginNotFound { get; } = new()
        {
            Code = nameof(LoginNotFound),
            Message = "This login was not found"
        };

        public static Error TokenNotFound { get; } = new()
        {
            Code = nameof(TokenNotFound),
            Message = "Token was not found"
        };

        public static Error AdminOnlyAccess { get; } = new()
        {
            Code = nameof(AdminOnlyAccess),
            Message = "This action can only be performed by an administrator"
        };

        public static Error WrongAppKey { get; } = new()
        {
            Code = nameof(WrongAppKey),
            Message = "Incorrect application key"
        };

        public static Error WrongLoginFormat { get; } = new()
        {
            Code = nameof(WrongLoginFormat),
            Message = "Login must contain letters and numbers"
        };

        public static Error LoginExists { get; } = new()
        {
            Code = nameof(LoginExists),
            Message = "Login already exists"
        };

        public static Error WrongPasswordFormat { get; } = new()
        {
            Code = nameof(WrongPasswordFormat),
            Message = "Wrong password format"
        };
    }
}
