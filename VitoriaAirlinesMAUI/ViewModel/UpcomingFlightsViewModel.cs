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
        await LoadDataAsync();
    }



    [RelayCommand]
    public async Task RefreshAsync()
    {
        await LoadDataAsync();
    }



    private async Task LoadDataAsync()
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
    private async Task ViewBoardingPassAsync(object parameter)
    {
        if (parameter is Ticket ticket)
        {
            await Shell.Current.GoToAsync($"{nameof(BoardingPassPage)}?TicketId={ticket.TicketId}");
        }
    }



    [RelayCommand]
    private async Task CancelFlightAsync(Ticket ticket)
    {
        if (ticket == null) return;

        bool confirm = await Shell.Current.DisplayAlert("Confirm Cancellation",
            $"Are you sure you want to cancel your ticket for flight {ticket.FlightNumber}?", "Yes, Cancel", "No");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            HasError = false;

            var response = await _ticketService.CancelTicketAsync(ticket.TicketId);

            if (response.IsSuccess)
            {
                await Shell.Current.DisplayAlert("Success", response.Message ?? "Ticket successfully canceled and refund initiated.", "OK");


                IsBusy = false;
                await LoadDataAsync();
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", response.Message ?? "Failed to cancel ticket. Please check the 24-hour cutoff time.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"An unexpected error occurred: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
