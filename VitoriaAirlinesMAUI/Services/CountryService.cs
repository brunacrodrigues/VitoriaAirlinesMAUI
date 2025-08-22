using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    /// <summary>
    /// Service for retrieving countries from the API.
    /// </summary>
    public class CountryService : ICountryService
    {
        private readonly IApiService _apiService;


        /// <summary>
        /// Initializes a new instance of the CountryServuce with the specified API service.
        /// </summary>
        /// <param name="apiService">The API service used to send HTTP requests to the backend.</param>
        public CountryService(IApiService apiService)
        {
            _apiService = apiService;
        }


        /// <summary>
        /// Sends a GET request to the API to retrieve the list of countries.
        /// </summary>
        /// <returns>An ApiResponse containing the list of countries or an error message.</returns>
        public async Task<ApiResponse<List<Country>?>> GetCountriesAsync()
        {
            return await _apiService.GetAsync<List<Country>>("api/countries");
        }
    }
}
