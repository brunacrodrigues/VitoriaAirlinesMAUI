namespace VitoriaAirlinesMAUI.Model
{
    public class FlightSearchRequest
    {
        public int OriginAirportId { get; set; }
        public int DestinationAirportId { get; set; }
        public DateTime DepartureDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public bool IsRoundTrip { get; set; } = false;
        public int NumberOfPassengers { get; set; } = 1;
    }
}
