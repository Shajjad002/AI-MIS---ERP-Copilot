using Microsoft.AspNetCore.Mvc;

namespace AI.MIS.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login() => StatusCode(StatusCodes.Status501NotImplemented, new { message = "Authentication is scaffolded and will be implemented before protected features are enabled." });
}
