using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/payments")]
public sealed class PaymentsController : ApiControllerBase
{
    [HttpPost("intents")]
    public IActionResult CreatePaymentIntent()
    {
        return NotImplementedResponse("Create payment intent");
    }

    [HttpPost("{paymentId}/confirm")]
    public IActionResult ConfirmPayment(string paymentId)
    {
        return NotImplementedResponse($"Confirm payment {paymentId}");
    }

    [HttpGet("{paymentId}")]
    public IActionResult GetPaymentStatus(string paymentId)
    {
        return NotImplementedResponse($"Get payment status {paymentId}");
    }
}
