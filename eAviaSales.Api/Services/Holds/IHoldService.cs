namespace eAviaSales.Api.Services.Holds;

public interface IHoldService
{
    CreateHoldResult Create(int eventId, IReadOnlyList<string> seatNumbers, int holdMinutes);
    bool Delete(int eventId, string holdId);
}
