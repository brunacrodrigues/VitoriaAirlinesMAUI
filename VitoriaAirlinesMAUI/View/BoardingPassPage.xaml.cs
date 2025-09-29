using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class BoardingPassPage : ContentPage
{
    public BoardingPassPage(BoardingPassViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}