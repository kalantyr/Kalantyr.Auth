using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Web;

namespace Kalantyr.Auth.Services
{
    public interface ILoginValidator
    {
        Task<ResultDto<bool>> ValidateAsync(string login, CancellationToken cancellationToken);
    }
}
