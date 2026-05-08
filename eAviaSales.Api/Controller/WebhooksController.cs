using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/webhooks")]
public sealed class WebhooksController : ApiControllerBase
{
    [HttpPost("payments")]
    public IActionResult HandlePaymentWebhook()
    {
        return NotImplementedResponse("Payment webhook");
    }

    [HttpPost("refunds")]
    public IActionResult HandleRefundWebhook()
    {
        return NotImplementedResponse("Refund webhook");
    }

    [HttpPost("notifications/delivery")]
    public IActionResult HandleNotificationDeliveryWebhook()
    {
        return NotImplementedResponse("Notification delivery webhook");
    }
}
