using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI.ViewModel;

public partial class UpcomingFlightsViewModel : BaseViewModel
{
    private readonly ITicketService _ticketService;


    [ObservableProperty]
    private ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();


    public UpcomingFlightsViewModel(ITicketService ticketService)
    {
        Title = "My Upcoming Flights";
        _ticketService = ticketService;
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

            var response = await _ticketService.GetMyUpcomingAsync();

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



    [RelayCommand]
    private async Task ViewBoardingPassAsync(object parameter)
    {
        if (parameter is Ticket ticket)
        {
            await Shell.Current.GoToAsync($"{nameof(BoardingPassPage)}?TicketId={ticket.TicketId}");
        }
    }
}
