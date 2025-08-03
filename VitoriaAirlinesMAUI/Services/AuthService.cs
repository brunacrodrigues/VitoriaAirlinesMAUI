using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    /// <summary>
    /// Service for handling account-related API operations such as login.
    /// Inherits common HTTP methods from ApiService.
    /// </summary>
    public class AuthService : ApiService, IAuthService
    {
        /// <summary>
        /// Initializes a new instance of the AccountService with the specified HttpClient.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for sending requests.</param>
        public AuthService(HttpClient httpClient) : base(httpClient) { }



        /// <summary>
        /// Sends a login request to the API and returns the authentication result.
        /// </summary>
        /// <param name="request">The login credentials (email and password).</param>
        /// <returns>An ApiResponse containing the token and expiration info, or an error message.</returns>
        public async Task<ApiResponse<LoginResponse?>> LoginAsync(LoginRequest request)
        {
            return await PostAsync<LoginRequest, LoginResponse>("api/auth/login", request);
        }


        /// <summary>
        /// Sends a password recovery request to the API.
        /// </summary>
        /// <param name="request">The email to send recovery instructions to.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse<object?>> ForgotPasswordAsync(RecoverPasswordRequest request)
        {
            return await PostAsync<RecoverPasswordRequest, object>("api/auth/forgotpassword", request);
        }



        /// <summary>
        /// Sends a reset password request to the API using a token and new password.
        /// </summary>
        /// <param name="request">The reset password data including token, email, and new password.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse<object?>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return await PostAsync<ResetPasswordRequest, object>("api/auth/resetpassword", request);
        }
    }
}
