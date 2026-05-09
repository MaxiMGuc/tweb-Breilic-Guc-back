using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[ApiController]
[Route("api/users")]
public sealed class UsersController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetProfile() => ControllerNotImplemented.Feature("Get user profile");

    [HttpPatch("me")]
    public IActionResult UpdateProfile() => ControllerNotImplemented.Feature("Update user profile");

    [HttpGet("me/orders")]
    public IActionResult GetUserOrders() => ControllerNotImplemented.Feature("Get user orders");

    [HttpGet("me/tickets")]
    public IActionResult GetUserTickets() => ControllerNotImplemented.Feature("Get user tickets");

    [HttpGet("me/notifications")]
    public IActionResult GetUserNotifications() => ControllerNotImplemented.Feature("Get user notifications");
}
