using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    public class FlightService : IFlightService
    {
        private const string FlightsSearchEndpoint = "api/flights/search";
        private const string AirportsEndpoint = "api/airports/combo";
        private const string FlightSeatsEndpoint = "api/flights";

        private readonly IApiService _api;

        public FlightService(IApiService api)
        {
            _api = api;
        }


        /// <summary>
        /// Retrieves a list of airport options from the API.
        /// </summary>
        /// <returns>An ApiResponse containing a list of AirportOption or null if not found/error.</returns>
        public async Task<ApiResponse<List<AirportOption>?>> GetAirportsAsync()
        {
            return await _api.GetAsync<List<AirportOption>?>(AirportsEndpoint);
        }



        /// <summary>
        /// Searches for flights based on the provided criteria.
        /// </summary>
        /// <param name="request">The flight search request details.</param>
        /// <returns>An ApiResponse containing FlightSearchResponse or null if not found/error.</returns>
        public async Task<ApiResponse<FlightSearchResponse?>> SearchFlightsAsync(FlightSearchRequest request)
        {
            return await _api.PostAsync<FlightSearchRequest, FlightSearchResponse>(FlightsSearchEndpoint, request);
        }


        /// <summary>
        /// Retrieves flight seat details for a given flight ID from the API.
        /// </summary>
        /// <param name="flightId">The ID of the flight to get seat details for.</param>
        /// <returns>An ApiResponse containing FlightSeatsResponse or null if not found/error.</returns>
        public async Task<ApiResponse<FlightSeatsResponse?>> GetFlightSeatsAsync(int flightId)
        {
            return await _api.GetAsync<FlightSeatsResponse?>($"{FlightSeatsEndpoint}/{flightId}/seats");
        }
    }
}
