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
    [ProducesResponseType(typeof(ApiResponse<EventAvailabilityResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EventAvailabilityResponse>>> GetAvailability(int eventId)
    {
        var flight = await _flightActions.GetFlightByIdActionAsync(eventId);
        if (flight is null)
        {
            return NotFound(new { Message = $"Event with ID {eventId} not found." });
        }

        var response = MapToAvailability(flight);
        return OkResponse(response);
    }

    [HttpGet("{eventId:int}/seat-map")]
    [ProducesResponseType(typeof(ApiResponse<EventSeatMapResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EventSeatMapResponse>>> GetSeatMap(int eventId)
    {
        var flight = await _flightActions.GetFlightByIdActionAsync(eventId);
        if (flight is null)
        {
            return NotFound(new { Message = $"Event with ID {eventId} not found." });
        }

        var response = BuildSeatMap(flight);
        return OkResponse(response);
    }

    [HttpGet("{eventId:int}/seats")]
    [ProducesResponseType(typeof(ApiResponse<EventSeatsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApiResponse<EventSeatsResponse>>> GetSeats(int eventId)
    {
        var flight = await _flightActions.GetFlightByIdActionAsync(eventId);
        if (flight is null)
        {
            return NotFound(new { Message = $"Event with ID {eventId} not found." });
        }

        var seats = BuildSeatList(flight, 60);
        return OkResponse(new EventSeatsResponse
        {
            EventId = eventId,
            Seats = seats
        });
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

    private static EventAvailabilityResponse MapToAvailability(FlightTicketDto flight)
    {
        const int totalSeats = 60;
        var availableSeats = Math.Clamp(flight.SeatsAvailable, 0, totalSeats);
        var soldSeats = totalSeats - availableSeats;

        return new EventAvailabilityResponse
        {
            EventId = flight.Id,
            TotalSeats = totalSeats,
            AvailableSeats = availableSeats,
            HeldSeats = 0,
            SoldSeats = soldSeats,
            AvailabilityStatus = flight.Status
        };
    }

    private static EventSeatMapResponse BuildSeatMap(FlightTicketDto flight)
    {
        var seats = BuildSeatList(flight, 60);
        var rows = seats
            .Select((seat, index) => new { seat, index })
            .GroupBy(pair => (pair.index / 6) + 1)
            .Select(group => new EventSeatMapRowDto
            {
                RowNumber = group.Key,
                Seats = group.Select(pair => pair.seat).ToList()
            })
            .ToList();

        return new EventSeatMapResponse
        {
            EventId = flight.Id,
            Layout = "3-3",
            Rows = rows
        };
    }

    private static IReadOnlyList<EventSeatDto> BuildSeatList(FlightTicketDto flight, int totalSeats)
    {
        var availableSeats = Math.Clamp(flight.SeatsAvailable, 0, totalSeats);
        var soldSeats = totalSeats - availableSeats;
        var seatLetters = new[] { "A", "B", "C", "D", "E", "F" };
        var seats = new List<EventSeatDto>(totalSeats);
        var soldCounter = 0;

        for (var row = 1; row <= totalSeats / 6; row++)
        {
            foreach (var letter in seatLetters)
            {
                var status = soldCounter < soldSeats ? "sold" : "free";
                if (status == "sold")
                {
                    soldCounter++;
                }

                seats.Add(new EventSeatDto
                {
                    SeatNumber = $"{row}{letter}",
                    CabinClass = "Economy",
                    Status = status
                });
            }
        }

        return seats;
    }
}
