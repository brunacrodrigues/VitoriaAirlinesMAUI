namespace VitoriaAirlinesMAUI.Model;

public class Ticket
{
    public int TicketId { get; set; }
    public int FlightId { get; set; }
    public string FlightNumber { get; set; } = string.Empty;

    public DateTime DepartureUtc { get; set; }
    public DateTime ArrivalUtc { get; set; }

    public string OriginAirport { get; set; } = string.Empty;
    public string DestinationAirport { get; set; } = string.Empty;


    public string OriginCountryCode { get; set; } = string.Empty;
    public string DestinationCountryCode { get; set; } = string.Empty;


    public string Seat { get; set; } = string.Empty;
    public decimal PricePaid { get; set; }
    public DateTime PurchaseDateUtc { get; set; }

    public string Status { get; set; } = "Scheduled";


    public string DepartureLocal => DepartureUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");
    public string ArrivalLocal => ArrivalUtc == default
        ? string.Empty
        : ArrivalUtc.ToLocalTime().ToString("dd/MM/yyyy HH:mm");

    public string PriceFormatted => $"€ {PricePaid:0.00}";
    public string Route => $"{OriginAirport} → {DestinationAirport}";
}
