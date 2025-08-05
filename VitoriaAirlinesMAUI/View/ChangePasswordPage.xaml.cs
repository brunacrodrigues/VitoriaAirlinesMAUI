using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class ChangePasswordPage : ContentPage
{
    public ChangePasswordPage(ChangePasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}