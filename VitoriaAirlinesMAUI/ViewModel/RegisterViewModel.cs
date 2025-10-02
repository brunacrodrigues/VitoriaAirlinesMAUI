using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Net.Mail;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel;

public partial class RegisterViewModel : BaseViewModel
{
    private readonly IAuthService _authService;

    // Propriedades para Binding
    [ObservableProperty]
    private string firstName = string.Empty;

    [ObservableProperty]
    private string lastName = string.Empty;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;


    // Propriedades para controlo da UI
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(PasswordToggleIcon))]
    private bool isPasswordHidden = true;

    public string PasswordToggleIcon => IsPasswordHidden ? "eye_slash.svg" : "eye.svg";


    public RegisterViewModel(IAuthService authService)
    {
        _authService = authService;
        Title = "Create New Account";
    }



    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        HasError = false;

        if (!ValidateInputs())
        {
            return;
        }

        IsBusy = true;

        try
        {
            var request = new RegisterNewUserRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                Username = Email,
                Password = Password
            };

            var response = await _authService.RegisterAsync(request);

            if (response.IsSuccess)
            {
                await Shell.Current.DisplayAlert("Registration Successful",
                    "Account created! Please check your email to confirm your account before logging in.", "OK");


                await Shell.Current.GoToAsync($"///{nameof(LoginPage)}");
            }
            else
            {
                ErrorMessage = response.Message ?? "Registration failed. Please try again.";
                HasError = true;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An unexpected error occurred: {ex.Message}";
            HasError = true;
        }
        finally
        {
            IsBusy = false;
        }
    }



    private bool ValidateInputs()
    {
        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName) ||
            string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password) ||
            string.IsNullOrWhiteSpace(ConfirmPassword))
        {
            ErrorMessage = "All fields are required.";
            HasError = true;
            return false;
        }

        try
        {
            // Tenta criar um MailAddress. Se falhar, o formato é inválido.
            _ = new MailAddress(Email);
        }
        catch (FormatException)
        {
            ErrorMessage = "Please enter a valid email address.";
            HasError = true;
            return false;
        }

        if (Password.Length < 8)
        {
            ErrorMessage = "Password must be at least 8 characters long.";
            HasError = true;
            return false;
        }

        if (Password != ConfirmPassword)
        {
            ErrorMessage = "Password and confirmation do not match.";
            HasError = true;
            return false;
        }

        return true;
    }



    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordHidden = !IsPasswordHidden;
    }
}