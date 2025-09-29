namespace VitoriaAirlinesMAUI.Model
{
    public class CreateCheckoutResponse
    {
        public string CheckoutUrl { get; set; } = string.Empty;
        public string StripeSessionId { get; set; } = string.Empty;
    }
}
