using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class SelectSeatPage : ContentPage
{
    public SelectSeatPage(SelectSeatViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}