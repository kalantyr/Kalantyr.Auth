using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Services;
using Kalantyr.Auth.Utils;
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
        public async Task<IActionResult> GetUserIdAsync(CancellationToken cancellationToken)
        {
            var result = await _authService.GetUserIdAsync(Request.GetAuthToken(), Request.GetAppKey(), cancellationToken);
            return Ok(result);
        }
    }
}
