using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.Services
{
    /// <summary>
    /// Service responsible for managing booking operations, including checkout sessions,
    /// booking completion, and boarding pass retrieval.
    /// </summary>
    public class BookingService : IBookingService
    {
        private readonly IApiService _apiService;

        public BookingService(IApiService apiService)
        {
            _apiService = apiService;
        }


        /// <summary>
        /// Creates a Stripe checkout session for a new booking.
        /// </summary>
        /// <param name="request">The booking request containing flight and passenger details.</param>
        /// <returns>
        /// An ApiResponse containing the checkout session information including the Stripe session URL,
        /// or an error message if the operation fails.
        /// </returns>
        public async Task<ApiResponse<CreateCheckoutResponse?>> CreateCheckoutSessionAsync(BookingRequest request)
        {
            return await _apiService.PostAsync<BookingRequest, CreateCheckoutResponse?>("api/booking/create-checkout-session", request);
        }



        /// <summary>
        /// Completes the booking process after successful payment confirmation.
        /// </summary>
        /// <param name="stripeSessionId">The Stripe session ID returned after successful payment.</param>
        /// <returns>
        /// An ApiResponse containing the completed booking details including ticket information,
        /// or an error message if the completion fails.
        /// </returns>
        public async Task<ApiResponse<CompleteBookingResponse?>> CompleteBookingAsync(string stripeSessionId)
        {
            var requestDto = new CompleteBookingRequest { StripeSessionId = stripeSessionId };
            return await _apiService.PostAsync<CompleteBookingRequest, CompleteBookingResponse?>("api/booking/complete-booking", requestDto);
        }



        /// <summary>
        /// Retrieves the boarding pass for a specific ticket.
        /// </summary>
        /// <param name="ticketId">The unique identifier of the ticket.</param>
        /// <returns>
        /// An ApiResponse containing the boarding pass details including passenger information,
        /// flight details, seat assignment, and QR code, or an error message if not found.
        /// </returns>
        public async Task<ApiResponse<BoardingPass?>> GetBoardingPassAsync(int ticketId)
        {
            return await _apiService.GetAsync<BoardingPass?>($"api/tickets/{ticketId}/boarding-pass");
        }
    }
}