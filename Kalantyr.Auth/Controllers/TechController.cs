using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Kalantyr.Auth.Controllers
{
    [ApiController]
    [Route("tech")]
    public class TechController : ControllerBase
    {
        [HttpGet]
        [Route("version")]
        public async Task<IActionResult> GetVersionAsync(CancellationToken cancellationToken)
        {
            var v = typeof(TechController).Assembly.GetName().Version;
            return Ok(v.ToString());
        }
    }
}
