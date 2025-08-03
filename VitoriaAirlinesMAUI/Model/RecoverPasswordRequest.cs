using System.ComponentModel.DataAnnotations;

namespace VitoriaAirlinesMAUI.Model
{
    /// <summary>
    /// Represents the request to initiate password recovery by email.
    /// </summary>
    public class RecoverPasswordRequest
    {

        /// <summary>
        /// The email address where the recovery instructions will be sent.
        /// </summary>
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; } = null!;
    }
}
