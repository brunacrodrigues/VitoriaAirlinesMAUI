using CommunityToolkit.Mvvm.Input;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel;

public partial class WelcomeViewModel : BaseViewModel
{
    public WelcomeViewModel()
    {
        Title = "Welcome to Vitoria Airlines";
    }



    [RelayCommand]
    private async Task GoToSearchAsync()
    {
        await Shell.Current.GoToAsync($"//{nameof(FlightsSearchPage)}");
    }



    [RelayCommand]
    private async Task GoToLoginAsync()
    {
        await Shell.Current.GoToAsync($"//{nameof(LoginPage)}");
    }



    [RelayCommand]
    private async Task GoToRegisterAsync()
    {

        await Shell.Current.GoToAsync(nameof(RegistrationPage));
    }
}