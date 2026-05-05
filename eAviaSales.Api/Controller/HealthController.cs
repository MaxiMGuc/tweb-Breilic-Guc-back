using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/health")]
[ApiController]
public class HealthController : ControllerBase
{
    [HttpGet]
    public IActionResult Health()
    {
        return Ok(new { Status = "Healthy" });
    }
}
