using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for changing the password of the authenticated user.
    /// </summary>
    public partial class ChangePasswordViewModel : BaseViewModel
    {
        private readonly IProfileService _profileService;


        /// <summary>
        /// Initializes a new instance of the ChangePasswordViewModel class.
        /// </summary>
        /// <param name="profileService">The service used to change the password.</param>
        public ChangePasswordViewModel(IProfileService profileService)
        {
            Title = "Change Password";
            _profileService = profileService;
        }



        /// <summary>
        /// Current password entered by the user.
        /// </summary>
        [ObservableProperty]
        private string currentPassword;



        /// <summary>
        /// New password the user wants to set.
        /// </summary>
        [ObservableProperty]
        private string newPassword;



        /// <summary>
        /// Confirmation of the new password entered by the user.
        /// </summary>
        [ObservableProperty]
        private string confirmPassword;



        /// <summary>
        /// Controls visibility of the current password entry field.
        /// </summary>
        [ObservableProperty]
        private bool isCurrentPasswordHidden = true;



        /// <summary>
        /// Controls visibility of the new password entry field.
        /// </summary>
        [ObservableProperty]
        private bool isNewPasswordHidden = true;



        /// <summary>
        /// Controls visibility of the confirm password entry field.
        /// </summary>
        [ObservableProperty]
        private bool isConfirmPasswordHidden = true;



        /// <summary>
        /// Gets the name of the icon to display based on current password visibility state.
        /// </summary>
        public string CurrentPasswordToggleIcon => IsCurrentPasswordHidden ? "eye_slash.svg" : "eye.svg";



        /// <summary>
        /// Gets the name of the icon to display based on new password visibility state.
        /// </summary>
        public string NewPasswordToggleIcon => IsNewPasswordHidden ? "eye_slash.svg" : "eye.svg";



        /// <summary>
        /// Gets the name of the icon to display based on confirm password visibility state.
        /// </summary>
        public string ConfirmPasswordToggleIcon => IsConfirmPasswordHidden ? "eye_slash.svg" : "eye.svg";





        /// <summary>
        /// Toggles visibility of the current password field.
        /// </summary>
        [RelayCommand]
        private void ToggleCurrentPasswordVisibility()
        {
            IsCurrentPasswordHidden = !IsCurrentPasswordHidden;
            OnPropertyChanged(nameof(CurrentPasswordToggleIcon));
        }



        /// <summary>
        /// Toggles visibility of the new password field.
        /// </summary>
        [RelayCommand]
        private void ToggleNewPasswordVisibility()
        {
            IsNewPasswordHidden = !IsNewPasswordHidden;
            OnPropertyChanged(nameof(NewPasswordToggleIcon));
        }



        /// <summary>
        /// Toggles visibility of the confirm password field.
        /// </summary>
        [RelayCommand]
        private void ToggleConfirmPasswordVisibility()
        {
            IsConfirmPasswordHidden = !IsConfirmPasswordHidden;
            OnPropertyChanged(nameof(ConfirmPasswordToggleIcon));
        }



        /// <summary>
        /// Submits the password change request after validating the input.
        /// </summary>
        [RelayCommand]
        private async Task SubmitAsync()
        {
            HasError = false;

            var model = new ChangePasswordRequest
            {
                OldPassword = CurrentPassword,
                NewPassword = NewPassword,
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

                var response = await _profileService.ChangePasswordAsync(model);

                if (!response.IsSuccess)
                {
                    ErrorMessage = response.Message ?? "Failed to change password.";
                    HasError = true;
                    return;
                }

                await Shell.Current.DisplayAlert("Success", "Your password was updated.", "OK");
                await Shell.Current.GoToAsync("..");

                CurrentPassword = string.Empty;
                NewPassword = string.Empty;
                ConfirmPassword = string.Empty;

            }
            finally
            {

                IsBusy = false;
            }
        }
    }
}
