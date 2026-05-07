using eAviaSales.BusinessLogic.Core.Flights;
using eAviaSales.BusinessLogic.Interface;
using eAviaSales.Data;
using eAviaSales.Domains.Models.Flight;

namespace eAviaSales.BusinessLogic.Functions.Flights;

public class FlightFlow : FlightAction, IFlightActions
{
    public FlightFlow(AviaSalesDbContext db)
        : base(db)
    {
    }

    public Task<List<FlightTicketDto>> SearchFlightsActionAsync(FlightSearchRequest request)
    {
        return ExecuteSearchFlightsActionAsync(request);
    }

    public Task<FlightTicketDto?> GetFlightByIdActionAsync(int id)
    {
        return ExecuteGetFlightByIdActionAsync(id);
    }
}
