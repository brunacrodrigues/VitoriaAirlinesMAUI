namespace VitoriaAirlinesMAUI.Model
{
    /// <summary>
    /// Represents the credentials sent to the API for user authentication.
    /// </summary>
    public class LoginRequest
    {
        /// <summary>
        /// The email address used for login.
        /// </summary>
        public string Username { get; set; } = null!;


        /// <summary>
        /// The password associated with the email account.
        /// </summary>
        public string Password { get; set; } = null!;
    }
}
