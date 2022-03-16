using System;
using System.Threading;
using System.Threading.Tasks;
using Kalantyr.Auth.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kalantyr.Auth.Controllers
{
    [ApiController]
    [Route("logout")]
    public class LogoutController : ControllerBase
    {
        private readonly IAuthService _authService;

        public LogoutController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }

        [HttpPost]
        [Route("")]
        public async Task<IActionResult> LogoutAsync(CancellationToken cancellationToken)
        {
            var result = await _authService.LogoutAsync(GetToken(Request), cancellationToken);
            return Ok(result);
        }

        private static string GetToken(HttpRequest request)
        {
            if (request.Headers.TryGetValue("Auth-Token", out var token))
                return token;
            return null;
        }
    }
}
