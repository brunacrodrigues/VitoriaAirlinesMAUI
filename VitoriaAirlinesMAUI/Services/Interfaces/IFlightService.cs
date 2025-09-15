using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces
{
    public interface IFlightService
    {
        Task<ApiResponse<List<AirportOption>?>> GetAirportsAsync();
        Task<ApiResponse<FlightSearchResponse?>> SearchFlightsAsync(FlightSearchRequest request);


        /// <summary>
        /// Retrieves flight seat details for a given flight ID from the API.
        /// </summary>
        /// <param name="flightId">The ID of the flight to get seat details for.</param>
        /// <returns>An ApiResponse containing FlightSeatsResponseDto or null if not found/error.</returns>
        Task<ApiResponse<FlightSeatsResponse?>> GetFlightSeatsAsync(int flightId);



    }
}
