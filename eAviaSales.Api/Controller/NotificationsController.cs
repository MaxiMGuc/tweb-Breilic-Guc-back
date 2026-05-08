using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/notifications")]
public sealed class NotificationsController : ApiControllerBase
{
    [HttpPost("email/send")]
    public IActionResult SendEmail()
    {
        return NotImplementedResponse("Send notification email");
    }
}
