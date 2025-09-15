namespace VitoriaAirlinesMAUI.Model
{
    public class SeatDetail
    {
        public int Id { get; set; }
        public int Row { get; set; }
        public string Letter { get; set; } = null!;
        public SeatClass Class { get; set; }
        public bool IsOccupied { get; set; }
    }
}
