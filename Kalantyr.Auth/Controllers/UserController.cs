using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Services;
using Kalantyr.Auth.Utils;
using Kalantyr.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kalantyr.Auth.Controllers
{
    [ApiController]
    [Route("user")]
    public class UserController : ControllerBase
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpGet]
        [Route("id")]
        [ProducesResponseType(typeof(ResultDto<uint>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetUserIdAsync(CancellationToken cancellationToken)
        {
            var result = await _authService.GetUserIdAsync(Request.GetAuthToken(), Request.GetAppKey(), cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("createWithPassword")]
        public async Task<IActionResult> CreateUserWithPasswordAsync(string login, [FromBody] string password, CancellationToken cancellationToken)
        {
            var result = await _authService.CreateUserWithPasswordAsync(Request.GetAuthToken(), login, password, cancellationToken);
            return Ok(result);
        }

        [HttpPost]
        [Route("setPassword")]
        public async Task<IActionResult> SetPasswordAsync([FromBody] string[] passwords, CancellationToken cancellationToken)
        {
            var result = await _authService.SetPasswordAsync(Request.GetAuthToken(), passwords[0], passwords[1], cancellationToken);
            return Ok(result);
        }
    }
}
