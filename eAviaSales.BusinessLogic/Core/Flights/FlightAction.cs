using eAviaSales.Data;
using eAviaSales.Data.Entities;
using eAviaSales.Domains.Enums;
using eAviaSales.Domains.Models.Flight;
using Microsoft.EntityFrameworkCore;

namespace eAviaSales.BusinessLogic.Core.Flights;

public class FlightAction
{
    protected readonly AviaSalesDbContext Db;

    public FlightAction(AviaSalesDbContext db)
    {
        Db = db;
    }

    protected async Task<List<FlightTicketDto>> ExecuteSearchFlightsActionAsync(FlightSearchRequest request)
    {
        var flights = await Db.Flights
            .AsNoTracking()
            .Include(f => f.Airline)
            .Include(f => f.Segments)
            .ThenInclude(s => s.DepartureAirport)
            .Include(f => f.Segments)
            .ThenInclude(s => s.ArrivalAirport)
            .ToListAsync();

        var filtered = flights
            .Select(f => new { Flight = f, First = f.Segments.OrderBy(s => s.SegmentOrder).FirstOrDefault() })
            .Where(x => x.First is not null)
            .Where(x => MatchesRoute(x.First!, request.FromIataCode, request.ToIataCode))
            .Where(x => MatchesDepartureDate(x.First!, request.DepartureDateUtc))
            .Where(x => x.Flight.SeatsAvailable >= Math.Max(request.Adults, 1))
            .Select(x => x.Flight)
            .ToList();

        return filtered.Select(MapToDto).ToList();
    }

    protected async Task<FlightTicketDto?> ExecuteGetFlightByIdActionAsync(int id)
    {
        var flight = await Db.Flights
            .AsNoTracking()
            .Include(f => f.Airline)
            .Include(f => f.Segments)
            .ThenInclude(s => s.DepartureAirport)
            .Include(f => f.Segments)
            .ThenInclude(s => s.ArrivalAirport)
            .FirstOrDefaultAsync(f => f.Id == id);

        return flight is null ? null : MapToDto(flight);
    }

    private static bool MatchesRoute(FlightSegment first, string fromIataCode, string toIataCode)
    {
        var fromMatch = string.IsNullOrWhiteSpace(fromIataCode) ||
                        string.Equals(first.DepartureAirport.IataCode, fromIataCode, StringComparison.OrdinalIgnoreCase);
        var toMatch = string.IsNullOrWhiteSpace(toIataCode) ||
                      string.Equals(first.ArrivalAirport.IataCode, toIataCode, StringComparison.OrdinalIgnoreCase);
        return fromMatch && toMatch;
    }

    private static bool MatchesDepartureDate(FlightSegment first, DateTime departureDateUtc)
    {
        if (departureDateUtc == default)
        {
            return true;
        }

        return first.DepartureAtUtc.Date == departureDateUtc.Date;
    }

    private static FlightTicketDto MapToDto(Flight flight)
    {
        var first = flight.Segments.OrderBy(s => s.SegmentOrder).FirstOrDefault();
        if (first is null)
        {
            return new FlightTicketDto
            {
                Id = flight.Id,
                FlightNumber = flight.FlightNumber,
                AirlineName = flight.Airline.Name,
                Status = flight.Status.ToString(),
                Price = flight.BasePrice,
                CurrencyCode = flight.CurrencyCode,
                SeatsAvailable = flight.SeatsAvailable
            };
        }

        return new FlightTicketDto
        {
            Id = flight.Id,
            FlightNumber = flight.FlightNumber,
            AirlineName = flight.Airline.Name,
            FromIataCode = first.DepartureAirport.IataCode,
            ToIataCode = first.ArrivalAirport.IataCode,
            DepartureAtUtc = first.DepartureAtUtc,
            ArrivalAtUtc = first.ArrivalAtUtc,
            Price = flight.BasePrice,
            CurrencyCode = flight.CurrencyCode,
            SeatsAvailable = flight.SeatsAvailable,
            Status = flight.Status.ToString()
        };
    }
}
