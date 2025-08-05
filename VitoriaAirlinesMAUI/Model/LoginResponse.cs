namespace VitoriaAirlinesMAUI.Model
{
    /// <summary>
    /// Represents the response received from the API upon successful 
    /// .
    /// </summary>
    public class LoginResponse
    {
        /// <summary>
        /// JWT token issued for the authenticated user.
        /// </summary>
        public string Token { get; set; } = null!;


        /// <summary>
        /// Expiration time of the issued token.
        /// </summary>
        public DateTime Expiration { get; set; }


        /// <summary>
        /// Role of the authenticated user.
        /// </summary>
        public string? Role { get; set; }
    }
}
