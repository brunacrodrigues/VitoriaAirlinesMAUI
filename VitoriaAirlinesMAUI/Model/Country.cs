namespace VitoriaAirlinesMAUI.Model
{
    /// <summary>
    /// Represents a selectable country used in dropdowns or forms within the application.
    /// Includes name, ISO code, and flag image URL.
    /// </summary>
    public class Country
    {
        /// <summary>
        /// Gets or sets the unique identifier of the country.
        /// </summary>
        public int Id { get; set; }


        /// <summary>
        /// Gets or sets the name of the country (e.g., "Portugal").
        /// </summary>
        public string Name { get; set; } = null!;


        /// <summary>
        /// Gets or sets the two-letter ISO country code (e.g., "PT").
        /// </summary>
        public string CountryCode { get; set; } = null!;


        /// <summary>
        /// Gets or sets the full URL of the country flag image.
        /// </summary>
        public string FlagImageUrl { get; set; } = null!;
    }
}
