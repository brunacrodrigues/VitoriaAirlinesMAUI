using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using VitoriaAirlinesMAUI.Model;
using VitoriaAirlinesMAUI.Services.Interfaces;

namespace VitoriaAirlinesMAUI.ViewModel;

public partial class FlightsHistoryViewModel : BaseViewModel
{
    private readonly ITicketService _ticketService;

    [ObservableProperty]
    private ObservableCollection<Ticket> tickets = new ObservableCollection<Ticket>();



    public FlightsHistoryViewModel(ITicketService ticketService)
    {
        Title = "My Past Flights";
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

            var response = await _ticketService.GetMyHistoryAsync();

            if (!response.IsSuccess || response.Data is null)
            {
                HasError = true;
                ErrorMessage = response.Message ?? "Failed to load past flights.";
                return;
            }


            foreach (var ticket in response.Data.OrderByDescending(x => x.DepartureUtc))
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
