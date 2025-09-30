using CommunityToolkit.Mvvm.ComponentModel;

namespace VitoriaAirlinesMAUI.ViewModel;

public partial class AboutViewModel : BaseViewModel
{
    [ObservableProperty]
    private string appVersion = "v1.0.0";


    [ObservableProperty]
    private string appDate = "03-10-2025";


    [ObservableProperty]
    private string authorName = "Bruna Rodrigues";


    [ObservableProperty]
    private string appDescription = "Mobile Application for the Vitoria Airlines web-based ticketing system.";


    public AboutViewModel()
    {
        Title = "About Vitoria Airlines";
    }

}