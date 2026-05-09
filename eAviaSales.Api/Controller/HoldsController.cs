using eAviaSales.Api.Contracts.Holds;
using eAviaSales.Api.Services.Holds;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[ApiController]
[Route("api/events/{eventId:int}/holds")]
public sealed class HoldsController : ControllerBase
{
    private readonly IHoldService _holdService;
    private readonly ILogger<HoldsController> _logger;

    public HoldsController(IHoldService holdService, ILogger<HoldsController> logger)
    {
        _holdService = holdService;
        _logger = logger;
    }

    [ProducesResponseType(typeof(CreateHoldResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [HttpPost]
    public ActionResult<CreateHoldResponse> CreateHold(int eventId, [FromBody] CreateHoldRequest request)
    {
        var result = _holdService.Create(eventId, request.SeatNumbers, request.HoldMinutes);
        if (!result.Success || result.Hold is null)
        {
            return Conflict(new
            {
                message = $"One of the seats is already held: {result.ConflictSeat ?? "unknown"}."
            });
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

        return StatusCode(StatusCodes.Status201Created, response);
    }

    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpDelete("{holdId}")]
    public IActionResult DeleteHold(int eventId, string holdId)
    {
        var deleted = _holdService.Delete(eventId, holdId);
        if (!deleted)
        {
            return NotFound(new { message = $"Hold {holdId} for event {eventId} was not found." });
        }

        _logger.LogInformation("Deleted hold {HoldId} for event {EventId}", holdId, eventId);
        return NoContent();
    }
}
