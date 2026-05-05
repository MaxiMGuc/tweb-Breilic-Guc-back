using eAviaSales.BusinessLogic.Functions.Auth;
using eAviaSales.Domains.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login([FromBody] UserAuthRequest request)
    {
        var authFlow = new AuthFlow();
        var token = authFlow.LoginActionFlow(request);
        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new { Message = "Invalid credentials payload." });
        }

        return Ok(new { Token = token });
    }
}
