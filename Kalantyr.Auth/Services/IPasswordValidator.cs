using Kalantyr.Web;

namespace Kalantyr.Auth.Services
{
    public interface IPasswordValidator
    {
        ResultDto<bool> Validate(string password);
    }
}
