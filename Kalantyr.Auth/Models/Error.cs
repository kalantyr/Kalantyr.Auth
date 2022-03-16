namespace Kalantyr.Auth.Models
{
    public class Error
    {
        public string Code { get; set; }

        public string Message { get; set; }
    }

    public static class Errors
    {
        public static Error WrongPassword { get; } = new Error
        {
            Code = nameof(WrongPassword),
            Message = "Incorrect password"
        };

        public static Error LoginNotFound { get; } = new Error
        {
            Code = nameof(LoginNotFound),
            Message = "This login was not found"
        };

        public static Error TokenNotFound { get; } = new Error
        {
            Code = nameof(TokenNotFound),
            Message = "Token was not found"
        };
    }
}
