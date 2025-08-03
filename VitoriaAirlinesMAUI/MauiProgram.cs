using Microsoft.Extensions.Logging;
using VitoriaAirlinesMAUI.Services;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;
using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Outfit-Regular.ttf", "OutfitRegular");
                    fonts.AddFont("Outfit-Bold.ttf", "OutfitBold");
                    fonts.AddFont("Outfit-SemiBold.ttf", "OutfitSemiBold");
                    fonts.AddFont("Outfit-Medium.ttf", "OutfitMedium");
                });


            //builder.Services.AddHttpClient("VitoriaAPI", client =>
            //{
            //    client.BaseAddress = new Uri("http://10.0.2.2:5283/");
            //});


            //builder.Services.AddHttpClient("VitoriaAPI", client =>
            //{
            //    client.BaseAddress = new Uri("http://10.0.2.2:5283/");
            //});

            builder.Services.AddHttpClient("VitoriaAPI", client =>
            {
                client.BaseAddress = new Uri("http://192.168.1.16:5283/");
            });


            builder.Services.AddSingleton<IAuthService>(sp =>
            {
                var factory = sp.GetRequiredService<IHttpClientFactory>();
                var httpClient = factory.CreateClient("VitoriaAPI");
                return new AuthService(httpClient);
            });


            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ResetPasswordPage>();
            builder.Services.AddTransient<ResetPasswordViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
