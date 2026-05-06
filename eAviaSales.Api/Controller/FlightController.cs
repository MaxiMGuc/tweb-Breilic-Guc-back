using eAviaSales.BusinessLogic.Interface;
using eAviaSales.Domains.Models.Flight;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/flights")]
[ApiController]
public class FlightController : ControllerBase
{
    private readonly IFlightActions _flightActions;

    public FlightController(IFlightActions flightActions)
    {
        _flightActions = flightActions;
    }

    [HttpPost("search")]
    public async Task<ActionResult<FlightSearchResponse>> SearchFlights([FromBody] FlightSearchRequest request)
    {
        var flights = await _flightActions.SearchFlightsActionAsync(request);
        return Ok(new FlightSearchResponse
        {
            Flights = flights
        });
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetFlightById(int id)
    {
        var flight = await _flightActions.GetFlightByIdActionAsync(id);
        if (flight is null)
        {
            return NotFound(new { Message = $"Flight with ID {id} not found." });
        }

        return Ok(flight);
    }
}
