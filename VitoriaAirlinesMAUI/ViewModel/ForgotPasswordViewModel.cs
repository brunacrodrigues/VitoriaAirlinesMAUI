using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for handling forgot password logic.
    /// </summary>
    public partial class ForgotPasswordViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;


        /// <summary>
        /// Initializes a new instance of the ForgotPasswordViewModel class.
        /// </summary>
        /// <param name="authService">The service responsible for account-related API operations.</param>
        public ForgotPasswordViewModel(IAuthService authService)
        {
            Title = "Forgot Password";
            _authService = authService;
        }


        /// <summary>
        /// Gets or sets the email address entered by the user for password recovery.
        /// </summary>
        [ObservableProperty]
        string email;



        /// <summary>
        /// Command that sends a password recovery email to the specified address.
        /// Validates the email format and displays UI feedback on success or failure.
        /// </summary>
        [RelayCommand]
        private async Task SendRecoveryEmailAsync()
        {
            HasError = false;

            var request = new RecoverPasswordRequest { Email = Email };

            var context = new ValidationContext(request);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(request, context, results, true))
            {
                ErrorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Invalid input.";
                HasError = true;
                return;
            }

            try
            {
                IsBusy = true;

                var response = await _authService.ForgotPasswordAsync(request);

                if (!response.IsSuccess)
                {
                    ErrorMessage = response.Message ?? "Failed to send recovery email.";
                    HasError = true;
                    return;
                }

                await Shell.Current.DisplayAlert("Success", "Check your email for password reset instructions.", "OK");

                //await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");

            }
            finally
            {
                IsBusy = false;
            }
        }



        /// <summary>
        /// Command that opens the Yopmail website in the device's default browser.
        /// </summary>
        [RelayCommand]
        private async Task OpenYopmailAsync()
        {
            try
            {
                var uri = new Uri("https://yopmail.com");
                //await Launcher.OpenAsync(uri);
                await Browser.OpenAsync(uri, BrowserLaunchMode.SystemPreferred);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error opening Yopmail: {ex.Message}");
                await Shell.Current.DisplayAlert("Error", "Could not open Yopmail. Make sure you have a web browser installed.", "OK");
            }
        }
    }
}
