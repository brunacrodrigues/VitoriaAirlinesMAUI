using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.ViewModel;

[QueryProperty(nameof(CheckoutUrl), "CheckoutUrl")]
[QueryProperty(nameof(StripeSessionId), "StripeSessionId")]
public partial class PaymentViewModel : BaseViewModel
{
    private readonly IBookingService _bookingService;

    [ObservableProperty] private string checkoutUrl = string.Empty;
    [ObservableProperty] private string stripeSessionId = string.Empty;

    public PaymentViewModel(IBookingService bookingService)
    {
        _bookingService = bookingService;
        Title = "Payment";
    }


    [RelayCommand]
    private async Task CompleteBookingAsync()
    {
        IsBusy = true;
        HasError = false;
        ErrorMessage = string.Empty;

        System.Diagnostics.Debug.WriteLine($"[CompleteBooking] Starting API Call for SessionId: {StripeSessionId}");

        try
        {
            if (string.IsNullOrEmpty(StripeSessionId))
            {
                throw new ArgumentNullException(nameof(StripeSessionId));
            }

            var apiResponse = await _bookingService.CompleteBookingAsync(StripeSessionId);



            if (apiResponse.IsSuccess && apiResponse.Data != null)
            {
                var outboundTicketId = apiResponse.Data.OutboundTicketId;
                var returnTicketId = apiResponse.Data.ReturnTicketId;

                var isRoundTrip = returnTicketId.HasValue && returnTicketId.Value > 0;



                await Shell.Current.GoToAsync($"//{nameof(View.FlightsSearchPage)}");


                if (isRoundTrip)
                {

                    await Shell.Current.DisplayAlert("Booking Confirmed",
                        "Your round-trip booking is confirmed! View both tickets in the 'My Flights' section.", "OK");


                    await Shell.Current.GoToAsync($"//{nameof(View.UpcomingFlightsPage)}");
                }
                else
                {

                    var destinationRoute = $"{nameof(View.BoardingPassPage)}?TicketId={outboundTicketId}";
                    await Shell.Current.GoToAsync(destinationRoute);
                }

                return;
            }
            else
            {
                HasError = true;
                ErrorMessage = apiResponse.Message ?? "Failed to finalize booking.";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {

            HasError = true;
            ErrorMessage = $"Payment processing error. Please contact support. Details: {ex.Message}";
            await Shell.Current.DisplayAlert("Fatal Error", ErrorMessage, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}