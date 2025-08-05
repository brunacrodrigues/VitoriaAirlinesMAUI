using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces
{
    /// <summary>
    /// Interface for retrieving the list of countries from the API.
    /// </summary>
    public interface ICountryService
    {
        /// <summary>
        /// Retrieves all countries available for selection, including flags and codes.
        /// </summary>
        /// <returns>
        /// An ApiResponse containing a list of countries or an error message.
        /// </returns>
        Task<ApiResponse<List<Country>?>> GetCountriesAsync();
    }
}
