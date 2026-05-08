using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/users")]
public sealed class UsersController : ApiControllerBase
{
    [HttpGet("me")]
    public IActionResult GetProfile()
    {
        return NotImplementedResponse("Get user profile");
    }

    [HttpPatch("me")]
    public IActionResult UpdateProfile()
    {
        return NotImplementedResponse("Update user profile");
    }

    [HttpGet("me/orders")]
    public IActionResult GetUserOrders()
    {
        return NotImplementedResponse("Get user orders");
    }

    [HttpGet("me/tickets")]
    public IActionResult GetUserTickets()
    {
        return NotImplementedResponse("Get user tickets");
    }

    [HttpGet("me/notifications")]
    public IActionResult GetUserNotifications()
    {
        return NotImplementedResponse("Get user notifications");
    }
}
