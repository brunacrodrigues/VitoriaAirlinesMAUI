using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Web;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for handling password reset logic, managing query parameters and UI bindings.
    /// </summary>
    public partial class ResetPasswordViewModel : BaseViewModel, IQueryAttributable
    {

        private readonly IAuthService _authService;

        /// <summary>
        /// Initializes a new instance of the ResetPasswordViewModel class.
        /// </summary>
        /// <param name="accountService">Service for account-related API operations.</param>
        public ResetPasswordViewModel(IAuthService accountService)
        {
            _authService = accountService;
        }


        /// <summary>
        /// Email address provided via deep link or entered by the user.
        /// </summary>
        [ObservableProperty]
        string email;


        /// <summary>
        /// Reset token decoded from deep link parameter.
        /// </summary>
        [ObservableProperty]
        string token;



        /// <summary>
        /// New password entered by the user.
        /// </summary>
        [ObservableProperty]
        string newPassword;



        /// <summary>
        /// Confirmation of the new password entered by the user.
        /// </summary>
        [ObservableProperty]
        string confirmPassword;



        /// <summary>
        /// Controls visibility of the new password entry field.
        /// </summary>
        [ObservableProperty]
        bool isNewPasswordHidden = true;


        /// <summary>
        /// Controls visibility of the confirm password entry field.
        /// </summary>
        [ObservableProperty]
        bool isConfirmPasswordHidden = true;



        /// <summary>
        /// Gets the name of the icon to display based on the new password visibility state.
        /// Returns "eye-slash.svg" if the password is hidden, or "eye.svg" if visible.
        /// </summary>
        public string NewPasswordToggleIcon => IsNewPasswordHidden ? "eye-slash.svg" : "eye.svg";


        /// <summary>
        /// Gets the name of the icon to display based on the confirm password visibility state.
        /// Returns "eye-slash.svg" if the password is hidden, or "eye.svg" if visible.
        /// </summary>
        public string ConfirmPasswordToggleIcon => IsConfirmPasswordHidden ? "eye-slash.svg" : "eye.svg";



        /// <summary>
        /// Toggles the visibility of the new password field and updates the icon accordingly.
        /// </summary>
        [RelayCommand]
        private void ToggleNewPasswordVisibility()
        {
            IsNewPasswordHidden = !IsNewPasswordHidden;
            OnPropertyChanged(nameof(NewPasswordToggleIcon));
        }


        /// <summary>
        /// Toggles the visibility of the confirm password field and updates the icon accordingly.
        /// </summary>
        [RelayCommand]
        private void ToggleConfirmPasswordVisibility()
        {
            IsConfirmPasswordHidden = !IsConfirmPasswordHidden;
            OnPropertyChanged(nameof(ConfirmPasswordToggleIcon));
        }



        /// <summary>
        /// Applies query parameters received via deep link to populate Email and Token.
        /// Decodes URL-encoded values and Base64Url-decodes the token.
        /// Sets error state if parameters are invalid or missing.
        /// </summary>
        /// <param name="query">Dictionary of query attributes from the URI.</param>
        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            // decodifica email
            Email = query.TryGetValue("email", out var eVal)
                ? HttpUtility.UrlDecode(eVal?.ToString())
                : null;

            // URL-decode
            var b64 = query.TryGetValue("token", out var tVal)
                ? HttpUtility.UrlDecode(tVal?.ToString())
                : null;

            if (!string.IsNullOrWhiteSpace(b64))
            {
                try
                {
                    var bytes = WebEncoders.Base64UrlDecode(b64);
                    Token = Encoding.UTF8.GetString(bytes);
                }
                catch
                {
                    Token = null;
                }
            }

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token))
            {
                ErrorMessage = "Invalid or expired link.";
                HasError = true;
            }
        }



        /// <summary>
        /// Command to submit the password reset request.
        /// Validates input, invokes the account service, and navigates on success.
        /// </summary>
        [RelayCommand]
        private async Task ResetPasswordAsync()
        {
            HasError = false;

            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Token))
            {
                ErrorMessage = "This link is invalid or incomplete. Please request a new password reset email.";
                HasError = true;
                return;
            }


            var model = new ResetPasswordRequest
            {
                Username = Email,
                Token = Token,
                Password = NewPassword,
                ConfirmPassword = ConfirmPassword
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, true))
            {
                ErrorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Invalid input.";
                HasError = true;
                return;
            }

            try
            {
                IsBusy = true;

                var response = await _authService.ResetPasswordAsync(model);

                if (!response.IsSuccess)
                {
                    ErrorMessage = response.Message ?? "Failed to reset password.";
                    HasError = true;
                    return;
                }

                await Shell.Current.DisplayAlert("Success", "Your password has been reset.", "OK");
                //var appShell = (App.Current as App)!.Services.GetRequiredService<AppShell>();
                //Application.Current.MainPage = appShell;
                //await Shell.Current.GoToAsync(nameof(LoginPage));
                await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
