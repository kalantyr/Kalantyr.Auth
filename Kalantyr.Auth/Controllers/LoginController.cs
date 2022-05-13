using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Models;
using Kalantyr.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace Kalantyr.Auth.Controllers
{
    [ApiController]
    [Route("login")]
    public class LoginController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LoginController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost]
        [Route("byPassword")]
        public async Task<IActionResult> ByPasswordAsync([FromBody] LoginPasswordDto dto, CancellationToken cancellationToken)
        {
            var loginResult = await _authService.LoginAsync(dto, cancellationToken);
            return Ok(loginResult);
        }
    }
}
