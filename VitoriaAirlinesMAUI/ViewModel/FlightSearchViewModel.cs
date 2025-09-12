using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for managing flight search functionality.
    /// Handles user input validation, airport selection, date management, and flight search operations.
    /// Implements reactive programming patterns using MVVM Community Toolkit.
    /// </summary>
    public partial class FlightSearchViewModel : BaseViewModel
    {

        /// <summary>
        /// Flight service dependency for API calls related to airports and flight searches.
        /// </summary>
        private readonly IFlightService _flightService;



        #region Observable Properties

        /// <summary>
        /// Collection of available airports for origin and destination selection.
        /// Populated from the API when the view loads.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<AirportOption> availableAirports = new();


        /// <summary>
        /// Currently selected origin airport.
        /// Triggers validation of search command and country code visibility when changed.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchFlightsCommand))]
        [NotifyPropertyChangedFor(nameof(IsOriginCountryCodeVisible))]
        private AirportOption? selectedOriginAirport;


        /// <summary>
        /// Currently selected destination airport.
        /// Triggers validation of search command and country code visibility when changed.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchFlightsCommand))]
        [NotifyPropertyChangedFor(nameof(IsDestinationCountryCodeVisible))]
        private AirportOption? selectedDestinationAirport;


        /// <summary>
        /// Selected departure date for the flight search.
        /// Defaults to today's date and triggers search validation when changed.
        /// </summary>
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SearchFlightsCommand))]
        private DateTime departureDate = DateTime.Today;


        /// <summary>
        /// Optional return date for round-trip flights.
        /// Automatically set to day after departure for round-trip searches.
        /// Set to null for one-way trips.
        /// </summary>
        [ObservableProperty]
        private DateTime? returnDate = DateTime.Today.AddDays(1);


        /// <summary>
        /// Indicates whether the search is for a round-trip flight.
        /// Defaults to true (round-trip). Controls return date visibility and validation.
        /// </summary>
        [ObservableProperty]
        private bool isRoundTrip = true;


        /// <summary>
        /// Number of passengers for the flight search.
        /// Defaults to 1 passenger with a range of 1-10.
        /// </summary>
        [ObservableProperty]
        private int numberOfPassengers = 1;


        /// <summary>
        /// Controls the visibility of the return date picker in the UI.
        /// True for round-trip searches, false for one-way searches.
        /// </summary>
        [ObservableProperty]
        private bool showReturnDate = true;

        #endregion

        #region Computed Properties

        /// <summary>
        /// Computed property that represents the inverse of IsRoundTrip.
        /// Used for one-way trip radio button binding.
        /// </summary>
        public bool IsOneWayTrip
        {
            get => !IsRoundTrip;
            set
            {
                if (value != IsOneWayTrip) // Only update if value actually changes
                {
                    IsRoundTrip = !value; // Toggling IsOneWayTrip changes IsRoundTrip
                }
            }
        }



        /// <summary>
        /// Determines whether the origin airport's country flag should be visible in the UI.
        /// Returns true if the selected origin airport has a valid country code.
        /// </summary>
        public bool IsOriginCountryCodeVisible => !string.IsNullOrWhiteSpace(SelectedOriginAirport?.CountryCode);


        /// <summary>
        /// Determines whether the destination airport's country flag should be visible in the UI.
        /// Returns true if the selected destination airport has a valid country code.
        /// </summary>
        public bool IsDestinationCountryCodeVisible => !string.IsNullOrWhiteSpace(SelectedDestinationAirport?.CountryCode);

        #endregion



        /// <summary>
        /// Initializes a new instance of the FlightSearchViewModel.
        /// </summary>
        /// <param name="flightService">Service for flight-related API operations</param>
        public FlightSearchViewModel(IFlightService flightService)
        {
            Title = "Book Flight";
            _flightService = flightService;
            UpdateReturnDateVisibility();
        }


        #region Reactive Methods (Property Change Handlers)

        /// <summary>
        /// Handles changes to the IsRoundTrip property.
        /// Updates return date visibility, sets/clears return date, and validates search command.
        /// </summary>
        /// <param name="value">New value of IsRoundTrip</param>
        partial void OnIsRoundTripChanged(bool value)
        {
            // Notify UI that IsOneWayTrip property has changed
            OnPropertyChanged(nameof(IsOneWayTrip));

            // Update return date field visibility
            UpdateReturnDateVisibility();

            if (value && (!ReturnDate.HasValue || ReturnDate.Value < DepartureDate))
            {
                // Set default return date for round-trip
                ReturnDate = DepartureDate.AddDays(1);
            }
            else if (!value)
            {
                // Clear return date for one-way trip
                ReturnDate = null;
            }

            // Revalidate search command availability
            SearchFlightsCommand.NotifyCanExecuteChanged();
        }



        /// <summary>
        /// Handles changes to the departure date.
        /// Ensures return date is not before departure date for round-trip searches.
        /// </summary>
        /// <param name="value">New departure date</param>
        partial void OnDepartureDateChanged(DateTime value)
        {
            // Ensure return date is not before departure date if it's a round trip
            if (IsRoundTrip && ReturnDate.HasValue && ReturnDate.Value.Date < value.Date)
            {
                ReturnDate = value.AddDays(1);
            }
            SearchFlightsCommand.NotifyCanExecuteChanged();
        }



        /// <summary>
        /// Handles changes to the return date.
        /// Triggers validation of the search command.
        /// </summary>
        /// <param name="value">New return date (nullable)</param>
        partial void OnReturnDateChanged(DateTime? value)
        {
            SearchFlightsCommand.NotifyCanExecuteChanged();
        }



        /// <summary>
        /// Handles changes to the selected origin airport.
        /// Prevents selecting the same airport for origin and destination.
        /// </summary>
        /// <param name="value">Newly selected origin airport</param>
        partial void OnSelectedOriginAirportChanged(AirportOption? value)
        {
            // Prevent same origin and destination airports
            if (value != null && value.Id == SelectedDestinationAirport?.Id)
            {
                SelectedDestinationAirport = null;
            }
            SearchFlightsCommand.NotifyCanExecuteChanged();
        }


        /// <summary>
        /// Handles changes to the selected destination airport.
        /// Prevents selecting the same airport for origin and destination.
        /// </summary>
        /// <param name="value">Newly selected destination airport</param>
        partial void OnSelectedDestinationAirportChanged(AirportOption? value)
        {
            // Prevent same origin and destination airports
            if (value != null && value.Id == SelectedOriginAirport?.Id)
            {
                SelectedOriginAirport = null;
            }
            SearchFlightsCommand.NotifyCanExecuteChanged();
        }



        #endregion

        #region Private Helper Methods

        /// <summary>
        /// Updates the visibility of the return date picker based on the trip type.
        /// Shows return date field for round-trip, hides it for one-way.
        /// </summary>
        private void UpdateReturnDateVisibility()
        {
            ShowReturnDate = IsRoundTrip;
        }



        /// <summary>
        /// Validates whether the search flights command can be executed.
        /// Checks for required fields, valid dates, and prevents duplicate airports.
        /// </summary>
        /// <returns>True if search can be performed, false otherwise</returns>
        private bool CanSearchFlights()
        {
            // Cannot search while busy
            if (IsBusy)
                return false;

            // Both airports must be selected
            if (SelectedOriginAirport == null || SelectedDestinationAirport == null)
                return false;

            // Origin and destination must be different
            if (SelectedOriginAirport.Id == SelectedDestinationAirport.Id)
                return false;

            // Departure date must be set
            if (DepartureDate == default)
                return false;

            // For round-trip: return date must be set and not before departure
            if (IsRoundTrip && (!ReturnDate.HasValue || ReturnDate.Value.Date < DepartureDate.Date))
                return false;

            return true;
        }



        #endregion

        #region Commands

        /// <summary>
        /// Asynchronously loads the list of available airports from the API.
        /// Handles loading states, error management, and sorts airports by name.
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        [RelayCommand]
        public async Task LoadAirportsAsync()
        {
            if (IsBusy) return;


            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                // Call flight service to get airports
                var response = await _flightService.GetAirportsAsync();

                if (response.IsSuccess && response.Data != null)
                {
                    // Clear existing airports and populate with sorted list
                    AvailableAirports.Clear();
                    foreach (var airport in response.Data.OrderBy(a => a.Name))
                    {
                        AvailableAirports.Add(airport);
                    }
                }
                else
                {
                    HasError = true;
                    ErrorMessage = response.Message ?? "Failed to load airports.";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error loading airports: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }



        /// <summary>
        /// Asynchronously searches for flights based on current search criteria.
        /// Performs client-side validation, calls flight service, and navigates to results page.
        /// Command execution is controlled by CanSearchFlights method.
        /// </summary>
        /// <returns>Task representing the async operation</returns>
        [RelayCommand(CanExecute = nameof(CanSearchFlights))]
        public async Task SearchFlightsAsync()
        {
            if (IsBusy) return;


            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                #region Client-Side Validation

                // Validate airport selection
                if (SelectedOriginAirport == null || SelectedDestinationAirport == null)
                {
                    HasError = true;
                    ErrorMessage = "Please select both origin and destination airports.";
                    return;
                }

                // Validate different airports
                if (SelectedOriginAirport.Id == SelectedDestinationAirport.Id)
                {
                    HasError = true;
                    ErrorMessage = "Origin and destination airports cannot be the same.";
                    return;
                }

                // Validate departure date
                if (DepartureDate < DateTime.Today)
                {
                    HasError = true;
                    ErrorMessage = "Departure date cannot be in the past.";
                    return;
                }

                // Validate return date for round-trip
                if (IsRoundTrip && (!ReturnDate.HasValue || ReturnDate.Value.Date < DepartureDate.Date))
                {
                    HasError = true;
                    ErrorMessage = "Return date must be selected and cannot be earlier than departure date for round trips.";
                    return;
                }

                #endregion

                // Create search request object
                var request = new FlightSearchRequest
                {
                    OriginAirportId = SelectedOriginAirport.Id,
                    DestinationAirportId = SelectedDestinationAirport.Id,
                    DepartureDate = DepartureDate,
                    ReturnDate = IsRoundTrip ? ReturnDate : null,
                    IsRoundTrip = IsRoundTrip,
                    NumberOfPassengers = NumberOfPassengers
                };

                // Execute flight search
                var response = await _flightService.SearchFlightsAsync(request);

                if (response.IsSuccess && response.Data != null)
                {
                    // Navigate to results page with search results and request data
                    var navigationParam = new Dictionary<string, object>
                    {
                        { "FlightSearchResults", response.Data },
                        { "FlightSearchRequest", request }
                    };
                    await Shell.Current.GoToAsync(nameof(FlightSearchResultsPage), navigationParam);
                }
                else
                {
                    HasError = true;
                    ErrorMessage = response.Message ?? "No flights found or an error occurred during search.";
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }


        /// <summary>
        /// Swaps the selected origin and destination airports.
        /// Only performs swap if both airports are currently selected.
        /// </summary>
        [RelayCommand]
        public void SwapAirports()
        {
            if (SelectedOriginAirport != null && SelectedDestinationAirport != null)
            {
                var temp = SelectedOriginAirport;
                SelectedOriginAirport = SelectedDestinationAirport;
                SelectedDestinationAirport = temp;
            }
        }

        #endregion
    }
}