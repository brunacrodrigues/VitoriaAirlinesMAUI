using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces
{
    public interface IAuthService
    {
        /// <summary>
        /// Attempts to log in with the given credentials.
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        /// <returns>An ApiResponse containing the authentication token or an error message.</returns>
        Task<ApiResponse<LoginResponse?>> LoginAsync(LoginRequest request);


        /// <summary>
        /// Sends a password recovery email to the user.
        /// </summary>
        /// <param name="request">The request containing the user's email address.</param>
        /// <returns>
        /// An ApiResponse indicating the result of the password recovery attempt,
        /// including success or an error message.
        /// </returns>
        Task<ApiResponse<object?>> ForgotPasswordAsync(RecoverPasswordRequest request);



        /// <summary>
        /// Resets the user's password using a token received by email.
        /// </summary>
        /// <param name="request">The request containing email, token, and the new password.</param>
        /// <returns>
        /// An ApiResponse indicating the result of the password reset operation,
        /// including success or an error message.
        /// </returns>
        Task<ApiResponse<object?>> ResetPasswordAsync(ResetPasswordRequest request);



        /// <summary>
        /// Logs out the current user by clearing stored authentication data,
        /// such as the JWT token and saved credentials.
        /// </summary>
        void Logout();
    }
}
