using eAviaSales.BusinessLogic.Interface;
using eAviaSales.Domains.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthActions _authActions;

    public AuthController(IAuthActions authActions)
    {
        _authActions = authActions;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserAuthRequest request)
    {
        var token = _authActions.LoginActionFlow(request);
        if (string.IsNullOrWhiteSpace(token))
        {
            return Unauthorized(new { Message = "Invalid credentials payload." });
        }

        return Ok(new { Token = token });
    }
}
