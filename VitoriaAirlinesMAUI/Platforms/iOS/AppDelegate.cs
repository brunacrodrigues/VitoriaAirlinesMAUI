using Foundation;
using UIKit;

namespace VitoriaAirlinesMAUI
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

        /// <summary>
        /// Handles incoming URLs on iOS (deep links via custom URL schemes).
        /// </summary>
        public override bool OpenUrl(UIApplication app, NSUrl url, NSDictionary options)
        {
            if (url != null)
            {
                var uri = new Uri(url.ToString());

                System.Diagnostics.Debug.WriteLine($"[iOS DEEP LINK] URL received: {uri}");

                if (Microsoft.Maui.Controls.Application.Current is App appInstance)
                {
                    // Call the shared handler on the main thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        appInstance.HandleAppLink(uri);
                    });
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Legacy method for iOS 8 and earlier (optional, for maximum compatibility).
        /// </summary>
        [Export("application:handleOpenURL:")]
        public bool HandleOpenUrl(UIApplication application, NSUrl url)
        {
            return OpenUrl(application, url, new NSDictionary());
        }
    }
}