using eAviaSales.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public sealed class TicketsController : ControllerBase
{
    [HttpGet("{ticketId}")]
    public IActionResult GetTicket(string ticketId) =>
        ControllerNotImplemented.Feature($"Get ticket {ticketId}");

    [HttpGet("{ticketId}/qr")]
    public IActionResult GetTicketQr(string ticketId) =>
        ControllerNotImplemented.Feature($"Get ticket QR {ticketId}");

    [HttpPost("validate")]
    public IActionResult ValidateTicket() => ControllerNotImplemented.Feature("Validate ticket");

    [HttpPost("{ticketId}/check-in")]
    public IActionResult CheckInTicket(string ticketId) =>
        ControllerNotImplemented.Feature($"Check-in ticket {ticketId}");

    [HttpGet("{ticketId}/validation-history")]
    public IActionResult GetValidationHistory(string ticketId) =>
        ControllerNotImplemented.Feature($"Validation history for ticket {ticketId}");
}

