using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class FlightSearchResultsPage : ContentPage
{
    public FlightSearchResultsPage(FlightSearchResultsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}