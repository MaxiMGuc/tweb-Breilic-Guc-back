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
    public ActionResult<UserAuthResponse> Login([FromBody] UserAuthRequest request)
    {
        var response = _authActions.LoginActionFlow(request);
        if (response is null)
        {
            return Unauthorized(new { Message = "Invalid credentials payload." });
        }

        return Ok(response);
    }
}
