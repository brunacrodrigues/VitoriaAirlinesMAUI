//using CommunityToolkit.Mvvm.ComponentModel;
//using VitoriaAirlinesMAUI.Data.Enums;
//using VitoriaAirlinesMAUI.Model;

//namespace VitoriaAirlinesMAUI.ViewModel
//{
//    public partial class SeatViewModel : ObservableObject
//    {
//        public SeatDetail SeatDetail { get; }

//        [ObservableProperty]
//        [NotifyPropertyChangedFor(nameof(BackgroundColor))]
//        [NotifyPropertyChangedFor(nameof(BorderColor))]
//        [NotifyPropertyChangedFor(nameof(SeatScale))]
//        private bool isSelected;

//        public SeatViewModel(SeatDetail seatDetail)
//        {
//            SeatDetail = seatDetail;
//        }

//        // Propriedades delegadas do modelo
//        public int Id => SeatDetail.Id;
//        public int Row => SeatDetail.Row;
//        public string Letter => SeatDetail.Letter;
//        public SeatClass Class => SeatDetail.Class;
//        public bool IsOccupied => SeatDetail.IsOccupied;

//        // Propriedades computadas para styling
//        public Color BackgroundColor
//        {
//            get
//            {
//                if (IsSelected) return Color.FromArgb("#28a745"); // Verde para selecionado
//                if (IsOccupied) return Color.FromArgb("#666666");  // Cinzento para ocupado

//                return Class switch
//                {
//                    SeatClass.Economy => Color.FromArgb("#007bff"),    // Azul para Economy
//                    SeatClass.Executive => Color.FromArgb("#ffc107"),  // Amarelo para Executive
//                    _ => Color.FromArgb("#333333")                     // Cor padrão
//                };
//            }
//        }

//        public Color BorderColor
//        {
//            get
//            {
//                if (IsSelected) return Colors.White;
//                if (IsOccupied) return Color.FromArgb("#999999");
//                return Colors.Transparent;
//            }
//        }

