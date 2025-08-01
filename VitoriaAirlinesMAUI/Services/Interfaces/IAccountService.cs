using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces
{
    public interface IAccountService
    {
        /// <summary>
        /// Attempts to log in with the given credentials.
        /// </summary>
        /// <param name="request">The login request containing email and password.</param>
        /// <returns>An ApiResponse containing the authentication token or an error message.</returns>
        Task<ApiResponse<LoginResponse?>> LoginAsync(LoginRequest request);
    }
}
