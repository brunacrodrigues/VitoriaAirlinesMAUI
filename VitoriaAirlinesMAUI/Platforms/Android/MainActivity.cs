using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;

namespace VitoriaAirlinesMAUI
{
    /// <summary>
    /// Main Android activity for the MAUI application. 
    /// Handles app launch and deep link intents via the custom URI scheme.
    /// </summary>
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true,
              LaunchMode = LaunchMode.SingleTop,
              ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
                                     ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
                                     ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
        DataScheme = "vitoriaairlinesapp",
        DataHost = "resetpassword")]

    [IntentFilter(new[] { Intent.ActionView },
    Categories = new[] { Intent.CategoryDefault, Intent.CategoryBrowsable },
    DataScheme = "vitoriaairlines",
    DataHost = "app",
    DataPathPrefix = "/booking/success")]
    public class MainActivity : MauiAppCompatActivity
    {
        /// <summary>
        /// Called when the activity receives a new intent while running in single-top mode.
        /// Processes deep link URIs and forwards them to the shared App for navigation.
        /// </summary>
        /// <param name="intent">The new Intent containing potential deep link data.</param>
        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            if (intent?.Data != null)
            {
                var uri = new Uri(intent.Data.ToString());

                if (Microsoft.Maui.Controls.Application.Current is App app)
                {
                    // Log the received URI for debugging
                    System.Diagnostics.Debug.WriteLine($"[INTENT] URI received: {uri}");
                    app.HandleAppLink(uri);
                }
                else
                {
                    // Log if the current Application instance is not ready or not of type Ap
                    System.Diagnostics.Debug.WriteLine("[INTENT] Application.Current is null or not of type App.");
                }
            }
        }


        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Request storage permissions
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M)
            {
                if (CheckSelfPermission(Manifest.Permission.WriteExternalStorage) != Permission.Granted)
                {
                    RequestPermissions(new string[] {
                Manifest.Permission.WriteExternalStorage,
                Manifest.Permission.ReadExternalStorage
            }, 1);
                }
            }
        }
    }
}
