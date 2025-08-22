using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for handling user login logic and UI binding.
    /// </summary>
    public partial class LoginViewModel : BaseViewModel
    {
        private readonly IAuthService _authService;


        /// <summary>
        /// Initializes a new instance of the LoginViewModel with injected dependencies.
        /// </summary>
        /// <param name="accountService">The service responsible for account-related API operations.</param>
        public LoginViewModel(IAuthService authtService)
        {
            Title = "Login";
            _authService = authtService;
            LoadSavedCredentials();
        }


        /// <summary>
        /// The email address entered by the user.
        /// </summary>
        [ObservableProperty]
        string email;



        /// <summary>
        /// The password entered by the user.
        /// </summary>
        [ObservableProperty]
        string password;



        /// <summary>
        /// Indicates whether the user chose to remember their credentials.
        /// </summary>
        [ObservableProperty]
        bool rememberMe;



        /// <summary>
        /// Indicates whether the password field is currently hidden (masked).
        /// This property controls the <c>IsPassword</c> state of the Entry.
        /// </summary>
        [ObservableProperty]
        bool isPasswordHidden = true;



        /// <summary>
        /// Gets the name of the icon to display based on the current password visibility state.
        /// Returns "eye-slash.svg" if the password is hidden, or "eye.svg" if visible.
        /// </summary>
        public string PasswordToggleIcon => IsPasswordHidden ? "eye-slash.svg" : "eye.svg";



        /// <summary>
        /// Toggles the visibility of the password field and updates the icon accordingly.
        /// </summary>
        [RelayCommand]
        private void TogglePasswordVisibility()
        {
            IsPasswordHidden = !IsPasswordHidden;
            OnPropertyChanged(nameof(PasswordToggleIcon));
        }



        /// <summary>
        /// Command that triggers the login process.
        /// </summary>
        [RelayCommand]
        private async Task LoginAsync()
        {
            if (IsBusy)
                return;

            HasError = false;

            if (!ValidateInputs())
                return;

            try
            {
                IsBusy = true;

                var response = await _authService.LoginAsync(new LoginRequest
                {
                    Username = Email,
                    Password = Password
                });

                if (!response.IsSuccess || response.Data is null)
                {
                    ErrorMessage = response.Message ?? "Login failed.";
                    HasError = true;
                    return;
                }


                if (response.Data.Role != "Customer")
                {
                    ErrorMessage = "Only customers can use this application.";
                    HasError = true;
                    return;
                }


                Preferences.Set("Token", response.Data.Token);
                Preferences.Set("UserRole", response.Data.Role);


                if (RememberMe)
                {
                    Preferences.Set("SavedEmail", Email);
                    Preferences.Set("SavedPassword", Password); // Consider SecureStorage for production
                }
                else
                {
                    Preferences.Remove("SavedEmail");
                    Preferences.Remove("SavedPassword");
                }

                var appShell = ((App)App.Current).Services.GetRequiredService<AppShell>();
                Application.Current.MainPage = appShell;

                await Task.Delay(100);

                appShell.ConfigureShellForAuthenticatedUser(((App)App.Current).Services);

                await Shell.Current.GoToAsync($"//{nameof(MainPage)}");

            }
            catch (Exception ex)
            {
                ErrorMessage = $"An unexpected error occurred.{ex.Message}";
                HasError = true;
            }
            finally
            {
                IsBusy = false;
            }
        }


        /// <summary>
        /// Navigates the user to the ForgotPasswordPage using Shell routing.
        /// </summary>
        [RelayCommand]
        private async Task NavigateToForgotPasswordAsync()
        {
            //await Shell.Current.GoToAsync(nameof(ForgotPasswordPage));
            var sp = ((App)Application.Current).Services;
            var page = sp.GetRequiredService<ForgotPasswordPage>();
            await Shell.Current.Navigation.PushAsync(page);

        }



        /// <summary>
        /// Loads saved credentials from Preferences if "Remember Me" was previously enabled.
        /// </summary>
        private void LoadSavedCredentials()
        {
            if (Preferences.ContainsKey("SavedEmail") && Preferences.ContainsKey("SavedPassword"))
            {
                Email = Preferences.Get("SavedEmail", string.Empty);
                Password = Preferences.Get("SavedPassword", string.Empty);
                RememberMe = true;
            }
        }


        /// <summary>
        /// Validates user inputs.
        /// </summary>
        /// <returns></returns>
        private bool ValidateInputs()
        {
            if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Email and password are required.";
                HasError = true;
                return false;
            }

            return true;
        }

    }
}
