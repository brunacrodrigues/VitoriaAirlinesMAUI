using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for managing seat and fare class selection for flight bookings.
    /// Handles both one-way and round-trip scenarios with complex navigation flow management.
    /// Provides reactive seat grids, fare class pricing, and dynamic UI state management
    /// based on user selections and booking context. Supports multi-leg booking workflows
    /// where outbound flight details are passed through to return flight selection.
    /// Implements comprehensive error handling and loading state management for API operations.
    /// </summary>
    [QueryProperty(nameof(FlightId), "FlightId")]
    [QueryProperty(nameof(SearchRequest), "SearchRequest")]
    [QueryProperty(nameof(IsRoundTripContext), "IsRoundTripContext")]
    [QueryProperty(nameof(IsOutboundLeg), "IsOutboundLeg")]
    [QueryProperty(nameof(ReturnFlightId), "ReturnFlightId")]
    [QueryProperty(nameof(SelectedOutboundFlight), "SelectedOutboundFlight")]
    [QueryProperty(nameof(SelectedOutboundSeat), "SelectedOutboundSeat")]
    [QueryProperty(nameof(SelectedOutboundFareClass), "SelectedOutboundFareClass")]
    public partial class SelectSeatViewModel : BaseViewModel
    {
        #region Dependencies

        /// <summary>
        /// Service dependency for flight-related API operations.
        /// Used to fetch flight seats data and pricing information from the backend.
        /// Injected through constructor for testability and dependency inversion.
        /// </summary>
        private readonly IFlightService _flightService;


        /// <summary>
        /// Service dependency for user profile-related API operations.
        /// Used to fetch authenticated user profile data from the backend.
        /// Injected through constructor for testability and dependency inversion.
        /// </summary>
        private readonly IProfileService _profileService;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the SelectSeatViewModel.
        /// Sets up service dependencies and configures initial page title.
        /// The ViewModel receives navigation parameters through QueryProperty attributes
        /// when navigated to from other pages in the booking flow.
        /// </summary>
        /// <param name="flightService">Service for flight-related API operations</param>
        public SelectSeatViewModel(IFlightService flightService, IProfileService profileService)
        {
            _flightService = flightService;
            _profileService = profileService;
            Title = "Select Your Fare & Seat";
        }

        #endregion

        #region Query Properties - Navigation Parameters

        /// <summary>
        /// The ID of the current flight for which seat selection is being performed.
        /// Received as navigation parameter and triggers automatic loading of flight seats data.
        /// When changed, initiates asynchronous loading of seat availability and pricing.
        /// </summary>
        [ObservableProperty]
        private int flightId;


        /// <summary>
        /// Flight details constructed from the loaded seat data response.
        /// Not directly received as QueryProperty but populated after successful API call.
        /// Used for displaying flight information and navigation to next steps.
        /// Contains comprehensive flight details needed for booking confirmation.
        /// </summary>
        [ObservableProperty]
        private FlightSearchResult? flightSearchResult;


        /// <summary>
        /// Original flight search parameters that initiated the booking flow.
        /// Received as navigation parameter and maintained throughout the booking process.
        /// Contains search criteria such as airports, dates, passengers, and trip type.
        /// Passed forward to subsequent pages for context preservation.
        /// </summary>
        [ObservableProperty]
        private FlightSearchRequest? searchRequest;


        /// <summary>
        /// Indicates whether this seat selection is part of a round-trip booking flow.
        /// When true, enables round-trip specific navigation logic and parameter passing.
        /// Controls whether the booking flow continues to return flight selection
        /// or proceeds directly to final confirmation for one-way trips.
        /// </summary>
        [ObservableProperty]
        private bool isRoundTripContext;


        /// <summary>
        /// Indicates whether this is the outbound leg of a round-trip booking.
        /// True for outbound leg selection, false for return leg selection.
        /// Used to determine navigation flow and parameter passing for round-trip bookings.
        /// Affects confirmation dialog text and subsequent navigation destinations.
        /// </summary>
        [ObservableProperty]
        private bool isOutboundLeg;


        /// <summary>
        /// The flight ID for the return leg of a round-trip booking.
        /// Only populated during outbound leg selection for round-trip bookings.
        /// Used to navigate to return leg seat selection after outbound confirmation.
        /// Remains zero for one-way trips or during return leg selection.
        /// </summary>
        [ObservableProperty]
        private int returnFlightId;


        /// <summary>
        /// Previously selected outbound flight details for round-trip bookings.
        /// Only populated during return leg selection, contains the confirmed outbound flight.
        /// Passed forward to final booking confirmation with both legs' details.
        /// Null during outbound leg selection and for one-way trips.
        /// </summary>
        [ObservableProperty]
        private FlightSearchResult? selectedOutboundFlight;


        /// <summary>
        /// Previously selected outbound seat details for round-trip bookings.
        /// Only populated during return leg selection, contains the confirmed outbound seat.
        /// Passed forward to final booking confirmation with complete seat selection data.
        /// Null during outbound leg selection and for one-way trips.
        /// </summary>
        [ObservableProperty]
        private SeatDetail? selectedOutboundSeat;


        /// <summary>
        /// Previously selected outbound fare class for round-trip bookings.
        /// Only populated during return leg selection, contains the confirmed outbound fare class.
        /// Passed forward to final booking confirmation with complete fare information.
        /// Defaults to None during outbound leg selection and for one-way trips.
        /// </summary>
        [ObservableProperty]
        private SeatClass selectedOutboundFareClass = SeatClass.None;


        #endregion

        #region Observable Properties - UI State Management

        /// <summary>
        /// Response containing flight details, seat availability, and pricing information.
        /// Loaded asynchronously when FlightId changes and triggers updates to dependent properties.
        /// Notifies EconomyPrice, ExecutivePrice, and HasFlightSeatsResponse for UI reactivity.
        /// Contains the authoritative seat data used to populate seat selection grids.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(EconomyPrice))]
        [NotifyPropertyChangedFor(nameof(ExecutivePrice))]
        [NotifyPropertyChangedFor(nameof(HasFlightSeatsResponse))]
        private FlightSeatsResponse? flightSeatsResponse;


        /// <summary>
        /// Complete collection of seat ViewModels for the current flight.
        /// Populated from FlightSeatsResponse data and wrapped in SeatViewModel for UI binding.
        /// Contains both available and occupied seats across all fare classes.
        /// Used as source for filtered display collections (DisplayEconomySeats, DisplayExecutiveSeats).
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<SeatViewModel> allSeats = new();


        /// <summary>
        /// The currently selected seat ViewModel, if any.
        /// When changed, automatically updates the CanConfirmSeat property for button state management.
        /// Null when no seat is selected or when fare class changes to incompatible type.
        /// Manages the IsSelected state of the underlying seat for visual feedback.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(CanConfirmSeat))]
        private SeatViewModel? selectedSeat;


        /// <summary>
        /// The currently selected fare class (Economy, Executive, or None).
        /// Controls which seat grid is displayed and affects seat selection logic.
        /// When changed, triggers extensive UI updates through multiple NotifyPropertyChangedFor attributes.
        /// Automatically clears seat selection if changing to incompatible fare class.
        /// Defaults to None, requiring explicit user selection before seat selection can proceed.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsEconomySelected))]
        [NotifyPropertyChangedFor(nameof(IsExecutiveSelected))]
        [NotifyPropertyChangedFor(nameof(ShowEconomySeatsGrid))]
        [NotifyPropertyChangedFor(nameof(ShowExecutiveSeatsGrid))]
        [NotifyPropertyChangedFor(nameof(CanConfirmSeat))]
        [NotifyPropertyChangedFor(nameof(IsFareClassSelected))]
        [NotifyPropertyChangedFor(nameof(IsFareClassNotSelected))]
        [NotifyPropertyChangedFor(nameof(IsEconomyNotSelected))]
        [NotifyPropertyChangedFor(nameof(IsExecutiveNotSelected))]
        private SeatClass selectedFareClass = SeatClass.None;


        #endregion

        #region Computed Properties - Display & UI Logic

        /// <summary>
        /// Formatted display string showing flight route and departure information.
        /// Combines origin and destination airports with IATA codes and departure date.
        /// Updates automatically when FlightSearchResult changes through manual property notification.
        /// Used for flight information display in the UI header or details section.
        /// </summary>
        public string FlightDetailsDisplay =>
            $"For flight ({FlightSearchResult?.OriginAirportIATA}) {FlightSearchResult?.OriginAirportFullName} → " +
            $"({FlightSearchResult?.DestinationAirportIATA}) {FlightSearchResult?.DestinationAirportFullName} " +
            $"on {FlightSearchResult?.DepartureUtc:MMM dd, yyyy}";


        /// <summary>
        /// Current economy class fare price from flight seats response.
        /// Updates automatically when FlightSeatsResponse changes due to NotifyPropertyChangedFor attribute.
        /// Returns zero if flight seats data is not yet loaded or unavailable.
        /// Used for fare selection UI and price display to users.
        /// </summary>
        public decimal EconomyPrice => FlightSeatsResponse?.EconomyClassPrice ?? 0;


        /// <summary>
        /// Current executive class fare price from flight seats response.
        /// Updates automatically when FlightSeatsResponse changes due to NotifyPropertyChangedFor attribute.
        /// Returns zero if flight seats data is not yet loaded or unavailable.
        /// Used for fare selection UI and price display to users.
        /// </summary>
        public decimal ExecutivePrice => FlightSeatsResponse?.ExecutiveClassPrice ?? 0;


        /// <summary>
        /// Indicates whether any economy class seats are available on this flight.
        /// Used to conditionally show/hide economy fare selection options in the UI.
        /// Performs efficient LINQ query on AllSeats collection to check availability.
        /// </summary>
        public bool HasEconomySeats => AllSeats.Any(s => s.Class == SeatClass.Economy);


        /// <summary>
        /// Indicates whether any executive class seats are available on this flight.
        /// Used to conditionally show/hide executive fare selection options in the UI.
        /// Performs efficient LINQ query on AllSeats collection to check availability.
        /// </summary>
        public bool HasExecutiveSeats => AllSeats.Any(s => s.Class == SeatClass.Executive);


        /// <summary>
        /// Inverse of IsEconomySelected for convenient XAML binding scenarios.
        /// Returns true when economy class is not selected, useful for hiding/showing UI elements.
        /// Updates automatically through property dependency chain when SelectedFareClass changes.
        /// </summary>
        public bool IsEconomyNotSelected => !IsEconomySelected;


        /// <summary>
        /// Inverse of IsExecutiveSelected for convenient XAML binding scenarios.
        /// Returns true when executive class is not selected, useful for hiding/showing UI elements.
        /// Updates automatically through property dependency chain when SelectedFareClass changes.
        /// </summary>
        public bool IsExecutiveNotSelected => !IsExecutiveSelected;


        /// <summary>
        /// Filtered collection of available economy seats for UI display.
        /// Excludes occupied seats to prevent selection of unavailable seats.
        /// Creates new ObservableCollection on each access - consider caching if performance becomes an issue.
        /// Updates when AllSeats collection changes through manual property change notifications.
        /// </summary>
        public ObservableCollection<SeatViewModel> DisplayEconomySeats =>
            new ObservableCollection<SeatViewModel>(AllSeats.Where(s => s.Class == SeatClass.Economy && !s.IsOccupied));


        /// <summary>
        /// Filtered collection of available executive seats for UI display.
        /// Excludes occupied seats to prevent selection of unavailable seats.
        /// Creates new ObservableCollection on each access - consider caching if performance becomes an issue.
        /// Updates when AllSeats collection changes through manual property change notifications.
        /// </summary>
        public ObservableCollection<SeatViewModel> DisplayExecutiveSeats =>
            new ObservableCollection<SeatViewModel>(AllSeats.Where(s => s.Class == SeatClass.Executive && !s.IsOccupied));


        /// <summary>
        /// Indicates whether economy fare class is currently selected.
        /// Used for UI state management, button styling, and conditional visibility.
        /// Updates automatically when SelectedFareClass changes due to NotifyPropertyChangedFor attributes.
        /// </summary>
        public bool IsEconomySelected => SelectedFareClass == SeatClass.Economy;


        /// <summary>
        /// Indicates whether executive fare class is currently selected.
        /// Used for UI state management, button styling, and conditional visibility.
        /// Updates automatically when SelectedFareClass changes due to NotifyPropertyChangedFor attributes.
        /// </summary>
        public bool IsExecutiveSelected => SelectedFareClass == SeatClass.Executive;


        /// <summary>
        /// Indicates whether any fare class has been selected (not None).
        /// Used to enable/disable UI elements that require fare class selection.
        /// Updates automatically when SelectedFareClass changes due to NotifyPropertyChangedFor attributes.
        /// </summary>
        public bool IsFareClassSelected => SelectedFareClass != SeatClass.None;


        /// <summary>
        /// Indicates whether no fare class has been selected (is None).
        /// Inverse of IsFareClassSelected for convenient XAML binding scenarios.
        /// Updates automatically when SelectedFareClass changes due to NotifyPropertyChangedFor attributes.
        /// </summary>
        public bool IsFareClassNotSelected => SelectedFareClass == SeatClass.None;


        /// <summary>
        /// Controls visibility of the economy seats grid in the UI.
        /// Returns true only when economy fare class is selected, hiding other seat grids.
        /// Updates automatically when SelectedFareClass changes due to NotifyPropertyChangedFor attributes.
        /// </summary>
        public bool ShowEconomySeatsGrid => SelectedFareClass == SeatClass.Economy;


        /// <summary>
        /// Controls visibility of the executive seats grid in the UI.
        /// Returns true only when executive fare class is selected, hiding other seat grids.
        /// Updates automatically when SelectedFareClass changes due to NotifyPropertyChangedFor attributes.
        /// </summary>
        public bool ShowExecutiveSeatsGrid => SelectedFareClass == SeatClass.Executive;


        /// <summary>
        /// Indicates whether flight seats response data has been loaded.
        /// Used to control visibility of UI elements that depend on loaded seat data.
        /// Updates automatically when FlightSeatsResponse changes due to NotifyPropertyChangedFor attribute.
        /// </summary>
        public bool HasFlightSeatsResponse => FlightSeatsResponse != null;


        /// <summary>
        /// Determines whether the seat confirmation action can be executed.
        /// Requires both a seat selection and fare class selection, plus the ViewModel must not be busy.
        /// Used as CanExecute predicate for ConfirmSeatSelectionCommand to control button state.
        /// Updates automatically through property dependency chain when related properties change.
        /// </summary>
        public bool CanConfirmSeat => SelectedSeat != null && SelectedFareClass != SeatClass.None && !IsBusy;


        #endregion

        #region Reactive Methods - Property Change Handlers

        /// <summary>
        /// Handles changes to the FlightId property received via QueryProperty navigation.
        /// Automatically triggered when the page receives a FlightId parameter during navigation.
        /// Initiates asynchronous loading of flight seats data through fire-and-forget pattern.
        /// Uses async void pattern through task fire-and-forget to avoid blocking navigation.
        /// </summary>
        /// <param name="value">The new flight ID value from navigation parameters</param>
        partial void OnFlightIdChanged(int value)
        {
            if (value > 0)
            {
                // Fire-and-forget async call - exceptions handled within ProcessFlightIdChangeAsync
                _ = ProcessFlightIdChangeAsync(value);
            }
        }


        /// <summary>
        /// Asynchronous handler for FlightId changes that loads flight data and constructs UI models.
        /// Performs two-phase loading: first loads seat data via API, then constructs FlightSearchResult.
        /// Implements comprehensive error handling with user notifications for API failures.
        /// Updates UI state through property notifications after successful data loading.
        /// </summary>
        /// <param name="flightIdValue">The flight ID to load data for</param>
        private async Task ProcessFlightIdChangeAsync(int flightIdValue)
        {
            try
            {

                await LoadFlightSeatsAsync(flightIdValue);


                if (FlightSeatsResponse != null)
                {
                    FlightSearchResult = new FlightSearchResult
                    {
                        FlightId = FlightSeatsResponse.FlightId,
                        FlightNumber = FlightSeatsResponse.FlightNumber,
                        OriginAirportIATA = FlightSeatsResponse.OriginAirportIATA,
                        OriginAirportFullName = FlightSeatsResponse.OriginAirportName,
                        OriginCountryCode = FlightSeatsResponse.OriginCountryCode,
                        DestinationAirportIATA = FlightSeatsResponse.DestinationAirportIATA,
                        DestinationAirportFullName = FlightSeatsResponse.DestinationAirportName,
                        DestinationCountryCode = FlightSeatsResponse.DestinationCountryCode,
                        DepartureUtc = FlightSeatsResponse.DepartureUtc,
                        ArrivalUtc = FlightSeatsResponse.ArrivalUtc,
                        Duration = FlightSeatsResponse.Duration,
                        EconomyClassPrice = FlightSeatsResponse.EconomyClassPrice,
                        ExecutiveClassPrice = FlightSeatsResponse.ExecutiveClassPrice,
                        AvailableSeats = 0,
                        Status = "Scheduled"
                    };

                    // Trigger update for computed display property
                    OnPropertyChanged(nameof(FlightDetailsDisplay));
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error processing flight details: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }
        }


        /// <summary>
        /// Handles changes to the selected fare class and manages related UI state updates.
        /// Automatically clears seat selection if the new fare class is incompatible with current selection.
        /// Triggers comprehensive UI updates through manual property change notifications.
        /// Updates command executability state to reflect new selection constraints.
        /// </summary>
        /// <param name="value">The newly selected fare class value</param>
        partial void OnSelectedFareClassChanged(SeatClass value)
        {
            // Clear seat selection if incompatible with new fare class
            if (SelectedSeat != null && SelectedSeat.Class != value)
            {
                SelectedSeat.IsSelected = false; // Update UI state
                SelectedSeat = null; // Clear ViewModel selection
            }

            // Trigger UI updates for fare class dependent properties
            OnPropertyChanged(nameof(ShowEconomySeatsGrid));
            OnPropertyChanged(nameof(ShowExecutiveSeatsGrid));
            OnPropertyChanged(nameof(IsFareClassSelected));
            OnPropertyChanged(nameof(IsFareClassNotSelected));

            // Update filtered seat collections
            OnPropertyChanged(nameof(DisplayEconomySeats));
            OnPropertyChanged(nameof(DisplayExecutiveSeats));

            // Update command executability
            ConfirmSeatSelectionCommand.NotifyCanExecuteChanged();
        }


        /// <summary>
        /// Handles changes to the selected seat and updates command executability.
        /// Automatically called when SelectedSeat property changes to update dependent UI state.
        /// Triggers re-evaluation of the ConfirmSeatSelectionCommand's CanExecute state.
        /// </summary>
        /// <param name="value">The newly selected seat ViewModel, can be null</param>
        partial void OnSelectedSeatChanged(SeatViewModel? value)
        {
            ConfirmSeatSelectionCommand.NotifyCanExecuteChanged();
        }


        #endregion

        #region Commands - User Actions

        /// <summary>
        /// Command to select a fare class (Economy or Executive).
        /// Updates the SelectedFareClass property which triggers cascading UI updates.
        /// Enables the appropriate seat grid display and fare-specific functionality.
        /// </summary>
        /// <param name="fareClass">The fare class to select (Economy or Executive)</param>
        [RelayCommand]
        private void SelectFare(SeatClass fareClass)
        {
            SelectedFareClass = fareClass;
        }


        /// <summary>
        /// Command to select a specific seat from the available seats grid.
        /// Implements toggle behavior - clicking the same seat deselects it.
        /// Validates seat availability, fare class compatibility, and busy state before selection.
        /// Manages visual selection state for both old and new seat selections.
        /// </summary>
        /// <param name="seat">The seat ViewModel to select/deselect</param>
        [RelayCommand]
        private void SelectSeat(SeatViewModel seat)
        {
            if (IsBusy || seat == null || seat.IsOccupied || seat.Class != SelectedFareClass) return;

            // Clear previous selection
            if (SelectedSeat != null)
            {
                SelectedSeat.IsSelected = false;
            }

            // Toggle selection - if same seat clicked, deselect it
            if (SelectedSeat == seat)
            {
                SelectedSeat = null;
            }
            else
            {
                SelectedSeat = seat;
                SelectedSeat.IsSelected = true;
            }
        }


        /// <summary>
        /// Command to confirm the current seat selection and proceed to next step.
        /// Implements complex navigation logic for both one-way and round-trip booking flows.
        /// For round-trip outbound leg: navigates to return flight seat selection.
        /// For round-trip return leg: prepares data for final booking confirmation.
        /// For one-way trips: prepares data for booking confirmation.
        /// Includes comprehensive validation and error handling throughout the process.
        /// </summary>
        [RelayCommand(CanExecute = nameof(CanConfirmSeat))]
        public async Task ConfirmSeatSelectionAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                // Validate required data
                if (SelectedSeat == null || FlightSeatsResponse == null || FlightSearchResult == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Please select a fare and a seat.", "OK");
                    return;
                }


                // 2. VALIDAÇÃO DO PERFIL PARA CLIENTES AUTENTICADOS (Nova Lógica)
                if (Preferences.ContainsKey("Token"))
                {
                    var profileResponse = await _profileService.GetProfileAsync();

                    // Nota: Verificamos apenas se o perfil existe. A validação de preenchimento (Passport/Country) 
                    // será feita no BookingConfirmationViewModel/Backend, mas a navegação para a ProfilePage
                    // é feita aqui para o utilizador corrigir.
                    if (!profileResponse.IsSuccess || profileResponse.Data == null)
                    {
                        bool goToProfile = await Shell.Current.DisplayAlert(
                            "Profile Error",
                            "Could not load your profile details. Please go to your profile page to ensure your details are complete.",
                            "Go to Profile", "Cancel");

                        if (goToProfile)
                        {
                            await Shell.Current.GoToAsync("ProfilePage");
                        }
                        return;
                    }
                }



                var seatDetail = SelectedSeat.SeatDetail;

                if (IsRoundTripContext)
                {
                    if (IsOutboundLeg)
                    {
                        // Outbound leg of round-trip: show confirmation and navigate to return leg
                        await Shell.Current.DisplayAlert("Seat Selected (Outbound Leg)",
                            $"Outbound Flight: {FlightSeatsResponse.FlightNumber}\n" +
                            $"Selected {SelectedFareClass} Seat: {seatDetail.Row}{seatDetail.Letter}", "OK");

                        if (ReturnFlightId <= 0)
                        {
                            await Shell.Current.DisplayAlert("Error", "Return flight ID is missing for round trip.", "OK");
                            return;
                        }

                        // Navigate to return flight seat selection with outbound details
                        var navigationParams = new Dictionary<string, object>
                        {
                            { "FlightId", ReturnFlightId },
                            { "SearchRequest", SearchRequest! },
                            { "IsRoundTripContext", true },
                            { "IsOutboundLeg", false },
                            { "SelectedOutboundFlight", FlightSearchResult },
                            { "SelectedOutboundSeat", seatDetail },
                            { "SelectedOutboundFareClass", SelectedFareClass }
                        };

                        await Shell.Current.GoToAsync("SelectSeatPage", navigationParams);
                    }
                    else
                    {
                        // Return leg of round-trip: show confirmation and prepare for booking
                        await Shell.Current.DisplayAlert("Seat Selected (Return Leg)",
                            $"Return Flight: {FlightSeatsResponse.FlightNumber}\n" +
                            $"Selected {SelectedFareClass} Seat: {seatDetail.Row}{seatDetail.Letter}", "OK");

                        // Prepare complete booking data for confirmation page
                        var navigationParams = new Dictionary<string, object>
                        {
                            { "OutboundFlight", SelectedOutboundFlight! },
                            { "OutboundSeat", SelectedOutboundSeat! },
                            { "OutboundFareClass", SelectedOutboundFareClass },
                            { "ReturnFlight", FlightSearchResult },
                            { "ReturnSeat", seatDetail },
                            { "ReturnFareClass", SelectedFareClass },
                            { "SearchRequest", SearchRequest! }
                        };

                        await Shell.Current.GoToAsync(nameof(BookingConfirmationPage), navigationParams);
                    }
                }

                else
                {

                    await Shell.Current.DisplayAlert("Seat Selected (One-Way)",
                        $"Flight: {FlightSeatsResponse.FlightNumber}\n" +
                        $"Selected {SelectedFareClass} Seat: {seatDetail.Row}{seatDetail.Letter}", "OK");

                    var navigationParams = new Dictionary<string, object>
                    {
                        { "OutboundFlight", FlightSearchResult },
                        { "OutboundSeat", seatDetail },
                        { "OutboundFareClass", SelectedFareClass },
                        { "SearchRequest", SearchRequest! }
                    };

                    await Shell.Current.GoToAsync(nameof(BookingConfirmationPage), navigationParams);
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error confirming seat: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }

        #endregion

        #region Private Methods - Data Loading

        /// <summary>
        /// Loads flight seats data and pricing information from the flight service API.
        /// Implements comprehensive error handling for different failure scenarios including
        /// HTTP request exceptions and general API errors. Updates busy state and error properties
        /// throughout the loading process. Populates AllSeats collection with SeatViewModel wrappers
        /// and triggers UI updates for dependent computed properties.
        /// </summary>
        /// <param name="fId">The flight ID to load seat data for</param>
        private async Task LoadFlightSeatsAsync(int fId)
        {
            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                var apiResponse = await _flightService.GetFlightSeatsAsync(fId);

                if (apiResponse.IsSuccess && apiResponse.Data != null)
                {
                    FlightSeatsResponse = apiResponse.Data;

                    if (FlightSeatsResponse.Seats != null)
                    {
                        // Clear existing seats and populate with new data
                        AllSeats.Clear();
                        foreach (var seatDetail in FlightSeatsResponse.Seats)
                        {
                            AllSeats.Add(new SeatViewModel(seatDetail));
                        }

                        // Update filtered display collections
                        OnPropertyChanged(nameof(DisplayEconomySeats));
                        OnPropertyChanged(nameof(DisplayExecutiveSeats));
                    }
                    else
                    {
                        ErrorMessage = "No seat data found for this flight.";
                        HasError = true;
                    }
                }
                else
                {
                    ErrorMessage = apiResponse.Message ?? "Failed to retrieve flight seats data.";
                    HasError = true;
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = $"API Error: {ex.Message}";
                HasError = true;
                await Shell.Current.DisplayAlert("API Error", ErrorMessage, "OK");
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error loading flight seats: {ex.Message}";
                HasError = true;
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }
            finally
            {
                IsBusy = false;
                OnPropertyChanged(nameof(HasFlightSeatsResponse));
            }
        }

        #endregion
    }
}