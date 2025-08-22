using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class UpcomingFlightsPage : ContentPage
{
    private readonly UpcomingFlightsViewModel _viewModel;

    public UpcomingFlightsPage(UpcomingFlightsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }




    private async void ContentPage_Loaded(object sender, EventArgs e)
    {
        await _viewModel.LoadAsync();
    }
}