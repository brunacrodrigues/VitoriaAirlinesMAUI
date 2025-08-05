using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI
{
    /// <summary>
    /// The main application class responsible for initializing and configuring the app,
    /// including page routing and deep link handling.
    /// </summary>
    public partial class App : Application
    {
        private readonly IServiceProvider _serviceProvider;

        public static IServiceProvider StaticServiceProvider { get; private set; }

        /// <summary>
        /// Initializes a new instance of the App class.
        /// Sets up the shell and initial navigation logic.
        /// </summary>
        public App(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            _serviceProvider = serviceProvider;
            StaticServiceProvider = serviceProvider;

            InitializeRouting();

            SetRootPageBasedOnAuthentication();

        }


        /// <summary>
        /// Provides access to the application's configured services for dependency injection.
        /// </summary>
        public IServiceProvider Services => _serviceProvider;



        /// <summary>
        /// Registers any custom routes used in the application.
        /// </summary>
        private void InitializeRouting()
        {
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(ForgotPasswordPage), typeof(ForgotPasswordPage));
            Routing.RegisterRoute(nameof(ResetPasswordPage), typeof(ResetPasswordPage));
        }



        /// <summary>
        /// Sets the initial root page based on whether a valid authentication token is stored.
        /// If a token exists, navigates to the authenticated user shell and home page.
        /// Otherwise, navigates to the login page inside a navigation stack.
        /// </summary>
        public static void SetRootPageBasedOnAuthentication()
        {

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                var token = Preferences.Get("Token", string.Empty);

                if (!string.IsNullOrWhiteSpace(token))
                {
                    var appShell = StaticServiceProvider.GetRequiredService<AppShell>();
                    Application.Current.MainPage = appShell;

                    await Task.Delay(100);

                    appShell.ConfigureShellForAuthenticatedUser(StaticServiceProvider);

                    await Shell.Current.GoToAsync($"//{nameof(MainPage)}");
                }
                else
                {
                    var loginPage = StaticServiceProvider.GetRequiredService<LoginPage>();
                    Application.Current.MainPage = new NavigationPage(loginPage);
                }
            });
        }



        /// <summary>
        /// Called when the application receives an App Link (deep link) request.
        /// Processes links with the custom scheme to navigate to the appropriate page.
        /// </summary>
        /// <param name="uri">The URI representing the App Link request.</param>
        protected override void OnAppLinkRequestReceived(Uri uri)
        {
            base.OnAppLinkRequestReceived(uri);

            if (uri.Scheme == "vitoriaairlinesapp" && uri.Host == "resetpassword")
            {
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
                var token = query["token"];
                var email = query["email"];

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    //await Shell.Current.GoToAsync($"ResetPasswordPage?token={token}&email={email}");
                    await Shell.Current.GoToAsync($"{nameof(ResetPasswordPage)}?token={token}&email={email}");

                });
            }
        }



        /// <summary>
        /// Handles custom deep link URIs to navigate within the application.
        /// This method can be called manually to process links outside of App Link events.
        /// </summary>
        /// <param name="uri">The URI containing deep link information.</param>
        public void HandleAppLink(Uri uri)
        {
            if (uri.Scheme == "vitoriaairlinesapp" && uri.Host == "resetpassword")
            {
                var query = System.Web.HttpUtility.ParseQueryString(uri.Query.TrimStart('?'));
                var token = query["token"];
                var email = query["email"];

                MainThread.BeginInvokeOnMainThread(async () =>
                {
                    await Task.Delay(500);
                    var route = $"{nameof(ResetPasswordPage)}?token={token}&email={email}";
                    await Shell.Current.GoToAsync(route);

                });
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("[HANDLE] Invalid deep link format.");
            }
        }
    }
}
