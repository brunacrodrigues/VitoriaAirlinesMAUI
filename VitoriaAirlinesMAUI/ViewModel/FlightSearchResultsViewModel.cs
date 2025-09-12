using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for displaying flight search results.
    /// Manages outbound and return flight collections, provides UI state properties
    /// based on flight availability, and handles navigation parameter binding.
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

        #region Computed Properties - Collections

        /// <summary>
        /// Observable collection of outbound flights for UI data binding.
        /// Automatically creates a new collection from FlightSearchResults.OutboundFlights.
        /// Returns empty collection if no outbound flights are available or FlightSearchResults is null.
        /// Used for binding to CollectionView or ListView controls in XAML.
        /// </summary>
        public ObservableCollection<FlightSearchResult> OutboundFlights =>
            new(FlightSearchResults?.OutboundFlights ?? new List<FlightSearchResult>());


        /// <summary>
        /// Observable collection of return flights for UI data binding.
        /// Automatically creates a new collection from FlightSearchResults.ReturnFlights.
        /// Returns empty collection if no return flights are available or FlightSearchResults is null.
        /// Only populated for round-trip searches; empty for one-way trips.
        /// </summary>
        public ObservableCollection<FlightSearchResult> ReturnFlights =>
            new(FlightSearchResults?.ReturnFlights ?? new List<FlightSearchResult>());

        #endregion

        #region Computed Properties - UI State

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
        public bool HasReturnFlights => FlightSearchResults?.ReturnFlights?.Any() ?? false;


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

        #region Reactive Methods (Property Change Handlers)

        /// <summary>
        /// Handles changes to the FlightSearchResults property.
        /// Automatically called by MVVM Community Toolkit when FlightSearchResults is updated.
        /// Triggers property change notifications for all dependent UI state properties
        /// to ensure data binding updates correctly when flight data is loaded or modified.
        /// </summary>
        /// <param name="value">The new FlightSearchResponse value, can be null if no results</param>
        partial void OnFlightSearchResultsChanged(FlightSearchResponse? value)
        {
            // Notify UI that flight collections have potentially changed
            OnPropertyChanged(nameof(OutboundFlights));
            OnPropertyChanged(nameof(ReturnFlights));

            // Notify UI that availability flags have potentially changed
            OnPropertyChanged(nameof(HasOutboundFlights));
            OnPropertyChanged(nameof(HasReturnFlights));
            OnPropertyChanged(nameof(HasAnyFlights));

            // Notify UI that visibility states have potentially changed
            OnPropertyChanged(nameof(IsNoFlightsMessageVisible));
        }

        #endregion
    }
}


