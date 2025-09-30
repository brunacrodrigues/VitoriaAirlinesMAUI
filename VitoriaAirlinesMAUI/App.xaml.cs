using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI
{
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;
        public static IServiceProvider StaticServiceProvider { get; private set; }

        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            StaticServiceProvider = serviceProvider;

            InitializeRouting();


            var token = Preferences.Get("Token", string.Empty);

            // O AppShell é instanciado UMA VEZ
            var appShell = _serviceProvider.GetRequiredService<AppShell>();
            MainPage = appShell; // Define o AppShell como o Root Page

            if (!string.IsNullOrWhiteSpace(token))
            {
                appShell.ConfigureShellForAuthenticatedUser(_serviceProvider);

                _ = Shell.Current.GoToAsync($"//{nameof(MainPage)}");
            }
            else
            {
                appShell.ConfigureShellForAnonymousUser(_serviceProvider);

                _ = Shell.Current.GoToAsync($"//{nameof(WelcomePage)}");
            }

        }


        public IServiceProvider Services => _serviceProvider;


        private void InitializeRouting()
        {

            Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));

            Routing.RegisterRoute(nameof(FlightSearchResultsPage), typeof(FlightSearchResultsPage));
            Routing.RegisterRoute(nameof(SelectSeatPage), typeof(SelectSeatPage));
            Routing.RegisterRoute(nameof(BookingConfirmationPage), typeof(BookingConfirmationPage));
            Routing.RegisterRoute(nameof(PaymentPage), typeof(PaymentPage));
            Routing.RegisterRoute(nameof(BoardingPassPage), typeof(BoardingPassPage));

            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(WelcomePage), typeof(WelcomePage));
            Routing.RegisterRoute(nameof(FlightsSearchPage), typeof(FlightsSearchPage));
            Routing.RegisterRoute(nameof(MainPage), typeof(MainPage));
            Routing.RegisterRoute(nameof(RegistrationPage), typeof(RegistrationPage));
            Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));

        }


        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            // Chamar o handler genérico para processar o URI
            HandleAppLink(uri);
        }


        public void HandleAppLink(Uri uri)
        {

            if (uri.Scheme == "vitoriaairlinesapp" && uri.Host == "resetpassword")
            {
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query.TrimStart('?'));
                var token = query["token"];
                var email = query["email"];
                _ = Shell.Current.GoToAsync($"{nameof(ResetPasswordPage)}?token={token}&email={email}");
            }
            else if (uri.Scheme == "vitoriaairlines" && uri.Host == "app" && uri.AbsolutePath.Contains("/booking/success"))
            {
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query.TrimStart('?'));
                var sessionId = query["sessionId"];

                if (!string.IsNullOrEmpty(sessionId))
                {

                    // Chama a lógica de finalização da reserva no Thread principal
                    MainThread.BeginInvokeOnMainThread(async () =>
                    {
                        try
                        {
                            var bookingService = _serviceProvider.GetService<Services.Interfaces.IBookingService>();
                            if (bookingService != null)
                            {
                                var result = await bookingService.CompleteBookingAsync(sessionId);

                                if (result.IsSuccess && result.Data != null)
                                {
                                    await Shell.Current.GoToAsync($"//{nameof(BoardingPassPage)}?TicketId={result.Data.OutboundTicketId}");
                                }
                                else
                                {
                                    await Shell.Current.DisplayAlert("Error",
                                        result.Message ?? "Failed to complete booking", "OK");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            await Shell.Current.DisplayAlert("Error", $"Payment processing error: {ex.Message}", "OK");
                        }
                    });
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[HANDLE] Invalid deep link format.");
            }
        }
    }
}

