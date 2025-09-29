using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel;

[QueryProperty(nameof(OutboundFlight), "OutboundFlight")]
[QueryProperty(nameof(OutboundSeat), "OutboundSeat")]
[QueryProperty(nameof(OutboundFareClass), "OutboundFareClass")]
[QueryProperty(nameof(ReturnFlight), "ReturnFlight")]
[QueryProperty(nameof(ReturnSeat), "ReturnSeat")]
[QueryProperty(nameof(ReturnFareClass), "ReturnFareClass")]
[QueryProperty(nameof(SearchRequest), "SearchRequest")]
public partial class BookingConfirmationViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IProfileService _profileService;
    private readonly IBookingService _bookingService;

    // Navigation Properties
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutboundPrice))]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private FlightSearchResult? outboundFlight;

    [ObservableProperty]
    private SeatDetail? outboundSeat;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(OutboundPrice))]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private SeatClass outboundFareClass;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsRoundTrip))]
    [NotifyPropertyChangedFor(nameof(ReturnPrice))]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private FlightSearchResult? returnFlight;

    [ObservableProperty]
    private SeatDetail? returnSeat;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ReturnPrice))]
    [NotifyPropertyChangedFor(nameof(TotalPrice))]
    private SeatClass returnFareClass;

    [ObservableProperty] private FlightSearchRequest? searchRequest;


    // User Authentication State
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotCustomerAuthenticated))]
    [NotifyPropertyChangedFor(nameof(ShowPassportFields))]
    private bool isCustomerAuthenticated = false;


    // Profile completeness state
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ShowPassportFields))]
    [NotifyPropertyChangedFor(nameof(IsProfileIncomplete))]
    private bool isProfileComplete = false;


    // User Profile Data
    [ObservableProperty] private CustomerProfile? customerProfile;
    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string passportNumber = string.Empty;



    // Computed Properties for UI Visibility
    public bool IsRoundTrip => ReturnFlight != null;
    public decimal OutboundPrice => OutboundFareClass == SeatClass.Economy ? OutboundFlight?.EconomyClassPrice ?? 0 : OutboundFlight?.ExecutiveClassPrice ?? 0;
    public decimal ReturnPrice => ReturnFareClass == SeatClass.Economy ? ReturnFlight?.EconomyClassPrice ?? 0 : ReturnFlight?.ExecutiveClassPrice ?? 0;
    public decimal TotalPrice => OutboundPrice + (IsRoundTrip ? ReturnPrice : 0);
    public bool IsNotCustomerAuthenticated => !IsCustomerAuthenticated;
    public bool ShowPassportFields => IsNotCustomerAuthenticated || !IsProfileComplete;
    public bool IsProfileIncomplete => !IsProfileComplete;
    public bool HasBoardingPass => BoardingPass != null;
    public bool IsPassportReadOnly => IsCustomerAuthenticated && IsProfileComplete;



    // Add boarding pass property for the final step
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasBoardingPass))]
    private BoardingPass? boardingPass;

    public BookingConfirmationViewModel(
        IAuthService authService,
        IProfileService profileService,
        IBookingService bookingService)
    {
        _authService = authService;
        _profileService = profileService;
        _bookingService = bookingService;
        Title = "Confirm Booking";
        IsCustomerAuthenticated = Preferences.ContainsKey("Token");
    }


    partial void OnOutboundFlightChanged(FlightSearchResult? value)
    {
        _ = LoadDataAsync();
    }



    private async Task LoadDataAsync()
    {
        if (IsCustomerAuthenticated)
        {
            await LoadProfileAsync();
        }
    }



    private async Task LoadProfileAsync()
    {
        if (!IsCustomerAuthenticated) return;

        try
        {
            var apiResponse = await _profileService.GetProfileAsync();
            if (apiResponse.IsSuccess && apiResponse.Data != null)
            {
                CustomerProfile = apiResponse.Data;


                FirstName = CustomerProfile.FirstName;
                LastName = CustomerProfile.LastName;
                Email = CustomerProfile.Email;
                PassportNumber = CustomerProfile.PassportNumber ?? string.Empty;


                IsProfileComplete = !string.IsNullOrWhiteSpace(CustomerProfile.PassportNumber);
            }
            else
            {
                IsProfileComplete = false;
            }
        }
        catch (Exception ex)
        {
            IsProfileComplete = false;
            HasError = true;
            ErrorMessage = $"Error loading user profile: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
        }
    }



    [RelayCommand]
    private async Task ProceedToPaymentAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        HasError = false;
        ErrorMessage = string.Empty;

        var currentToken = Preferences.Get("Token", string.Empty);

        try
        {
            // Validation for both authenticated and anonymous users
            if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(PassportNumber))
            {
                ErrorMessage = "Please fill in all required passenger details (Name, Email, Passport) to continue.";
                HasError = true;
                return;
            }

            // Update profile if user is authenticated but profile was incomplete
            if (IsCustomerAuthenticated && !IsProfileComplete)
            {
                await UpdateProfileAsync();
            }


            var request = new BookingRequest
            {
                Legs = new List<BookingLeg>
                {
                    new BookingLeg { FlightId = OutboundFlight!.FlightId, SeatId = OutboundSeat!.Id }
                },
                FirstName = FirstName,
                LastName = LastName,
                Email = Email,
                PassportNumber = PassportNumber
            };

            if (IsRoundTrip)
            {
                request.Legs.Add(new BookingLeg { FlightId = ReturnFlight!.FlightId, SeatId = ReturnSeat!.Id });
            }


            var apiResponse = await _bookingService.CreateCheckoutSessionAsync(request);

            if (apiResponse.IsSuccess && apiResponse.Data != null)
            {
                var navigationParams = new Dictionary<string, object>
                {
                    { "CheckoutUrl", apiResponse.Data.CheckoutUrl },
                    { "StripeSessionId", apiResponse.Data.StripeSessionId }
                };
                await Shell.Current.GoToAsync(nameof(PaymentPage), navigationParams);
            }
            else
            {
                HasError = true;
                ErrorMessage = apiResponse.Message ?? "Failed to initiate payment session.";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Error processing payment: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }



    private async Task UpdateProfileAsync()
    {
        try
        {
            var updateRequest = new UpdateCustomerProfileRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                PassportNumber = PassportNumber,
                CountryId = CustomerProfile?.CountryId
            };

            var result = await _profileService.UpdateProfileAsync(updateRequest);
            if (result.IsSuccess)
            {
                IsProfileComplete = true;
            }
        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = $"Error updating user profile: {ex.Message}";
            await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
        }
    }
}