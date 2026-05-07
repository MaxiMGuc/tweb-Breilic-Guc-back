using eAviaSales.Data.Entities;
using eAviaSales.Domains.Enums;
using Microsoft.EntityFrameworkCore;

namespace eAviaSales.Data;

public static class AviaSalesDbContextSeed
{
    public static void Seed(AviaSalesDbContext db)
    {
        if (db.Airports.Any())
        {
            return;
        }

        var airports = new[]
        {
            new Airport { Id = 1, IataCode = "WAW", Name = "Frederic Chopin Airport", City = "Warsaw", Country = "Poland" },
            new Airport { Id = 2, IataCode = "FCO", Name = "Leonardo da Vinci Airport", City = "Rome", Country = "Italy" },
            new Airport { Id = 3, IataCode = "IST", Name = "Istanbul Airport", City = "Istanbul", Country = "Turkey" },
            new Airport { Id = 4, IataCode = "DXB", Name = "Dubai International Airport", City = "Dubai", Country = "UAE" }
        };

        var airlines = new[]
        {
            new Airline { Id = 1, Code = "ES", Name = "EuroSky" },
            new Airline { Id = 2, Code = "AS", Name = "AeroSprint" }
        };

        var referenceDate = DateTime.UtcNow.Date.AddDays(7);

        var flights = new[]
        {
            new Flight
            {
                Id = 1,
                FlightNumber = "ES-101",
                AirlineId = 1,
                BasePrice = 179.99m,
                CurrencyCode = "EUR",
                SeatsAvailable = 8,
                Status = FlightStatus.Scheduled,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            },
            new Flight
            {
                Id = 2,
                FlightNumber = "AS-225",
                AirlineId = 2,
                BasePrice = 249.50m,
                CurrencyCode = "USD",
                SeatsAvailable = 14,
                Status = FlightStatus.Scheduled,
                CreatedAtUtc = DateTime.UtcNow,
                UpdatedAtUtc = DateTime.UtcNow
            }
        };

        var segments = new[]
        {
            new FlightSegment
            {
                Id = 1,
                FlightId = 1,
                DepartureAirportId = 1,
                ArrivalAirportId = 2,
                DepartureAtUtc = referenceDate.AddHours(7),
                ArrivalAtUtc = referenceDate.AddHours(10),
                SegmentOrder = 0
            },
            new FlightSegment
            {
                Id = 2,
                FlightId = 2,
                DepartureAirportId = 3,
                ArrivalAirportId = 4,
                DepartureAtUtc = referenceDate.AddHours(12),
                ArrivalAtUtc = referenceDate.AddHours(16),
                SegmentOrder = 0
            }
        };

        var fareRules = new[]
        {
            new FareRule
            {
                Id = 1,
                FareTier = FareTier.Economy,
                CheckedBagsIncluded = 1,
                CarryOnWeightKg = 8,
                CheckedBagWeightKg = 23,
                PriceMultiplier = 1.0m,
                Summary = "Economy: 1 checked bag up to 23 kg, carry-on up to 8 kg."
            },
            new FareRule
            {
                Id = 2,
                FareTier = FareTier.Business,
                CheckedBagsIncluded = 2,
                CarryOnWeightKg = 12,
                CheckedBagWeightKg = 32,
                PriceMultiplier = 2.2m,
                Summary = "Business: 2 checked bags up to 32 kg each, carry-on up to 12 kg."
            }
        };

        db.Airports.AddRange(airports);
        db.Airlines.AddRange(airlines);
        db.Flights.AddRange(flights);
        db.FlightSegments.AddRange(segments);
        db.FareRules.AddRange(fareRules);
        db.SaveChanges();
    }
}
