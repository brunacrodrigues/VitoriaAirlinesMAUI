namespace VitoriaAirlinesMAUI.Model
{
    public class BoardingPass
    {
        public int TicketId { get; set; }
        public string PassengerName { get; set; } = string.Empty;
        public string FlightNumber { get; set; } = string.Empty;
        public string FromAirportIATA { get; set; } = string.Empty;
        public string FromAirportFullName { get; set; } = string.Empty;
        public string ToAirportIATA { get; set; } = string.Empty;
        public string ToAirportFullName { get; set; } = string.Empty;
        public DateTime DepartureUtc { get; set; }
        public DateTime ArrivalUtc { get; set; }
        public string Gate { get; set; } = "N17";
        public string BoardingZone { get; set; } = "A";
        public string SeatNumber { get; set; } = string.Empty;
        public string SeatClass { get; set; } = string.Empty;
        public string TicketBarcodeValue { get; set; } = string.Empty;
    }
}
