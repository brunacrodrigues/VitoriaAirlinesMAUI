using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class AboutPage : ContentPage
{
    public AboutPage(AboutViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}