//        public double SeatOpacity => IsOccupied ? 0.6 : 1.0;
//        public double SeatScale => IsSelected ? 1.1 : 1.0;
//        public bool IsInputTransparent => IsOccupied;
//    }
//}
using CommunityToolkit.Mvvm.ComponentModel;
using VitoriaAirlinesMAUI.Data.Enums;
using VitoriaAirlinesMAUI.Model;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel wrapper for SeatDetail model that provides UI-specific properties and visual state management.
    /// Implements observable properties for seat selection state and computed styling properties
    /// for dynamic appearance based on seat status, class, and selection state.
    /// Used in seat selection grids to enable interactive seat picking with visual feedback.
    /// </summary>
    public partial class SeatViewModel : ObservableObject
    {
        #region Properties

        /// <summary>
        /// Reference to the underlying SeatDetail model containing seat data.
        /// This property holds the core seat information (ID, row, letter, class, availability)
        /// and serves as the data source for all delegated properties.
        /// Immutable after construction to maintain data integrity.
        /// </summary>
        public SeatDetail SeatDetail { get; }


        /// <summary>
        /// Observable property indicating whether this seat is currently selected by the user.
        /// When changed, automatically triggers property change notifications for dependent UI styling properties
        /// (BackgroundColor, BorderColor, SeatScale) to provide immediate visual feedback.
        /// Used by the UI to track user selection state and apply appropriate visual styling.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(BackgroundColor))]
        [NotifyPropertyChangedFor(nameof(BorderColor))]
        [NotifyPropertyChangedFor(nameof(SeatScale))]
        private bool isSelected;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new SeatViewModel instance wrapping the provided SeatDetail model.
        /// The seat starts in an unselected state (IsSelected = false).
        /// </summary>
        /// <param name="seatDetail">The underlying seat data model to wrap. Cannot be null.</param>
        public SeatViewModel(SeatDetail seatDetail)
        {
            SeatDetail = seatDetail;
        }

        #endregion

        #region Delegated Properties

        /// <summary>
        /// Gets the unique identifier of the seat from the underlying model.
        /// Delegates to SeatDetail.Id for data binding and identification purposes.
        /// </summary>
        public int Id => SeatDetail.Id;


        /// <summary>
        /// Gets the row number of the seat from the underlying model.
        /// Used for display purposes and seat location identification.
        /// Delegates to SeatDetail.Row.
        /// </summary>
        public int Row => SeatDetail.Row;


        /// <summary>
        /// Gets the seat letter (A, B, C, etc.) from the underlying model.
        /// Combined with Row to create the full seat identifier (e.g., "12A").
        /// Delegates to SeatDetail.Letter.
        /// </summary>
        public string Letter => SeatDetail.Letter;


        /// <summary>
        /// Gets the seat class (Economy, Executive) from the underlying model.
        /// Used to determine seat pricing tier and visual styling.
        /// Delegates to SeatDetail.Class.
        /// </summary>
        public SeatClass Class => SeatDetail.Class;


        /// <summary>
        /// Gets the seat availability status from the underlying model.
        /// When true, the seat is already taken and cannot be selected.
        /// Affects visual styling and interaction behavior.
        /// Delegates to SeatDetail.IsOccupied.
        /// </summary>
        public bool IsOccupied => SeatDetail.IsOccupied;


        #endregion

        #region Computed Styling Properties

        /// <summary>
        /// Computed property that returns the appropriate background color based on seat state.
        /// Priority order: Selected (green) > Occupied (gray) > Class-based colors.
        /// Updates automatically when IsSelected changes due to NotifyPropertyChangedFor attribute.
        /// 
        /// Color scheme:
        /// - Selected: Green (#28a745) - highest priority
        /// - Occupied: Gray (#666666) - second priority  
        /// - Economy: Blue (#007bff) - available economy seats
        /// - Executive: Yellow (#ffc107) - available executive seats
        /// - Default: Dark gray (#333333) - fallback color
        /// </summary>
        public Color BackgroundColor
        {
            get
            {
                if (IsSelected) return Color.FromArgb("#28a745"); // Verde para selecionado
                if (IsOccupied) return Color.FromArgb("#666666");  // Cinzento para ocupado
                return Class switch
                {
                    SeatClass.Economy => Color.FromArgb("#007bff"),    // Azul para Economy
                    SeatClass.Executive => Color.FromArgb("#ffc107"),  // Amarelo para Executive
                    _ => Color.FromArgb("#333333")                     // Cor padrão
                };
            }
        }

        /// <summary>
        /// Computed property that returns the appropriate border color based on seat state.
        /// Provides visual emphasis for selected seats and subtle styling for occupied seats.
        /// Updates automatically when IsSelected changes due to NotifyPropertyChangedFor attribute.
        /// 
        /// Color scheme:
        /// - Selected: White border for high contrast and emphasis
        /// - Occupied: Light gray border for subtle disabled appearance
        /// - Available: Transparent border for clean look
        /// </summary>
        public Color BorderColor
        {
            get
            {
                if (IsSelected) return Colors.White;
                if (IsOccupied) return Color.FromArgb("#999999");
                return Colors.Transparent;
            }
        }

        /// <summary>
        /// Computed property that returns the opacity level for the seat based on availability.
        /// Occupied seats are rendered with reduced opacity to indicate they're unavailable.
        /// Available seats use full opacity for clear visibility and interaction.
        /// Provides visual cue about seat availability without completely hiding occupied seats.
        /// </summary>
        public double SeatOpacity => IsOccupied ? 0.6 : 1.0;


        /// <summary>
        /// Computed property that returns the scale factor for visual selection feedback.
        /// Selected seats are slightly enlarged to provide immediate visual confirmation.
        /// Updates automatically when IsSelected changes due to NotifyPropertyChangedFor attribute.
        /// Creates a subtle animation effect when combined with UI transitions.
        /// 
        /// Scale values:
        /// - Selected: 1.1 (110% size) for emphasis
        /// - Unselected: 1.0 (normal size)
        /// </summary>
        public double SeatScale => IsSelected ? 1.1 : 1.0;


        /// <summary>
        /// Computed property that determines whether the seat should ignore touch input.
        /// Occupied seats are made non-interactive to prevent selection attempts.
        /// Available seats remain interactive for normal selection behavior.
        /// Used by UI controls to disable touch/click events on unavailable seats.
        /// </summary>
        public bool IsInputTransparent => IsOccupied;

        #endregion
    }
}