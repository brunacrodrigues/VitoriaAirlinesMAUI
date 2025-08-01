namespace VitoriaAirlinesMAUI.Model
{
    /// <summary>
    /// Represents the profile information of an authenticated user.
    /// </summary>
    public class User
    {
        /// <summary>
        /// Full name of the user.
        /// </summary>
        public string FullName { get; set; } = null!;


        /// <summary>
        /// Email address of the user.
        /// </summary>
        public string Email { get; set; } = null!;
    }
}
