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

                bool wasAuthenticated = Preferences.ContainsKey("Token");



                if (isRoundTrip)
                {
                    await Shell.Current.GoToAsync($"///{nameof(View.FlightsSearchPage)}");


                    if (wasAuthenticated)
                    {
                        await Shell.Current.DisplayAlert("Booking Confirmed",
                            "Your round-trip booking is confirmed! View both tickets in the 'My Flights' section.", "OK");

                        await Shell.Current.GoToAsync($"///{nameof(View.UpcomingFlightsPage)}");
                    }
                    else
                    {

                        await Shell.Current.DisplayAlert("Booking Confirmed!",
                            "Your round-trip booking is confirmed! Account details were sent to your email. Please Log In to view your return ticket.", "OK");

                        await Shell.Current.GoToAsync($"///{nameof(View.LoginPage)}");
                    }

                }
                else
                {
                    if (!wasAuthenticated)
                    {
                        await Shell.Current.DisplayAlert("Booking Confirmed!",
                            "Your one-way booking is confirmed! Account details were sent to your email. Please Log In to manage your booking.", "OK");
                    }
                    else
                    {
                        await Shell.Current.DisplayAlert("Booking Confirmed!",
                           "Your one-way booking is confirmed! Your boarding pass was sent to your email.", "OK");
                    }

                    await Shell.Current.GoToAsync($"///{nameof(View.FlightsSearchPage)}");


                    var boardingPassRoute = $"{nameof(View.BoardingPassPage)}?TicketId={outboundTicketId}";
                    await Shell.Current.GoToAsync(boardingPassRoute);
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