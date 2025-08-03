using System.ComponentModel.DataAnnotations;

namespace VitoriaAirlinesMAUI.Model
{
    /// <summary>
    /// Represents the request data required to reset a user's password.
    /// </summary>
    public class ResetPasswordRequest
    {
        /// <summary>
        /// Gets or sets the email address of the user requesting the password reset.
        /// </summary>

        [Required(ErrorMessage = "Email is required.")]
        public string Username { get; set; } = null!;



        /// <summary>
        /// Gets or sets the new password for the user's account.
        /// </summary>
        [Required(ErrorMessage = "New password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;



        /// <summary>
        /// Gets or sets the confirmation of the new password.
        /// Must match the <see cref="Password"/> property.
        /// </summary>
        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = null!;



        /// <summary>
        /// Gets or sets the password reset token sent to the user.
        /// </summary>
        [Required(ErrorMessage = "Token is required.")]
        public string Token { get; set; } = null!;
    }
}
