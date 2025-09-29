namespace VitoriaAirlinesMAUI.Model
{
    /// <summary>
    /// Represents the customer profile data received from the API.
    /// Includes personal details such as name, country, profile image and passport number.
    /// </summary>
    public class CustomerProfile
    {
        /// <summary>
        /// Gets or sets the customer's first name.
        /// </summary>
        public string? FirstName { get; set; }


        /// <summary>
        /// Gets or sets the customer's last name.
        /// </summary>
        public string? LastName { get; set; }

        /// <summary>
        /// Gets or sets the full URL to the customer's profile image.
        /// </summary>
        public string? ProfileImageUrl { get; set; }


        /// <summary>
        /// Gets or sets the ID of the customer's nationality/country.
        /// </summary>
        public int? CountryId { get; set; }


        /// <summary>
        /// Gets or sets the customer's passport number.
        /// </summary>
        public string? PassportNumber { get; set; }


        /// <summary>
        /// Gets or sets the customer's email.
        /// </summary>
        public string Email { get; set; } = string.Empty;
    }
}
