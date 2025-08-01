using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}