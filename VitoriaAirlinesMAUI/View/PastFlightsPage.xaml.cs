using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class PastFlightsPage : ContentPage
{
    private readonly FlightsHistoryViewModel _viewModel;


    public PastFlightsPage(FlightsHistoryViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }

    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await _viewModel.LoadAsync();
    }
}