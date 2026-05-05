using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    [HttpGet("ping")]
    public IActionResult Ping()
    {
        return Ok("pong");
    }
}
