using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class PaymentPage : ContentPage
{
    private readonly PaymentViewModel _viewModel;


    private const string SuccessRedirectHost = "vitoriaairlines";
    private const string SuccessRedirectPath = "app/booking/success";


    private bool isBookingInProgress = false;

    public PaymentPage(PaymentViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        this.BindingContext = viewModel;
    }

    private async void WebView_Navigating(object sender, WebNavigatingEventArgs e)
    {
        if (isBookingInProgress)
        {
            if (e.Url.Contains($"{SuccessRedirectHost}://{SuccessRedirectPath}"))
            {
                e.Cancel = true;
            }
            return;
        }


        if (_viewModel == null)
        {
            e.Cancel = true;
            await Shell.Current.DisplayAlert("Error", "Payment ViewModel not initialized.", "OK");
            await Shell.Current.GoToAsync("..");
            return;
        }

        var uri = new Uri(e.Url);


        if (uri.Scheme == SuccessRedirectHost &&
            uri.Host == "app" &&
            uri.AbsolutePath.Contains("/booking/success"))
        {

            e.Cancel = true;

            isBookingInProgress = true;

            try
            {
                if (_viewModel.CompleteBookingCommand.CanExecute(null))
                {
                    await _viewModel.CompleteBookingCommand.ExecuteAsync(null);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WebView_Navigating] Error calling CompleteBooking: {ex.Message}");

            }
            finally
            {

                isBookingInProgress = false;
            }
        }
        else if (e.Url.Contains("payment-cancel")) // Verifica o URL de cancelamento da API
        {
            e.Cancel = true;
            await Shell.Current.DisplayAlert("Payment Canceled", "Your payment was canceled. Please try again.", "OK");
            await Shell.Current.GoToAsync("..");
        }
        else
        {
            // Se o URL for o do Stripe, mostra o ActivityIndicator
            _viewModel.IsBusy = true;
        }
    }


    protected override void OnDisappearing()
    {
        base.OnDisappearing();
        // Garante que o indicador para ao sair
        if (_viewModel != null)
        {
            _viewModel.IsBusy = false;
        }
        // Limpar a flag ao sair da página para que possa ser usada novamente
        isBookingInProgress = false;
    }
}