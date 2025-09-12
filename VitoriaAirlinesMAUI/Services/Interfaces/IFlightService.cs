using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces
{
    public interface IFlightService
    {
        Task<ApiResponse<List<AirportOption>?>> GetAirportsAsync();
        Task<ApiResponse<FlightSearchResponse?>> SearchFlightsAsync(FlightSearchRequest request);

    }
}
