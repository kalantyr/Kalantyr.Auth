using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Services;
using Kalantyr.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kalantyr.Auth.Controllers
{
    [ApiController]
    [Route("admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService ?? throw new ArgumentNullException(nameof(adminService));
        }

        [HttpPost]
        [Route("migrate")]
        [ProducesResponseType(typeof(ResultDto<bool>), StatusCodes.Status200OK)]
        public async Task<IActionResult> MigrateAsync(CancellationToken cancellationToken)
        {
            return await WrapExceptionAsync(
                _adminService.MigrateAsync(Request.GetAuthToken(), cancellationToken));
        }

        private async Task<ObjectResult> WrapExceptionAsync<T>(Task<T> func)
        {
            try
            {
                return Ok(await func);
            }
            catch (Exception e)
            {
                var error = e.GetBaseException();
                return StatusCode(StatusCodes.Status500InternalServerError, error.GetType().Name + ": " + error.Message);
            }
        }
    }
}
