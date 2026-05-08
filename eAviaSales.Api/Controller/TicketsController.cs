using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/tickets")]
public sealed class TicketsController : ApiControllerBase
{
    [HttpGet("{ticketId}")]
    public IActionResult GetTicket(string ticketId)
    {
        return NotImplementedResponse($"Get ticket {ticketId}");
    }

    [HttpGet("{ticketId}/qr")]
    public IActionResult GetTicketQr(string ticketId)
    {
        return NotImplementedResponse($"Get ticket QR {ticketId}");
    }

    [HttpPost("validate")]
    public IActionResult ValidateTicket()
    {
        return NotImplementedResponse("Validate ticket");
    }

    [HttpPost("{ticketId}/check-in")]
    public IActionResult CheckInTicket(string ticketId)
    {
        return NotImplementedResponse($"Check-in ticket {ticketId}");
    }

    [HttpGet("{ticketId}/validation-history")]
    public IActionResult GetValidationHistory(string ticketId)
    {
        return NotImplementedResponse($"Validation history for ticket {ticketId}");
    }
}
