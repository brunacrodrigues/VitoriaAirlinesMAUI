namespace VitoriaAirlinesMAUI.Model
{
    public class FlightSearchResponse
    {
        public List<FlightSearchResult>? OutboundFlights { get; set; }
        public List<FlightSearchResult>? ReturnFlights { get; set; }
    }
}
