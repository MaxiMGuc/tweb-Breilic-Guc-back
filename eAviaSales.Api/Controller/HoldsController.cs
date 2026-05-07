using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/events/{eventId:int}/holds")]
public sealed class HoldsController : ApiControllerBase
{
    [HttpPost]
    public IActionResult CreateHold(int eventId)
    {
        return NotImplementedResponse($"Create hold for event {eventId}");
    }

    [HttpDelete("{holdId}")]
    public IActionResult DeleteHold(int eventId, string holdId)
    {
        return NotImplementedResponse($"Delete hold {holdId} for event {eventId}");
    }
}
