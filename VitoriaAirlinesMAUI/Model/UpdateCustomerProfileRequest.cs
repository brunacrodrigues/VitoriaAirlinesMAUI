using System.ComponentModel.DataAnnotations;

namespace VitoriaAirlinesMAUI.Model;

/// <summary>
/// Represents the data sent to the API to update the customer's profile.
/// Used for editing name, nationality, passport, and uploading a new profile image.
/// </summary>
public class UpdateCustomerProfileRequest
{
    /// <summary>
    /// Gets or sets the customer's first name.
    /// This field is required and cannot exceed 100 characters.
    /// </summary>
    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(100, ErrorMessage = "First name cannot exceed 100 characters.")]
    public string FirstName { get; set; } = null!;



    /// <summary>
    /// Gets or sets the customer's last name.
    /// This field is required and cannot exceed 100 characters.
    /// </summary>
    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(100, ErrorMessage = "Last name cannot exceed 100 characters.")]
    public string LastName { get; set; } = null!;



    /// <summary>
    /// Gets or sets the ID of the selected country (nationality).
    /// This field is optional and may be null if not selected.
    /// </summary>
    public int? CountryId { get; set; }



    /// <summary>
    /// Gets or sets the customer's passport number.
    /// This field is optional but cannot exceed 20 characters.
    /// </summary>
    [MaxLength(20, ErrorMessage = "Passport number cannot exceed 20 characters.")]
    public string? PassportNumber { get; set; }



    /// <summary>
    /// Gets or sets a value indicating whether the current profile image should be removed.
    /// </summary>
    public bool RemoveImage { get; set; }



    /// <summary>
    /// Gets or sets the stream of the new profile image file selected by the user.
    /// This is used to send the binary image data to the API.
    /// </summary>
    public Stream? ProfileImageStream { get; set; }



    /// <summary>
    /// Gets or sets the file name of the uploaded profile image.
    /// This is used to preserve the original image name in the upload request.
    /// </summary>
    public string? ProfileImageFileName { get; set; }
}
