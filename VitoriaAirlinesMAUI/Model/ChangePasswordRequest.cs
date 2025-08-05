using System.ComponentModel.DataAnnotations;

namespace VitoriaAirlinesMAUI.Model;

/// <summary>
/// Represents the data sent to the API to change the customer's password.
/// Contains fields for the current password, new password, and confirmation of the new password.
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// Gets or sets the current password of the user.
    /// This field is required and must match the user's existing password.
    /// </summary>
    [Required(ErrorMessage = "Current password is required.")]
    public string OldPassword { get; set; } = null!;



    /// <summary>
    /// Gets or sets the new password that the user wants to set.
    /// This field is required.
    /// </summary>
    [Required(ErrorMessage = "New password is required.")]
    public string NewPassword { get; set; } = null!;



    /// <summary>
    /// Gets or sets the confirmation of the new password.
    /// This field is required and must match the NewPassword field.
    /// </summary>
    [Required(ErrorMessage = "Confirm password is required.")]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = null!;
}
