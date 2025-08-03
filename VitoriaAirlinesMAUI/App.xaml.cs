using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI
{
    /// <summary>
    /// The main application class responsible for initializing and configuring the app,
    /// including page routing and deep link handling.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// Sets up the shell and registers application routes.
        /// </summary>
        public App()
        {
            InitializeComponent();

            MainPage = new AppShell();
            Routing.RegisterRoute("ResetPasswordPage", typeof(ResetPasswordPage));
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
                    await Shell.Current.GoToAsync($"ResetPasswordPage?token={token}&email={email}");

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
                    var route = $"ResetPasswordPage?token={token}&email={email}";
                    System.Diagnostics.Debug.WriteLine($"[HANDLE] Navigating to: {route}");
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
