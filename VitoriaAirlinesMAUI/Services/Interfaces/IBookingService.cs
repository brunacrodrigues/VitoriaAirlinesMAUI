using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.Services.Interfaces
{
    /// <summary>
    /// Defines the contract for booking-related operations including checkout, payment completion,
    /// and boarding pass retrieval.
    /// </summary>
    public interface IBookingService
    {
        /// <summary>
        /// Creates a Stripe checkout session for a new booking.
        /// </summary>
        /// <param name="request">The booking request containing flight and passenger details.</param>
        /// <returns>
        /// An ApiResponse containing the checkout session information including the Stripe session URL,
        /// or an error message if the operation fails.
        /// </returns>
        Task<ApiResponse<CreateCheckoutResponse?>> CreateCheckoutSessionAsync(BookingRequest request);


        /// <summary>
        /// Completes the booking process after successful payment confirmation.
        /// </summary>
        /// <param name="stripeSessionId">The Stripe session ID returned after successful payment.</param>
        /// <returns>
        /// An ApiResponse containing the completed booking details including ticket information,
        /// or an error message if the completion fails.
        /// </returns>
        Task<ApiResponse<CompleteBookingResponse?>> CompleteBookingAsync(string stripeSessionId);


        /// <summary>
        /// Retrieves the boarding pass for a specific ticket.
        /// </summary>
        /// <param name="ticketId">The unique identifier of the ticket.</param>
        /// <returns>
        /// An ApiResponse containing the boarding pass details including passenger information,
        /// flight details, seat assignment, and QR code, or an error message if not found.
        /// </returns>
        Task<ApiResponse<BoardingPass?>> GetBoardingPassAsync(int ticketId);
    }
}
