using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class BookingConfirmationPage : ContentPage
{
    public BookingConfirmationPage(BookingConfirmationViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}