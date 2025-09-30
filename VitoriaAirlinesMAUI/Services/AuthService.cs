using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    /// <summary>
    /// Service for handling account-related API operations such as login.
    /// Inherits common HTTP methods from ApiService.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IApiService _apiService;

        /// <summary>
        /// Initializes a new instance of the AuthService with the specified API service.
        /// </summary>
        /// <param name="apiService">The API service used to send HTTP requests to the backend.</param>
        public AuthService(IApiService apiService)
        {
            _apiService = apiService;
        }



        /// <summary>
        /// Sends a login request to the API and returns the authentication result.
        /// </summary>
        /// <param name="request">The login credentials (email and password).</param>
        /// <returns>An ApiResponse containing the token and expiration info, or an error message.</returns>
        public async Task<ApiResponse<LoginResponse?>> LoginAsync(LoginRequest request)
        {
            return await _apiService.PostAsync<LoginRequest, LoginResponse>("api/auth/login", request);
        }



        /// <summary>
        /// Sends a password recovery request to the API.
        /// </summary>
        /// <param name="request">The email to send recovery instructions to.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse<object?>> ForgotPasswordAsync(RecoverPasswordRequest request)
        {
            return await _apiService.PostAsync<RecoverPasswordRequest, object>("api/auth/forgotpassword", request);
        }



        /// <summary>
        /// Sends a reset password request to the API using a token and new password.
        /// </summary>
        /// <param name="request">The reset password data including token, email, and new password.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse<object?>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            return await _apiService.PostAsync<ResetPasswordRequest, object>("api/auth/resetpassword", request);
        }



        /// <summary>
        /// Clears the stored authentication token and user credentials from device preferences,
        /// effectively logging out the current user.
        /// </summary>
        public void Logout()
        {
            Preferences.Remove("Token");
            Preferences.Remove("UserRole");
            Preferences.Remove("SavedEmail");
            Preferences.Remove("SavedPassword");
        }



        /// <summary>
        /// Sends a new user registration request to the API.
        /// </summary>
        /// <param name="request">The registration data.</param>
        /// <returns>An ApiResponse indicating success or failure of the registration.</returns>
        public async Task<ApiResponse<object?>> RegisterAsync(RegisterNewUserRequest request)
        {
            return await _apiService.PostAsync<RegisterNewUserRequest, object>("api/auth/register", request);
        }
    }
}
