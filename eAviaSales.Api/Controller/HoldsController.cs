using eAviaSales.Api.Contracts.Common;
using eAviaSales.Api.Contracts.Errors;
using eAviaSales.Api.Contracts.Holds;
using eAviaSales.Api.Services.Holds;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/events/{eventId:int}/holds")]
public sealed class HoldsController : ApiControllerBase
{
    private readonly IHoldService _holdService;
    private readonly ILogger<HoldsController> _logger;

    public HoldsController(IHoldService holdService, ILogger<HoldsController> logger)
    {
        _holdService = holdService;
        _logger = logger;
    }

    [ProducesResponseType(typeof(ApiResponse<CreateHoldResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status409Conflict)]
    [HttpPost]
    public ActionResult<ApiResponse<CreateHoldResponse>> CreateHold(
        int eventId,
        [FromBody] CreateHoldRequest request)
    {
        var result = _holdService.Create(eventId, request.SeatNumbers, request.HoldMinutes);
        if (!result.Success || result.Hold is null)
        {
            return Conflict(ApiResponse<object>.Fail(
                new ApiError
                {
                    Code = ApiErrorCodes.HoldConflict,
                    Message = $"One of the seats is already held: {result.ConflictSeat ?? "unknown"}."
                },
                HttpContext.TraceIdentifier));
        }

        _logger.LogInformation(
            "Created hold {HoldId} for event {EventId}, seats: {Seats}",
            result.Hold.HoldId,
            eventId,
            string.Join(", ", result.Hold.SeatNumbers));

        var response = new CreateHoldResponse
        {
            HoldId = result.Hold.HoldId,
            EventId = result.Hold.EventId,
            SeatNumbers = result.Hold.SeatNumbers,
            ExpiresAtUtc = result.Hold.ExpiresAtUtc
        };

        return StatusCode(StatusCodes.Status201Created, ApiResponse<CreateHoldResponse>.Ok(response, HttpContext.TraceIdentifier));
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)]
    [HttpDelete("{holdId}")]
    public IActionResult DeleteHold(int eventId, string holdId)
    {
        var deleted = _holdService.Delete(eventId, holdId);
        if (!deleted)
        {
            return NotFound(ApiResponse<object>.Fail(
                new ApiError
                {
                    Code = ApiErrorCodes.HoldNotFound,
                    Message = $"Hold {holdId} for event {eventId} was not found."
                },
                HttpContext.TraceIdentifier));
        }

        _logger.LogInformation("Deleted hold {HoldId} for event {EventId}", holdId, eventId);
        return NoContent();
    }
}
