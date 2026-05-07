using eAviaSales.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace eAviaSales.Data;

public class AviaSalesDbContext : DbContext
{
    public AviaSalesDbContext(DbContextOptions<AviaSalesDbContext> options)
        : base(options)
    {
    }

    public DbSet<Airport> Airports => Set<Airport>();
    public DbSet<Airline> Airlines => Set<Airline>();
    public DbSet<Flight> Flights => Set<Flight>();
    public DbSet<FlightSegment> FlightSegments => Set<FlightSegment>();
    public DbSet<FareRule> FareRules => Set<FareRule>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookingPassenger> BookingPassengers => Set<BookingPassenger>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Airport>(e =>
        {
            e.HasIndex(a => a.IataCode).IsUnique();
        });

        modelBuilder.Entity<Airline>(e =>
        {
            e.HasIndex(a => a.Code).IsUnique();
        });

        modelBuilder.Entity<Flight>(e =>
        {
            e.HasIndex(f => f.FlightNumber).IsUnique();
            e.HasOne(f => f.Airline)
                .WithMany(a => a.Flights)
                .HasForeignKey(f => f.AirlineId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FlightSegment>(e =>
        {
            e.HasOne(s => s.Flight)
                .WithMany(f => f.Segments)
                .HasForeignKey(s => s.FlightId)
                .OnDelete(DeleteBehavior.Cascade);

            e.HasOne(s => s.DepartureAirport)
                .WithMany()
                .HasForeignKey(s => s.DepartureAirportId)
                .OnDelete(DeleteBehavior.Restrict);

            e.HasOne(s => s.ArrivalAirport)
                .WithMany()
                .HasForeignKey(s => s.ArrivalAirportId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<FareRule>(e =>
        {
            e.HasIndex(r => r.FareTier).IsUnique();
        });

        modelBuilder.Entity<Booking>(e =>
        {
            e.HasOne(b => b.Flight)
                .WithMany(f => f.Bookings)
                .HasForeignKey(b => b.FlightId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<BookingPassenger>(e =>
        {
            e.HasOne(p => p.Booking)
                .WithMany(b => b.Passengers)
                .HasForeignKey(p => p.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
