using eAviaSales.Domains.Models.Flight;

namespace eAviaSales.BusinessLogic.Interface;

public interface IFlightActions
{
    Task<List<FlightTicketDto>> SearchFlightsActionAsync(FlightSearchRequest request);
    Task<FlightTicketDto?> GetFlightByIdActionAsync(int id);
}
