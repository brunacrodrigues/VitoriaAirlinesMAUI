namespace VitoriaAirlinesMAUI.Model
{
    public class FlightSeatsResponse
    {
        public int FlightId { get; set; }
        public string FlightNumber { get; set; } = null!;
        public string OriginAirportIATA { get; set; } = null!;
        public string DestinationAirportIATA { get; set; } = null!;
        public string OriginAirportName { get; set; } = null!;
        public string DestinationAirportName { get; set; } = null!;
        public string OriginCountryCode { get; set; } = null!;
        public string DestinationCountryCode { get; set; } = null!;
        public DateTime DepartureUtc { get; set; }
        public DateTime ArrivalUtc { get; set; }
        public TimeSpan Duration { get; set; }
        public decimal EconomyClassPrice { get; set; }
        public decimal ExecutiveClassPrice { get; set; }
        public IEnumerable<SeatDetail> Seats { get; set; } = new List<SeatDetail>();
    }
}
