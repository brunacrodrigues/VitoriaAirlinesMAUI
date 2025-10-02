using Microsoft.Extensions.Logging;
using Syncfusion.Maui.Core.Hosting;
using VitoriaAirlinesMAUI.Helpers;
using VitoriaAirlinesMAUI.Services;
using VitoriaAirlinesMAUI.Services.Interfaces;
using VitoriaAirlinesMAUI.View;
using VitoriaAirlinesMAUI.ViewModel;
using ZXing.Net.Maui.Controls;

namespace VitoriaAirlinesMAUI
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder
                .UseMauiApp<App>()
                .UseBarcodeReader()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                    fonts.AddFont("Outfit-Regular.ttf", "OutfitRegular");
                    fonts.AddFont("Outfit-Bold.ttf", "OutfitBold");
                    fonts.AddFont("Outfit-SemiBold.ttf", "OutfitSemiBold");
                    fonts.AddFont("Outfit-Medium.ttf", "OutfitMedium");
                });

            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JFaF5cXGRCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdmWH9ecXZVRWFZV01/XkdWYEg=");


            builder.Services.AddTransient<AuthHeaderHandler>();


            //var baseUri = new Uri("http://10.0.2.2:5283/");
            //var baseUri = new Uri("http://192.168.1.254:5283/");
            var baseUri = new Uri("http://vitoriaairlinesapi.eu-north-1.elasticbeanstalk.com/");





            builder.Services
               .AddHttpClient<IApiService, ApiService>(client =>
               {
                   client.BaseAddress = baseUri;
               })
               .AddHttpMessageHandler<AuthHeaderHandler>();


            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<ICountryService, CountryService>();
            builder.Services.AddScoped<ITicketService, TicketService>();
            builder.Services.AddScoped<IFlightService, FlightService>();
            builder.Services.AddScoped<IBookingService, BookingService>();
            builder.Services.AddScoped<IDashboardService, DashboardService>();

            // ViewModels
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<ForgotPasswordViewModel>();
            builder.Services.AddTransient<ResetPasswordViewModel>();
            builder.Services.AddTransient<EditProfileViewModel>();
            builder.Services.AddTransient<ChangePasswordViewModel>();
            builder.Services.AddTransient<UpcomingFlightsViewModel>();
            builder.Services.AddTransient<FlightsHistoryViewModel>();
            builder.Services.AddTransient<FlightSearchViewModel>();
            builder.Services.AddTransient<FlightSearchResultsViewModel>();
            builder.Services.AddTransient<SelectSeatViewModel>();
            builder.Services.AddTransient<BookingConfirmationViewModel>();
            builder.Services.AddTransient<PaymentViewModel>();
            builder.Services.AddTransient<BoardingPassViewModel>();
            builder.Services.AddTransient<WelcomeViewModel>();
            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<AboutViewModel>();
            builder.Services.AddTransient<MainPageViewModel>();


            // Views
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<ForgotPasswordPage>();
            builder.Services.AddTransient<ResetPasswordPage>();
            builder.Services.AddTransient<ProfilePage>();
            builder.Services.AddTransient<ChangePasswordPage>();
            builder.Services.AddTransient<MainPage>();
            builder.Services.AddTransient<UpcomingFlightsPage>();
            builder.Services.AddTransient<PastFlightsPage>();
            builder.Services.AddTransient<FlightsSearchPage>();
            builder.Services.AddTransient<FlightSearchResultsPage>();
            builder.Services.AddTransient<SelectSeatPage>();
            builder.Services.AddTransient<BookingConfirmationPage>();
            builder.Services.AddTransient<PaymentPage>();
            builder.Services.AddTransient<BoardingPassPage>();
            builder.Services.AddTransient<WelcomePage>();
            builder.Services.AddTransient<RegistrationPage>();
            builder.Services.AddTransient<AboutPage>();


            builder.Services.AddSingleton<AppShell>();


            builder.ConfigureSyncfusionCore();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}

