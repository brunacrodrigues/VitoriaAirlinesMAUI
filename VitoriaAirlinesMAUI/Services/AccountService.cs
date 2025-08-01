using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    /// <summary>
    /// Service for handling account-related API operations such as login.
    /// Inherits common HTTP methods from ApiService.
    /// </summary>
    public class AccountService : ApiService, IAccountService
    {
        /// <summary>
        /// Initializes a new instance of the AccountService with the specified HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for sending requests.</param>
        public AccountService(HttpClient httpClient) : base(httpClient) { }



        /// <summary>
        /// Sends a login request to the API and returns the authentication result.
        /// </summary>
        /// <param name="request">The login credentials (email and password).</param>
        /// <returns>An ApiResponse containing the token and expiration info, or an error message.</returns>
        public async Task<ApiResponse<LoginResponse?>> LoginAsync(LoginRequest request)
        {
            return await PostAsync<LoginRequest, LoginResponse>("api/auth/login", request);
        }


    }
}
