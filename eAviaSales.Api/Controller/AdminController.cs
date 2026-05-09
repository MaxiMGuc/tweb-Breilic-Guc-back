using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[ApiController]
[Route("api/admin")]
public sealed class AdminController : ControllerBase
{
    [HttpGet("events")]
    public IActionResult GetEvents() => ControllerNotImplemented.Feature("Admin events list");

    [HttpPost("events")]
    public IActionResult CreateEvent() => ControllerNotImplemented.Feature("Admin create event");

    [HttpPatch("events/{eventId:int}")]
    public IActionResult UpdateEvent(int eventId) =>
        ControllerNotImplemented.Feature($"Admin update event {eventId}");

    [HttpPost("events/{eventId:int}/publish")]
    public IActionResult PublishEvent(int eventId) =>
        ControllerNotImplemented.Feature($"Admin publish event {eventId}");

    [HttpPost("events/{eventId:int}/close-sales")]
    public IActionResult CloseSales(int eventId) =>
        ControllerNotImplemented.Feature($"Admin close sales for event {eventId}");

    [HttpGet("orders")]
    public IActionResult GetOrders() => ControllerNotImplemented.Feature("Admin orders");

    [HttpGet("payments")]
    public IActionResult GetPayments() => ControllerNotImplemented.Feature("Admin payments");

    [HttpPost("refunds/{refundId}/approve")]
    public IActionResult ApproveRefund(string refundId) =>
        ControllerNotImplemented.Feature($"Admin approve refund {refundId}");

    [HttpGet("reports/sales")]
    public IActionResult GetSalesReport() => ControllerNotImplemented.Feature("Admin sales report");
}
