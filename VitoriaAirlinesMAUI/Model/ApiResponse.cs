namespace VitoriaAirlinesMAUI.Model
{
    /// <summary>
    /// Generic wrapper for handling API responses consistently.
    /// </summary>
    /// <typeparam name="T">The type of data returned from the API.</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates whether the API call was successful.
        /// </summary>
        public bool IsSuccess { get; set; }


        /// <summary>
        /// Optional message from the API, usually used for errors or status details.
        /// </summary>
        public string? Message { get; set; }


        /// <summary>
        /// The data returned from the API, if successful.
        /// </summary>
        public T? Data { get; set; }


        /// <summary>
        /// Creates a successful response with optional message.
        /// </summary>
        /// <param name="data">The data returned from the API.</param>
        /// <param name="message">An optional success message.</param>
        /// <returns>An instance of <see cref="ApiResponse{T}"/> representing a successful result.</returns>
        public static ApiResponse<T> Success(T? data, string? message = null)
            => new ApiResponse<T> { IsSuccess = true, Data = data, Message = message };


        /// <summary>
        /// Creates a failed response with an error message.
        /// </summary>
        /// <param name="message">The error message to include.</param>
        /// <returns>An instance of <see cref="ApiResponse{T}"/> representing a failure result.</returns>
        public static ApiResponse<T> Fail(string message)
            => new ApiResponse<T> { IsSuccess = false, Message = message };
    }
}
