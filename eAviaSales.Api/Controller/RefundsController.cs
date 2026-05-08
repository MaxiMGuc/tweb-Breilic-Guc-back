using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/refunds")]
public sealed class RefundsController : ApiControllerBase
{
    [HttpPost]
    public IActionResult CreateRefund()
    {
        return NotImplementedResponse("Create refund");
    }

    [HttpGet("{refundId}")]
    public IActionResult GetRefundStatus(string refundId)
    {
        return NotImplementedResponse($"Get refund status {refundId}");
    }
}
