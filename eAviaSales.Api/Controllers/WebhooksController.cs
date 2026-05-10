using eAviaSales.Api.Helpers;
using eAviaSales.Api.Models.Payments;
using eAviaSales.Api.Models.Webhooks;
using eAviaSales.Api.Services.Payments;
using eAviaSales.Api.Services.Refunds;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controllers;

[ApiController]
[Route("api/webhooks")]
public sealed class WebhooksController : ControllerBase
{
    private readonly IPaymentService _paymentService;
    private readonly IRefundService _refundService;

    public WebhooksController(IPaymentService paymentService, IRefundService refundService)
    {
        _paymentService = paymentService;
        _refundService = refundService;
    }

    [ProducesResponseType(typeof(PaymentDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [HttpPost("payments")]
    public ActionResult<PaymentDetailsResponse> HandlePaymentWebhook([FromBody] PaymentWebhookPayload body)
    {
        var result = _paymentService.ApplyExternalStatus(body.PaymentId, body.Status);
        if (!result.Success)
        {
            return BadRequest(new
            {
                message = result.Message ?? "Webhook could not apply payment status."
            });
        }

        return Ok(ToDetails(result.Value!));
    }

    [ProducesResponseType(typeof(WebhookAcceptedDto), StatusCodes.Status200OK)]
    [HttpPost("refunds")]
    public ActionResult<WebhookAcceptedDto> HandleRefundWebhook([FromBody] RefundWebhookPayload body)
    {
        _refundService.UpsertStatusFromWebhook(body.RefundId, body.Status);
        return Ok(new WebhookAcceptedDto());
    }

    [HttpPost("notifications/delivery")]
    public IActionResult HandleNotificationDeliveryWebhook()
    {
        return ControllerNotImplemented.Feature("Notification delivery webhook");
    }

    private static PaymentDetailsResponse ToDetails(PaymentRecord r) =>
        new PaymentDetailsResponse
        {
            PaymentId = r.PaymentId,
            OrderId = r.OrderId,
            Amount = r.Amount,
            CurrencyCode = r.CurrencyCode,
            Status = r.Status
        };
}

