using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/admin")]
public sealed class AdminController : ApiControllerBase
{
    [HttpGet("events")]
    public IActionResult GetEvents()
    {
        return NotImplementedResponse("Admin events list");
    }

    [HttpPost("events")]
    public IActionResult CreateEvent()
    {
        return NotImplementedResponse("Admin create event");
    }

    [HttpPatch("events/{eventId:int}")]
    public IActionResult UpdateEvent(int eventId)
    {
        return NotImplementedResponse($"Admin update event {eventId}");
    }

    [HttpPost("events/{eventId:int}/publish")]
    public IActionResult PublishEvent(int eventId)
    {
        return NotImplementedResponse($"Admin publish event {eventId}");
    }

    [HttpPost("events/{eventId:int}/close-sales")]
    public IActionResult CloseSales(int eventId)
    {
        return NotImplementedResponse($"Admin close sales for event {eventId}");
    }

    [HttpGet("orders")]
    public IActionResult GetOrders()
    {
        return NotImplementedResponse("Admin orders");
    }

    [HttpGet("payments")]
    public IActionResult GetPayments()
    {
        return NotImplementedResponse("Admin payments");
    }

    [HttpPost("refunds/{refundId}/approve")]
    public IActionResult ApproveRefund(string refundId)
    {
        return NotImplementedResponse($"Admin approve refund {refundId}");
    }

    [HttpGet("reports/sales")]
    public IActionResult GetSalesReport()
    {
        return NotImplementedResponse("Admin sales report");
    }
}
