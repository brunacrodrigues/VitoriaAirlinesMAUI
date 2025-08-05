using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    /// <summary>
    /// Service for retrieving countries from the API.
    /// </summary>
    public class CountryService : ApiService, ICountryService
    {
        /// <summary>
        /// Initializes a new instance of the CountryService with the specified HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient used for sending requests.</param>
        public CountryService(HttpClient httpClient) : base(httpClient) { }


        /// <summary>
        /// Sends a GET request to the API to retrieve the list of countries.
        /// </summary>
        /// <returns>An ApiResponse containing the list of countries or an error message.</returns>
        public async Task<ApiResponse<List<Country>?>> GetCountriesAsync()
        {
            return await GetAsync<List<Country>>("api/countries");
        }
    }
}
