using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    public class FlightService : IFlightService
    {
        private const string FlightsSearchEndpoint = "api/flights/search";
        private const string AirportsEndpoint = "api/airports/combo";

        private readonly IApiService _api;

        public FlightService(IApiService api)
        {
            _api = api;
        }


        public async Task<ApiResponse<List<AirportOption>?>> GetAirportsAsync()
        {
            return await _api.GetAsync<List<AirportOption>?>(AirportsEndpoint);
        }


        public async Task<ApiResponse<FlightSearchResponse?>> SearchFlightsAsync(FlightSearchRequest request)
        {
            return await _api.PostAsync<FlightSearchRequest, FlightSearchResponse>(FlightsSearchEndpoint, request);
        }
    }
}
