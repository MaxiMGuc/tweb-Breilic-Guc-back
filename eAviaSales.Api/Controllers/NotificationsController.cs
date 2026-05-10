using eAviaSales.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controllers;

[ApiController]
[Route("api/notifications")]
public sealed class NotificationsController : ControllerBase
{
    [HttpPost("email/send")]
    public IActionResult SendEmail() => ControllerNotImplemented.Feature("Send notification email");
}

