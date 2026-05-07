using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/events")]
public sealed class EventsController : ApiControllerBase
{
    [HttpGet]
    public IActionResult GetEvents()
    {
        return NotImplementedResponse("Events catalog");
    }

    [HttpGet("{eventId:int}")]
    public IActionResult GetEventById(int eventId)
    {
        return NotImplementedResponse($"Event details {eventId}");
    }

    [HttpGet("{eventId:int}/availability")]
    public IActionResult GetAvailability(int eventId)
    {
        return NotImplementedResponse($"Event availability {eventId}");
    }

    [HttpGet("{eventId:int}/seat-map")]
    public IActionResult GetSeatMap(int eventId)
    {
        return NotImplementedResponse($"Event seat map {eventId}");
    }

    [HttpGet("{eventId:int}/seats")]
    public IActionResult GetSeats(int eventId)
    {
        return NotImplementedResponse($"Event seats {eventId}");
    }
}
