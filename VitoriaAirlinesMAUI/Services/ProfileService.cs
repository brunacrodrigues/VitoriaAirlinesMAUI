using System.Net.Http.Headers;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    /// <summary>
    /// Service for handling customer profile operations such as retrieving, updating profile, and changing password.
    /// Inherits common HTTP methods from ApiService.
    /// </summary>
    public class ProfileService : IProfileService
    {
        private readonly IApiService _apiService;

        /// <summary>
        /// Initializes a new instance of the ProfileService with the specified API service.
        /// </summary>
        /// <param name="apiService">The API service used to send HTTP requests to the backend.</param>
        public ProfileService(IApiService apiService)
        {
            _apiService = apiService;
        }



        /// <summary>
        /// Retrieves the current customer's profile from the API.
        /// </summary>
        /// <returns>An ApiResponse containing the customer's profile data or an error message.</returns>
        public async Task<ApiResponse<CustomerProfile?>> GetProfileAsync()
        {
            return await _apiService.GetAsync<CustomerProfile>("api/profile");
        }


        public async Task<ApiResponse<object?>> UpdateProfileAsync(UpdateCustomerProfileRequest request)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(request.FirstName), "FirstName");
            content.Add(new StringContent(request.LastName), "LastName");

            if (request.CountryId.HasValue)
                content.Add(new StringContent(request.CountryId.Value.ToString()), "CountryId");

            if (!string.IsNullOrWhiteSpace(request.PassportNumber))
                content.Add(new StringContent(request.PassportNumber), "PassportNumber");

            content.Add(new StringContent(request.RemoveImage.ToString()), "RemoveImage");

            if (request.ProfileImageStream != null && !string.IsNullOrWhiteSpace(request.ProfileImageFileName))
            {
                request.ProfileImageStream.Position = 0;
                var imageContent = new StreamContent(request.ProfileImageStream);
                imageContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                content.Add(imageContent, "ProfileImage", request.ProfileImageFileName);
            }

            return await _apiService.PutMultipartAsync<object?>("api/profile", content);
        }



        /// <summary>
        /// Sends a request to change the customer's password.
        /// </summary>
        /// <param name="request">The password change data including old and new password.</param>
        /// <returns>An ApiResponse indicating success or failure.</returns>
        public async Task<ApiResponse<object?>> ChangePasswordAsync(ChangePasswordRequest request)
        {
            return await _apiService.PutAsync<ChangePasswordRequest, object>("api/profile/change-password", request);
        }
    }
}
