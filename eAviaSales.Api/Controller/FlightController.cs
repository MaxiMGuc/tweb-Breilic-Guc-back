using eAviaSales.BusinessLogic.Interface;
using eAviaSales.Domains.Models.Flight;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/flights")]
[ApiController]
public class FlightController : ControllerBase
{
    private readonly IFlightActions _flightActions;
    private readonly ILogger<FlightController> _logger;

    public FlightController(IFlightActions flightActions, ILogger<FlightController> logger)
    {
        _flightActions = flightActions;
        _logger = logger;
    }

    [ProducesResponseType(typeof(FlightSearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [HttpPost("search")]
    public async Task<ActionResult<FlightSearchResponse>> SearchFlights([FromBody] FlightSearchRequest request)
    {
        _logger.LogInformation(
            "Searching flights {From}->{To} on {Date} for {Adults} adults",
            request.FromIataCode,
            request.ToIataCode,
            request.DepartureDateUtc.Date,
            request.Adults);
        var flights = await _flightActions.SearchFlightsActionAsync(request);
        _logger.LogInformation("Flights found: {Count}", flights.Count);
        return Ok(new FlightSearchResponse
        {
            Flights = flights
        });
    }

    [ProducesResponseType(typeof(FlightTicketDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetFlightById(int id)
    {
        _logger.LogInformation("Fetching flight by id {FlightId}", id);
        var flight = await _flightActions.GetFlightByIdActionAsync(id);
        if (flight is null)
        {
            _logger.LogWarning("Flight not found by id {FlightId}", id);
            return NotFound(new { Message = $"Flight with ID {id} not found." });
        }

        return Ok(flight);
    }
}
