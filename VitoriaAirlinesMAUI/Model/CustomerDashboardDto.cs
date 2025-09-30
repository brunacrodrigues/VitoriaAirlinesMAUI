namespace VitoriaAirlinesMAUI.Model
{
    public class CustomerDashboardDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string? ProfilePictureUrl { get; set; }
        public int UpcomingFlightsCount { get; set; }
        public int CompletedFlightsCount { get; set; }
        public decimal TotalSpent { get; set; }
        public NextFlightDto? NextUpcomingFlight { get; set; }
    }
}
