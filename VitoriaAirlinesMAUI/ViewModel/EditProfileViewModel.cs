using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel
{
    /// <summary>
    /// ViewModel responsible for displaying and updating the authenticated customer's profile.
    /// </summary>
    public partial class EditProfileViewModel : BaseViewModel
    {
        private readonly ICountryService _countryService;
        private readonly IProfileService _profileService;
        private readonly IAuthService _authService;


        /// <summary>
        /// Initializes a new instance of the EditProfileViewModel with the provided profile service.
        /// </summary>
        public EditProfileViewModel(
            ICountryService countryService,
            IProfileService profileService,
            IAuthService authService)
        {
            Title = "Edit Profile";
            _countryService = countryService;
            _profileService = profileService;
            _authService = authService;
        }


        /// <summary>
        /// Customer's first name displayed in the form.
        /// </summary>
        [ObservableProperty]
        private string firstName = string.Empty;


        /// <summary>
        /// Customer's last name displayed in the form.
        /// </summary>
        [ObservableProperty]
        private string lastName = string.Empty;


        /// <summary>
        /// Customer's passport number displayed in the form.
        /// </summary>
        [ObservableProperty]
        private string? passportNumber;


        /// <summary>
        /// Selected country ID (nationality).
        /// </summary>
        [ObservableProperty]
        private int? countryId;


        [ObservableProperty]
        private string? currentImageUrlString; // URL vindo da API



        [ObservableProperty]
        private ImageSource? currentImageUrl; // Usado na UI



        /// <summary>
        /// Binary stream of the newly selected profile image.
        /// </summary>
        [ObservableProperty]
        private Stream? profileImageStream;


        /// <summary>
        /// File name of the newly selected profile image.
        /// </summary>
        [ObservableProperty]
        private string? profileImageFileName;


        /// <summary>
        /// Indicates whether the user wants to remove the current image.
        /// </summary>
        [ObservableProperty]
        private bool removeImage;



        /// <summary>
        /// List of countries available for selection.
        /// </summary>
        [ObservableProperty]
        private List<Country> countries = new();



        /// <summary>
        /// Currently selected country object from the list.
        /// </summary>
        [ObservableProperty]
        private Country? selectedCountry;



        /// <summary>
        /// Loads the customer profile from the API and populates the form fields.
        /// </summary>
        [RelayCommand]
        public async Task LoadProfileAsync()
        {
            var token = Preferences.Get("Token", string.Empty);
            if (string.IsNullOrWhiteSpace(token))
            {
                await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
                return;
            }


            IsBusy = true;
            HasError = false;

            try
            {

                // Load countries
                var countryResponse = await _countryService.GetCountriesAsync();
                if (countryResponse.IsSuccess && countryResponse.Data is not null)
                {
                    Countries = countryResponse.Data;
                }


                //Load profile
                var response = await _profileService.GetProfileAsync();

                if (!response.IsSuccess || response.Data is null)
                {
                    ErrorMessage = response.Message ?? "Failed to load profile.";
                    HasError = true;
                    return;
                }

                FirstName = response.Data.FirstName ?? string.Empty;
                LastName = response.Data.LastName ?? string.Empty;
                PassportNumber = response.Data.PassportNumber;
                CountryId = response.Data.CountryId;
                CurrentImageUrlString = response.Data.ProfileImageUrl;
                CurrentImageUrl = ImageSource.FromUri(new Uri(CurrentImageUrlString));


                SelectedCountry = Countries.FirstOrDefault(c => c.Id == CountryId);
            }
            finally
            {
                IsBusy = false;
            }
        }




        /// <summary>
        /// Submits the updated profile to the API after validating the form data.
        /// </summary>
        [RelayCommand]
        private async Task UpdateProfileAsync()
        {
            HasError = false;

            CountryId = SelectedCountry?.Id;

            var model = new UpdateCustomerProfileRequest
            {
                FirstName = FirstName,
                LastName = LastName,
                PassportNumber = PassportNumber,
                CountryId = CountryId,
                RemoveImage = RemoveImage,
                ProfileImageStream = ProfileImageStream,
                ProfileImageFileName = ProfileImageFileName
            };

            var context = new ValidationContext(model);
            var results = new List<ValidationResult>();
            if (!Validator.TryValidateObject(model, context, results, true))
            {
                ErrorMessage = results.FirstOrDefault()?.ErrorMessage ?? "Invalid profile data.";
                HasError = true;
                return;
            }

            IsBusy = true;

            try
            {
                var response = await _profileService.UpdateProfileAsync(model);

                if (!response.IsSuccess)
                {
                    ErrorMessage = response.Message ?? "Failed to update profile.";
                    HasError = true;
                    return;
                }

                await LoadProfileAsync();

                await Shell.Current.DisplayAlert("Success", "Your profile was updated.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            finally
            {
                IsBusy = false;
            }
        }



        /// <summary>
        /// Allows the user to pick a new profile image from their device.
        /// </summary>
        [RelayCommand]
        private async Task SelectImageAsync()
        {
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select Profile Image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result is null)
                    return;

                // Abre o stream original
                var originalStream = await result.OpenReadAsync();

                // Copia para a memória (garante que permanece acessível mesmo após sair da page)
                var memoryStream = new MemoryStream();
                await originalStream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                // Guarda o stream para enviar na API depois (ProfileImageStream pode ser lido diretamente)
                ProfileImageStream = memoryStream;
                ProfileImageFileName = result.FileName;

                // cria novo stream temporário a partir do stream principal
                CurrentImageUrl = ImageSource.FromStream(() =>
                {
                    var previewCopy = new MemoryStream();
                    memoryStream.Position = 0;
                    memoryStream.CopyTo(previewCopy);
                    previewCopy.Position = 0;
                    return previewCopy;
                });

                RemoveImage = false;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"Failed to load image: {ex.Message}", "OK");
            }
        }



        /// <summary>
        /// Marks the profile image to be removed on update.
        /// </summary>
        [RelayCommand]
        private void MarkImageForRemoval()
        {
            ProfileImageStream = null;
            ProfileImageFileName = null;
            CurrentImageUrl = null;
            RemoveImage = true;
        }



        /// <summary>
        /// Navigates the user to the ChangePasswordPage using Shell navigation.
        /// </summary>
        [RelayCommand]
        private async Task GoToChangePasswordAsync()
        {
            await Shell.Current.GoToAsync(nameof(ChangePasswordPage));

        }


        ///// <summary>
        ///// Logs out the current user, clears authentication data, and redirects to login.
        ///// </summary>      
        [RelayCommand]
        private async Task LogoutAsync()
        {
            bool confirm = await Shell.Current.DisplayAlert("Logout", "Do you really want to logout?", "Yes", "Cancel");
            if (!confirm) return;

            _authService.Logout();

            App.SetRootPageBasedOnAuthentication();

        }
    }
}
