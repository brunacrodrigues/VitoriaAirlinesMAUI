using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for displaying flight search results.
    /// Manages outbound and return flight collections, provides UI state properties
    /// based on flight availability, and handles flight selection for round-trip bookings.
    /// Implements reactive programming patterns using MVVM Community Toolkit.
    /// </summary>
    [QueryProperty(nameof(FlightSearchResults), "FlightSearchResults")]
    [QueryProperty(nameof(FlightSearchRequest), "FlightSearchRequest")]
    public partial class FlightSearchResultsViewModel : BaseViewModel
    {
        #region Observable Properties

        /// <summary>
        /// Flight search response containing outbound and return flights collections.
        /// Received as navigation parameter and triggers UI updates when changed.
        /// Nullable to handle scenarios where no search results are available.
        /// </summary>
        [ObservableProperty]
        private FlightSearchResponse? flightSearchResults;


        /// <summary>
        /// Original flight search request parameters that generated the current results.
        /// Received as navigation parameter and used to maintain search context.
        /// Contains criteria such as airports, dates, passengers, and trip type.
        /// </summary>
        [ObservableProperty]
        private FlightSearchRequest? flightSearchRequest;

        #endregion

        #region Collections

        /// <summary>
        /// Observable collection of outbound flights for UI data binding.
        /// Each flight in this collection has an 'IsSelected' property for individual selection state.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<FlightSearchResult> outboundFlights = new();


        /// <summary>
        /// Observable collection of return flights for UI data binding.
        /// Each flight in this collection has an 'IsSelected' property for individual selection state.
        /// Only populated for round-trip searches; empty for one-way trips.
        /// </summary>
        [ObservableProperty]
        private ObservableCollection<FlightSearchResult> returnFlights = new();

        #endregion

        #region Computed Properties - UI State & Selected Flights Access

        /// <summary>
        /// Determines whether outbound flights are available for display.
        /// Uses LINQ Any() method for efficient collection checking.
        /// Controls visibility of outbound flights section in the UI.
        /// </summary>
        public bool HasOutboundFlights => OutboundFlights.Any();


        /// <summary>
        /// Determines whether return flights are available for display.
        /// Safely checks for null FlightSearchResults and ReturnFlights collection.
        /// Controls visibility of return flights section in the UI.
        /// Returns false for one-way trips or when no return flights found.
        /// </summary>
        public bool HasReturnFlights => ReturnFlights.Any();


        /// <summary>
        /// Determines whether any flights (outbound or return) are available.
        /// Combines both flight availability checks using logical OR operation.
        /// Used for overall page state management and content visibility.
        /// </summary>
        public bool HasAnyFlights => HasOutboundFlights || HasReturnFlights;


        /// <summary>
        /// Controls visibility of the "no flights found" message in the UI.
        /// Returns true when no flights are available, false when flights exist.
        /// Inverse logic of HasAnyFlights property for convenient XAML binding.
        /// </summary>
        public bool IsNoFlightsMessageVisible => !HasAnyFlights;


        /// <summary>
        /// Determines if the current search is for a one-way trip.
        /// Returns true if FlightSearchRequest is null or IsRoundTrip is false.
        /// </summary>
        public bool IsOneWayTrip => FlightSearchRequest?.IsRoundTrip != true;


        /// <summary>
        /// Determines if the current search is for a round-trip.
        /// Returns true only if FlightSearchRequest exists and IsRoundTrip is true.
        /// </summary>
        public bool IsRoundTrip => FlightSearchRequest?.IsRoundTrip == true;


        /// <summary>
        /// Gets the currently selected outbound flight.
        /// This property is computed by finding the first flight in the OutboundFlights collection where IsSelected is true.
        /// </summary>
        public FlightSearchResult? CurrentSelectedOutboundFlight => OutboundFlights.FirstOrDefault(f => f.IsSelected);


        /// <summary>
        /// Gets the currently selected return flight.
        /// This property is computed by finding the first flight in the ReturnFlights collection where IsSelected is true.
        /// </summary>
        public FlightSearchResult? CurrentSelectedReturnFlight => ReturnFlights.FirstOrDefault(f => f.IsSelected);


        /// <summary>
        /// Determines if the "Book Selected Flights" button should be visible for round-trip flights.
        /// Returns true only when both an outbound and a return flight are selected for round-trips.
        /// </summary>
        public bool CanBookRoundTripFlights =>
            IsRoundTrip && CurrentSelectedOutboundFlight != null && CurrentSelectedReturnFlight != null;


        /// <summary>
        /// Determines if the "Book Flight" button should be visible for one-way flights.
        /// Returns true only when an outbound flight is selected and it's a one-way trip.
        /// </summary>
        public bool CanBookOneWayFlight =>
            IsOneWayTrip && CurrentSelectedOutboundFlight != null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the FlightSearchResultsViewModel.
        /// Sets the page title for display in navigation bar or page header.
        /// View model will receive FlightSearchResults and FlightSearchRequest 
        /// through QueryProperty attributes during navigation.
        /// </summary>
        public FlightSearchResultsViewModel()
        {
            Title = "Flight Results";
        }

        #endregion

        #region Reactive Methods

        /// <summary>
        /// Handles changes to the FlightSearchResults property.
        /// Automatically called by MVVM Community Toolkit when FlightSearchResults is updated.
        /// Clears existing flights and populates new collections, then updates UI state properties.
        /// </summary>
        /// <param name="value">The new FlightSearchResponse value, can be null if no results</param>
        partial void OnFlightSearchResultsChanged(FlightSearchResponse? value)
        {
            OutboundFlights.Clear();
            ReturnFlights.Clear();
            // Not explicitly clearing SelectedOutboundFlight/SelectedReturnFlight,
            // as new items will have IsSelected=false by default.

            if (value?.OutboundFlights != null)
            {
                foreach (var flight in value.OutboundFlights)
                    OutboundFlights.Add(flight);
            }

            if (value?.ReturnFlights != null)
            {
                foreach (var flight in value.ReturnFlights)
                    ReturnFlights.Add(flight);
            }

            OnPropertyChanged(nameof(HasOutboundFlights));
            OnPropertyChanged(nameof(HasReturnFlights));
            OnPropertyChanged(nameof(HasAnyFlights));
            OnPropertyChanged(nameof(IsNoFlightsMessageVisible));
            // Trigger property changed notifications for computed properties that depend on flight collections.
            UpdateCanBookProperties();
        }


        /// <summary>
        /// Handles changes to the FlightSearchRequest property.
        /// Updates trip-type related properties when the search request changes.
        /// </summary>
        /// <param name="value">The new FlightSearchRequest value, can be null</param>
        partial void OnFlightSearchRequestChanged(FlightSearchRequest? value)
        {
            OnPropertyChanged(nameof(IsOneWayTrip));
            OnPropertyChanged(nameof(IsRoundTrip));
            UpdateCanBookProperties();
        }


        /// <summary>
        /// Helper method to manually trigger property changed notifications for
        /// properties that control booking button visibility and command executability.
        /// </summary>
        private void UpdateCanBookProperties()
        {
            OnPropertyChanged(nameof(CurrentSelectedOutboundFlight));
            OnPropertyChanged(nameof(CurrentSelectedReturnFlight));
            OnPropertyChanged(nameof(CanBookRoundTripFlights));
            OnPropertyChanged(nameof(CanBookOneWayFlight));
            BookFlightsCommand.NotifyCanExecuteChanged(); // Update the state of the booking command button
        }

        #endregion

        #region Commands

        /// <summary>
        /// Command to select an outbound flight.
        /// If the clicked flight is already selected, it is deselected.
        /// Ensures that only one outbound flight is selected at a time within its collection.
        /// </summary>
        /// <param name="flight">The outbound flight to be selected/desselected.</param>
        [RelayCommand]
        private void SelectOutboundFlight(FlightSearchResult flight)
        {
            if (flight == null) return;

            // If this flight is already selected, deselect it
            if (flight.IsSelected)
            {
                flight.IsSelected = false;
            }
            else
            {
                // Deselect all other outbound flights first to ensure single selection
                foreach (var f in OutboundFlights)
                {
                    if (f != flight && f.IsSelected) // If not the current flight and is selected
                    {
                        f.IsSelected = false; // Deselect it
                    }
                }
                // Then, select the current flight
                flight.IsSelected = true;
            }

            UpdateCanBookProperties(); // Re-evaluate the visibility/state of the booking buttons
        }


        /// <summary>
        /// Command to select a return flight.
        /// If the clicked flight is already selected, it is deselected.
        /// Ensures that only one return flight is selected at a time within its collection.
        /// </summary>
        /// <param name="flight">The return flight to be selected/desselected.</param>
        [RelayCommand]
        private void SelectReturnFlight(FlightSearchResult flight)
        {
            if (flight == null) return;

            // If this flight is already selected, deselect it
            if (flight.IsSelected)
            {
                flight.IsSelected = false;
            }
            else
            {
                // Deselect all other return flights first to ensure single selection
                foreach (var f in ReturnFlights)
                {
                    if (f != flight && f.IsSelected) // If not the current flight and is selected
                    {
                        f.IsSelected = false; // Deselect it
                    }
                }
                // Then, select the current flight
                flight.IsSelected = true;
            }

            UpdateCanBookProperties(); // Re-evaluate the visibility/state of the booking buttons
        }




        /// <summary>
        /// Command to initiate the booking process for the selected flights.
        /// Its CanExecute logic ensures the command is enabled only when appropriate flights are selected.
        /// </summary>        
        [RelayCommand(CanExecute = nameof(CanExecuteBookFlights))]
        public async Task BookFlightsAsync()
        {
            if (IsBusy) return;

            IsBusy = true;
            HasError = false;
            ErrorMessage = string.Empty;

            try
            {
                var selectedOutbound = CurrentSelectedOutboundFlight;
                var selectedReturn = CurrentSelectedReturnFlight;

                // Round Trip
                if (IsRoundTrip && selectedOutbound != null && selectedReturn != null)
                {
                    var navigationParam = new Dictionary<string, object>
                    {
                        { "FlightId", selectedOutbound.FlightId },
                        { "SearchRequest", FlightSearchRequest! },
                        { "IsRoundTripContext", true },
                        { "IsOutboundLeg", true },
                        { "ReturnFlightId", selectedReturn.FlightId }
                    };

                    await Shell.Current.GoToAsync(nameof(SelectSeatPage), navigationParam);
                }

                // One Way Trip
                else if (IsOneWayTrip && selectedOutbound != null)
                {
                    var navigationParam = new Dictionary<string, object>
                    {
                        { "FlightId", selectedOutbound.FlightId },
                        { "SearchRequest", FlightSearchRequest! },
                        { "IsRoundTripContext", false },
                        { "IsOutboundLeg", true }
                    };

                    await Shell.Current.GoToAsync(nameof(SelectSeatPage), navigationParam);
                }
                else
                {

                    string message = "Please select an outbound flight";
                    if (IsRoundTrip)
                    {
                        message += " and a return flight";
                    }
                    message += " to proceed.";
                    await Shell.Current.DisplayAlert("Selection Error", message, "OK");
                }
            }
            catch (Exception ex)
            {
                HasError = true;
                ErrorMessage = $"Error during booking: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", ErrorMessage, "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }




        /// <summary>
        /// Method that determines if the BookFlightsCommand can be executed.
        /// Returns true if the flights required for booking (based on trip type) are selected, otherwise false.
        /// Also checks if the ViewModel is currently busy.
        /// </summary>
        /// <returns>True if booking can proceed, false otherwise.</returns>
        private bool CanExecuteBookFlights()
        {
            if (IsBusy) return false;

            if (IsRoundTrip)
                return CurrentSelectedOutboundFlight != null && CurrentSelectedReturnFlight != null;

            // If it's a One-Way trip, only the outbound flight needs to be selected
            return IsOneWayTrip && CurrentSelectedOutboundFlight != null;
        }

        #endregion
    }
}