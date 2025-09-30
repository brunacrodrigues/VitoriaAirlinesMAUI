using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel;

public partial class MainPageViewModel : BaseViewModel
{
    private readonly IDashboardService _dashboardService;

    [ObservableProperty]
    private string firstName = "Client";

    [ObservableProperty]
    private string profilePictureUrl = string.Empty;

    [ObservableProperty]
    private int upcomingFlightsCount;

    [ObservableProperty]
    private int completedFlightsCount;

    [ObservableProperty]
    private decimal totalSpent;

    [ObservableProperty]
    private NextFlightDto? nextFlight;

    [ObservableProperty]
    private bool hasNextFlight;

    [ObservableProperty]
    private bool hasNoNextFlight = true;


    public MainPageViewModel(IDashboardService dashboardService)
    {
        _dashboardService = dashboardService;
        Title = "Dashboard";
        _ = LoadDashboardAsync();
    }



    partial void OnNextFlightChanged(NextFlightDto? value)
    {
        HasNextFlight = value != null;
        HasNoNextFlight = value == null;
    }




    [RelayCommand]
    public async Task LoadDashboardAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        try
        {
            var response = await _dashboardService.GetDashboardDataAsync();
            if (response.IsSuccess && response.Data != null)
            {
                FirstName = response.Data.FirstName;
                ProfilePictureUrl = response.Data.ProfilePictureUrl ?? string.Empty;
                UpcomingFlightsCount = response.Data.UpcomingFlightsCount;
                CompletedFlightsCount = response.Data.CompletedFlightsCount;
                TotalSpent = response.Data.TotalSpent;
                NextFlight = response.Data.NextUpcomingFlight;
            }
            else
            {
                ErrorMessage = response.Message ?? "Failed to load dashboard data.";
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



    [RelayCommand]
    public async Task GoToSearchAsync()
    {
        await Shell.Current.GoToAsync($"///{nameof(FlightsSearchPage)}");
    }
}