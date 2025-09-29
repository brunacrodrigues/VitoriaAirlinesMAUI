using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces;

/// <summary>
/// Defines the contract for HTTP API operations, providing methods for common REST operations
/// including GET, POST, PUT requests with JSON and multipart form data support.
/// </summary>
public interface IApiService
{
    /// <summary>
    /// Sends an HTTP GET request to the specified URI and returns a deserialized response.
    /// </summary>
    /// <typeparam name="T">The type of the expected response data.</typeparam>
    /// <param name="uri">The URI to send the GET request to.</param>
    /// <returns>An ApiResponse containing the data or error message.</returns>
    Task<ApiResponse<T?>> GetAsync<T>(string uri);



    /// <summary>
    /// Sends an HTTP POST request with JSON content and returns a deserialized response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body to send.</typeparam>
    /// <typeparam name="TResult">The type of the expected response data.</typeparam>
    /// <param name="uri">The URI to send the POST request to.</param>
    /// <param name="body">The request body object to be serialized as JSON.</param>
    /// <returns>An ApiResponse containing the result or error message.</returns>
    Task<ApiResponse<TResult?>> PostAsync<TRequest, TResult>(string uri, TRequest body);



    /// <summary>
    /// Sends an HTTP PUT request with JSON content and returns a deserialized response.
    /// </summary>
    /// <typeparam name="TRequest">The type of the request body to send.</typeparam>
    /// <typeparam name="TResult">The type of the expected response data.</typeparam>
    /// <param name="uri">The URI to send the PUT request to.</param>
    /// <param name="body">The request body object to be serialized as JSON.</param>
    /// <returns>An ApiResponse containing the result or error message.</returns>
    Task<ApiResponse<TResult?>> PutAsync<TRequest, TResult>(string uri, TRequest body);



    /// <summary>
    /// Sends an HTTP PUT request with multipart/form-data content and returns a deserialized response.
    /// </summary>
    /// <typeparam name="TResult">The type of the expected response data.</typeparam>
    /// <param name="uri">The URI to send the PUT request to.</param>
    /// <param name="content">The multipart/form-data content to send, typically used for file uploads.</param>
    /// <returns>An ApiResponse containing the result or error message.</returns>
    Task<ApiResponse<TResult?>> PutMultipartAsync<TResult>(string uri, MultipartFormDataContent content);



    /// <summary>
    /// Sends an HTTP GET request to retrieve a stream of data from the specified endpoint.
    /// </summary>
    /// <param name="endpoint">The URI endpoint to retrieve the stream from.</param>
    /// <returns>
    /// An ApiResponse containing a Stream if successful, or an error message if the request fails.
    /// The caller is responsible for disposing of the returned stream.
    /// </returns>
    Task<ApiResponse<Stream?>> GetStreamAsync(string endpoint);
}