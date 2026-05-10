using eAviaSales.Api.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controllers;

[ApiController]
[Route("api/refunds")]
public sealed class RefundsController : ControllerBase
{
    [HttpPost]
    public IActionResult CreateRefund() => ControllerNotImplemented.Feature("Create refund");

    [HttpGet("{refundId}")]
    public IActionResult GetRefundStatus(string refundId) =>
        ControllerNotImplemented.Feature($"Get refund status {refundId}");
}

