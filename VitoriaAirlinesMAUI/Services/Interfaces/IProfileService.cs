using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces
{
    /// <summary>
    /// Interface for handling customer profile operations such as retrieving, updating profile, and changing password.
    /// </summary>
    public interface IProfileService
    {
        /// <summary>
        /// Retrieves the profile information of the currently authenticated customer.
        /// </summary>
        /// <returns>
        /// An ApiResponse containing the customer profile details or an error message.
        /// </returns>
        Task<ApiResponse<CustomerProfile?>> GetProfileAsync();



        /// <summary>
        /// Sends the updated customer profile data to the API.
        /// </summary>
        /// <param name="request">The request containing updated profile details.</param>
        /// <returns>
        /// An ApiResponse indicating the result of the update operation,
        /// including success or an error message.
        /// </returns>
        Task<ApiResponse<object?>> UpdateProfileAsync(UpdateCustomerProfileRequest request);



        /// <summary>
        /// Sends a request to change the customer's password.
        /// </summary>
        /// <param name="request">The request containing the old and new password values.</param>
        /// <returns>
        /// An ApiResponse indicating whether the password change was successful or not.
        /// </returns>
        Task<ApiResponse<object?>> ChangePasswordAsync(ChangePasswordRequest request);
    }
}
