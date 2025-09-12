using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class FlightsSearchPage : ContentPage
{
    private readonly FlightSearchViewModel _viewModel;

    public FlightsSearchPage(FlightSearchViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();


        if (_viewModel.AvailableAirports.Count == 0 && !_viewModel.IsBusy)
        {
            await _viewModel.LoadAirportsAsync();
        }

    }
}