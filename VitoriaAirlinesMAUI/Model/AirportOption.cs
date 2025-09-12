namespace VitoriaAirlinesMAUI.Model
{
    public class AirportOption
    {
        //public int Id { get; set; }
        //public string IATA { get; set; } = string.Empty;
        //public string Name { get; set; } = string.Empty;
        //public string CountryCode { get; set; } = "PT";
        //public string Display => $"({IATA}) {Name}";
        public int Id { get; set; }
        public string IATA { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string City { get; set; } = null!;
        public string CountryCode { get; set; } = null!; // Used for displaying flags
        public string FullName => $"{IATA} - {Name} ({City})"; // For display in Picker
    }
}
