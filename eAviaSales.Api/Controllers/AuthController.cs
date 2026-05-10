using eAviaSales.Api.Helpers;
using eAviaSales.BusinessLogic.Interface;
using eAviaSales.Domains.Models.User;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IAuthActions _authActions;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthActions authActions, ILogger<AuthController> logger)
    {
        _authActions = authActions;
        _logger = logger;
    }

    [ProducesResponseType(typeof(UserAuthResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost("login")]
    public ActionResult<UserAuthResponse> Login([FromBody] UserAuthRequest request)
    {
        _logger.LogInformation("Login attempt for user {Login}", request.Login);
        var response = _authActions.LoginActionFlow(request);
        if (response is null)
        {
            _logger.LogWarning("Login failed for user {Login}", request.Login);
            return Unauthorized(new { message = "Invalid credentials payload." });
        }

        _logger.LogInformation("Login succeeded for user {Login}", request.Login);
        return Ok(response);
    }

    [HttpPost("register")]
    public IActionResult Register()
    {
        return ControllerNotImplemented.Feature("Register");
    }

    [HttpPost("refresh")]
    public IActionResult RefreshToken()
    {
        return ControllerNotImplemented.Feature("Refresh token");
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        return ControllerNotImplemented.Feature("Logout");
    }
}

