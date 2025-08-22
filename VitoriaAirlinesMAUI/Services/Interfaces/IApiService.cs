using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces;

public interface IApiService
{
    Task<ApiResponse<T?>> GetAsync<T>(string uri);
    Task<ApiResponse<TResult?>> PostAsync<TRequest, TResult>(string uri, TRequest body);
    Task<ApiResponse<TResult?>> PutAsync<TRequest, TResult>(string uri, TRequest body);
    Task<ApiResponse<TResult?>> PutMultipartAsync<TResult>(string uri, MultipartFormDataContent content);

}
