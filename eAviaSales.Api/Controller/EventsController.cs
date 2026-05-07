using eAviaSales.Api.Contracts.Common;
using eAviaSales.Api.Contracts.Events;
using eAviaSales.BusinessLogic.Interface;
using eAviaSales.Domains.Models.Flight;
using Microsoft.AspNetCore.Mvc;

namespace eAviaSales.Api.Controller;

[Route("api/events")]
public sealed class EventsController : ApiControllerBase
{
    private readonly IFlightActions _flightActions;
    private readonly ILogger<EventsController> _logger;

    public EventsController(IFlightActions flightActions, ILogger<EventsController> logger)
    {
        _flightActions = flightActions;
        _logger = logger;
    }

    [ProducesResponseType(typeof(ApiResponse<EventsCatalogResponse>), StatusCodes.Status200OK)]
    [HttpGet]
    public async Task<ActionResult<ApiResponse<EventsCatalogResponse>>> GetEvents(
        [FromQuery] string fromIataCode,
        [FromQuery] string toIataCode,
        [FromQuery] DateTime departureDateUtc,
        [FromQuery] int adults = 1,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var searchRequest = new FlightSearchRequest
        {
            FromIataCode = fromIataCode,
            ToIataCode = toIataCode,
            DepartureDateUtc = departureDateUtc,
            Adults = adults
        };

        _logger.LogInformation(
            "Events catalog request {From}->{To} {Date}, page {Page}, pageSize {PageSize}",
            fromIataCode,
            toIataCode,
            departureDateUtc.Date,
            page,
            pageSize);

        var flights = await _flightActions.SearchFlightsActionAsync(searchRequest);
        var totalItems = flights.Count;
        var items = flights
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(MapToEventSummary)
            .ToList();

        return OkResponse(new EventsCatalogResponse
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        });
    }

    [ProducesResponseType(typeof(ApiResponse<EventDetailsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [HttpGet("{eventId:int}")]
    public async Task<ActionResult<ApiResponse<EventDetailsResponse>>> GetEventById(int eventId)
    {
        var flight = await _flightActions.GetFlightByIdActionAsync(eventId);
        if (flight is null)
        {
            return NotFound(new { Message = $"Event with ID {eventId} not found." });
        }

        return OkResponse(MapToEventDetails(flight));
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

    private static EventSummaryDto MapToEventSummary(FlightTicketDto flight)
    {
        return new EventSummaryDto
        {
            EventId = flight.Id,
            EventCode = flight.FlightNumber,
            ProviderName = flight.AirlineName,
            FromIataCode = flight.FromIataCode,
            ToIataCode = flight.ToIataCode,
            StartsAtUtc = flight.DepartureAtUtc,
            EndsAtUtc = flight.ArrivalAtUtc,
            Price = flight.Price,
            CurrencyCode = flight.CurrencyCode,
            AvailabilityStatus = flight.Status
        };
    }

    private static EventDetailsResponse MapToEventDetails(FlightTicketDto flight)
    {
        return new EventDetailsResponse
        {
            EventId = flight.Id,
            EventCode = flight.FlightNumber,
            ProviderName = flight.AirlineName,
            FromIataCode = flight.FromIataCode,
            ToIataCode = flight.ToIataCode,
            StartsAtUtc = flight.DepartureAtUtc,
            EndsAtUtc = flight.ArrivalAtUtc,
            Price = flight.Price,
            CurrencyCode = flight.CurrencyCode,
            SeatsAvailable = flight.SeatsAvailable,
            AvailabilityStatus = flight.Status
        };
    }
}
