namespace VitoriaAirlinesMAUI.Model
{
    public class BookingRequest
    {
        public List<BookingLeg> Legs { get; set; } = new List<BookingLeg>();
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PassportNumber { get; set; }
    }
}
