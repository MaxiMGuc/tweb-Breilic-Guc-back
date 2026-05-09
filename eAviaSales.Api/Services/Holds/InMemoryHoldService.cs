namespace eAviaSales.Api.Services.Holds;

public sealed class InMemoryHoldService : IHoldService
{
    private readonly object _sync = new();
    private readonly Dictionary<string, HoldTicket> _holds = new(StringComparer.OrdinalIgnoreCase);
    private readonly Dictionary<string, string> _seatReservations = new(StringComparer.OrdinalIgnoreCase);

    public CreateHoldResult Create(int eventId, IReadOnlyList<string> seatNumbers, int holdMinutes)
    {
        var normalizedSeats = seatNumbers
            .Where(static seat => !string.IsNullOrWhiteSpace(seat))
            .Select(static seat => seat.Trim().ToUpperInvariant())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (normalizedSeats.Count == 0)
        {
            return new CreateHoldResult
            {
                Success = false
            };
        }

        lock (_sync)
        {
            RemoveExpiredUnsafe();

            foreach (var seat in normalizedSeats)
            {
                var key = GetSeatKey(eventId, seat);
                if (_seatReservations.ContainsKey(key))
                {
                    return new CreateHoldResult
                    {
                        Success = false,
                        ConflictSeat = seat
                    };
                }
            }

            var holdId = Guid.NewGuid().ToString("N");
            var hold = new HoldTicket
            {
                HoldId = holdId,
                EventId = eventId,
                SeatNumbers = normalizedSeats,
                ExpiresAtUtc = DateTime.UtcNow.AddMinutes(holdMinutes)
            };

            _holds[holdId] = hold;
            foreach (var seat in normalizedSeats)
            {
                _seatReservations[GetSeatKey(eventId, seat)] = holdId;
            }

            return new CreateHoldResult
            {
                Success = true,
                Hold = hold
            };
        }
    }

    public bool Delete(int eventId, string holdId)
    {
        lock (_sync)
        {
            RemoveExpiredUnsafe();

            if (!_holds.TryGetValue(holdId, out var hold) || hold.EventId != eventId)
            {
                return false;
            }

            _holds.Remove(holdId);
            foreach (var seat in hold.SeatNumbers)
            {
                _seatReservations.Remove(GetSeatKey(eventId, seat));
            }

            return true;
        }
    }

    private void RemoveExpiredUnsafe()
    {
        var now = DateTime.UtcNow;
        var expiredHoldIds = _holds.Values
            .Where(hold => hold.ExpiresAtUtc <= now)
            .Select(hold => hold.HoldId)
            .ToList();

        foreach (var holdId in expiredHoldIds)
        {
            if (_holds.Remove(holdId, out var removedHold))
            {
                foreach (var seat in removedHold.SeatNumbers)
                {
                    _seatReservations.Remove(GetSeatKey(removedHold.EventId, seat));
                }
            }
        }
    }

    private static string GetSeatKey(int eventId, string seatNumber)
    {
        return $"{eventId}:{seatNumber}";
    }
}
