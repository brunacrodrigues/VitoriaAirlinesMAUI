using CommunityToolkit.Mvvm.ComponentModel;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// Base ViewModel providing common UI-binding properties.
    /// </summary>
    public partial class BaseViewModel : ObservableObject
    {

        /// <summary>
        /// Indicates whether a background operation is running.
        /// </summary>
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsNotBusy))]
        bool isBusy;


        /// <summary>
        /// Indicates whether the ViewModel has an active error.
        /// </summary>
        [ObservableProperty]
        private bool hasError;


        /// <summary>
        /// The error message to be displayed when an error occurs.
        /// </summary>
        [ObservableProperty]
        private string errorMessage;


        /// <summary>
        /// The title of the current page or context.
        /// </summary>
        [ObservableProperty]
        string title;


        /// <summary>
        /// Inverse of IsBusy, useful for enabling/disabling UI elements.
        /// </summary>
        public bool IsNotBusy => !IsBusy;
    }
}
