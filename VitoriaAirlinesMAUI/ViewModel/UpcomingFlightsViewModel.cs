using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.ViewModel;

public partial class UpcomingFlightsViewModel : BaseViewModel
{
    private readonly ITicketService _flightService;


    [ObservableProperty]
    private ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();


    public UpcomingFlightsViewModel(ITicketService flightService)
    {
        Title = "My Upcoming Flights";
        _flightService = flightService;
    }


    [RelayCommand]
    public async Task LoadAsync()
    {
        if (IsBusy)
            return;

        IsBusy = true;
        HasError = false;

        try
        {
            tickets.Clear();

            var response = await _flightService.GetMyUpcomingAsync();

            if (!response.IsSuccess || response.Data is null)
            {
                HasError = true;
                ErrorMessage = response.Message ?? "Failed to load upcoming flights.";
                return;
            }


            foreach (var ticket in response.Data.OrderBy(x => x.DepartureUtc))
                tickets.Add(ticket);

        }
        catch (Exception ex)
        {
            HasError = true;
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }

    }


    [RelayCommand]
    public async Task RefreshAsync()
    {
        await LoadAsync();
    }
}
