using System.Net.Http.Json;
using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services
{
    /// <summary>
    /// Base class for API service classes, providing common HTTP operations.
    /// </summary>
    public abstract class ApiService
    {
        /// <summary>
        /// The HttpClient instance used for sending HTTP requests.
        /// </summary>
        protected readonly HttpClient _httpClient;


        /// <summary>
        /// The HttpClient instance used for sending HTTP requests.
        /// </summary>
        protected ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        /// <summary>
        /// Sends an HTTP GET request to the specified URI and returns a deserialized ApiResponse.
        /// </summary>
        /// <typeparam name="T">The type of the expected response data.</typeparam>
        /// <param name="uri">The URI to send the GET request to.</param>
        /// <returns>An ApiResponse containing the data or error message.</returns>
        protected async Task<ApiResponse<T?>> GetAsync<T>(string uri)
        {
            try
            {
                var response = await _httpClient.GetAsync(uri);
                if (!response.IsSuccessStatusCode)
                    return ApiResponse<T?>.Fail(await response.Content.ReadAsStringAsync());


                var data = await response.Content.ReadFromJsonAsync<T>();
                return ApiResponse<T?>.Success(data);

            }
            catch (Exception ex)
            {
                return ApiResponse<T?>.Fail(ex.Message);
            }
        }


        /// <summary>
        /// Sends an HTTP POST request with the specified content and returns a deserialized ApiResponse.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request content.</typeparam>
        /// <typeparam name="TResult">The type of the expected response data.</typeparam>
        /// <param name="uri">The URI to send the POST request to.</param>
        /// <param name="content">The content to include in the POST request.</param>
        /// <returns>An ApiResponse containing the result or error message.</returns>
        protected async Task<ApiResponse<TResult?>> PostAsync<TRequest, TResult>(string uri, TRequest content)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(uri, content);
                if (!response.IsSuccessStatusCode)
                    return ApiResponse<TResult?>.Fail(await response.Content.ReadAsStringAsync());


                var data = await response.Content.ReadFromJsonAsync<TResult>();
                return ApiResponse<TResult?>.Success(data);

            }
            catch (Exception ex)
            {
                return ApiResponse<TResult?>.Fail(ex.Message);
            }
        }


        /// <summary>
        /// Sends an HTTP PUT request with the specified content and returns a deserialized ApiResponse.
        /// </summary>
        /// <typeparam name="TRequest">The type of the request body to send.</typeparam>
        /// <typeparam name="TResult">The type of the expected response data.</typeparam>
        /// <param name="uri">The URI to send the PUT request to.</param>
        /// <param name="content">The content object to include in the request body.</param>
        /// <returns>An ApiResponse containing the result or an error message.</returns>
        protected async Task<ApiResponse<TResult?>> PutAsync<TRequest, TResult>(string uri, TRequest content)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync(uri, content);
                if (!response.IsSuccessStatusCode)
                    return ApiResponse<TResult?>.Fail(await response.Content.ReadAsStringAsync());


                var data = await response.Content.ReadFromJsonAsync<TResult>();
                return ApiResponse<TResult?>.Success(data);

            }
            catch (Exception ex)
            {
                return ApiResponse<TResult?>.Fail(ex.Message);
            }
        }

    }
}
