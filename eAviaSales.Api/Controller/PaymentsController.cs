using eAviaSales.Api.Contracts.Payments;
using eAviaSales.Api.Services.Payments;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[ApiController]
[Route("api/payments")]
public sealed class PaymentsController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public PaymentsController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [ProducesResponseType(typeof(PaymentIntentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("intents")]
    public ActionResult CreatePaymentIntent([FromBody] PaymentIntentRequest request)
    {
        var result = _paymentService.CreateIntent(request.OrderId);
        if (!result.Success)
        {
            return PaymentProblem(result.ErrorCode, result.Message);
        }

        var r = result.Value!;
        var dto = new PaymentIntentResponse
        {
            PaymentId = r.PaymentId,
            OrderId = r.OrderId,
            Amount = r.Amount,
            CurrencyCode = r.CurrencyCode,
            Status = r.Status
        };

        return StatusCode(StatusCodes.Status201Created, dto);
    }

    [ProducesResponseType(typeof(PaymentDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost("{paymentId}/confirm")]
    public ActionResult ConfirmPayment(string paymentId)
    {
        var result = _paymentService.Confirm(paymentId);
        if (!result.Success)
        {
            return PaymentProblem(result.ErrorCode, result.Message);
        }

        return Ok(ToDetails(result.Value!));
    }

    [ProducesResponseType(typeof(PaymentDetailsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{paymentId}")]
    public ActionResult<PaymentDetailsResponse> GetPaymentStatus(string paymentId)
    {
        var record = _paymentService.Get(paymentId);
        if (record is null)
        {
            return NotFound(new { message = "Payment was not found." });
        }

        return Ok(ToDetails(record));
    }

    private ActionResult PaymentProblem(string? errorCode, string? message)
    {
        var status = errorCode switch
        {
            "order_not_found" or "payment_not_found" => StatusCodes.Status404NotFound,
            "invalid_order_state" => StatusCodes.Status409Conflict,
            "order_payment_failed" => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest
        };

        return StatusCode(status, new { message = message ?? "Request failed." });
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
