using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api")]
public sealed class OrdersController : ApiControllerBase
{
    [HttpPost("checkout")]
    public IActionResult Checkout()
    {
        return NotImplementedResponse("Checkout");
    }

    [HttpPost("orders")]
    public IActionResult CreateOrder()
    {
        return NotImplementedResponse("Create order");
    }

    [HttpGet("orders/{orderId}")]
    public IActionResult GetOrder(string orderId)
    {
        return NotImplementedResponse($"Get order {orderId}");
    }

    [HttpGet("orders")]
    public IActionResult GetOrders()
    {
        return NotImplementedResponse("Get user orders");
    }

    [HttpPost("orders/{orderId}/cancel")]
    public IActionResult CancelOrder(string orderId)
    {
        return NotImplementedResponse($"Cancel order {orderId}");
    }

    [HttpPost("orders/{orderId}/issue")]
    public IActionResult IssueTickets(string orderId)
    {
        return NotImplementedResponse($"Issue tickets for order {orderId}");
    }

    [HttpGet("orders/{orderId}/tickets")]
    public IActionResult GetOrderTickets(string orderId)
    {
        return NotImplementedResponse($"Get tickets for order {orderId}");
    }
}
