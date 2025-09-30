using VitoriaAirlinesMAUI.View;

namespace VitoriaAirlinesMAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

        }



        public void ConfigureShellForAuthenticatedUser(IServiceProvider serviceProvider)
        {
            Items.Clear();

            Items.Add(new TabBar
            {
                Items =
            {
                new ShellContent
                {
                    Title = "Home",
                    Route = nameof(MainPage),
                    Icon = "home-icon.png",
                    ContentTemplate = new DataTemplate(() =>
                        serviceProvider.GetRequiredService<MainPage>())
                },

                   new ShellContent
                {
                    Title = "Book Flight",
                    Route = nameof(FlightsSearchPage),
                    Icon = "home-icon.png",
                    ContentTemplate = new DataTemplate(() =>
                        serviceProvider.GetRequiredService<FlightsSearchPage>())
                },

                new ShellSection
                {
                    Title = "Profile",
                    Icon = "profile-icon.png",
                    Items =
                    {
                        new ShellContent
                        {
                            Title = "Edit Profile",
                            Route = nameof(ProfilePage),
                            ContentTemplate = new DataTemplate(() =>
                                serviceProvider.GetRequiredService<ProfilePage>())
                        },
                        new ShellContent
                        {
                            Title = "Change Password",
                            Route = nameof(ChangePasswordPage),
                            ContentTemplate = new DataTemplate(() =>
                                serviceProvider.GetRequiredService<ChangePasswordPage>())
                        }
                    }
                },

                new ShellSection
                {
                    Title = "My Flights",
                    Icon = "myflights-icon.png",
                    Items =
                    {
                        new ShellContent
                        {
                            Title = "Upcoming Flights",
                            Route = nameof(UpcomingFlightsPage),
                            ContentTemplate = new DataTemplate(() =>
                                serviceProvider.GetRequiredService<UpcomingFlightsPage>())
                        },
                        new ShellContent
                        {
                            Title = "Past Flights",
                            Route = nameof(PastFlightsPage),
                            ContentTemplate = new DataTemplate(() =>
                                serviceProvider.GetRequiredService<PastFlightsPage>())
                        }
                    }
                },


            }
            });
        }

        public void ConfigureShellForAnonymousUser(IServiceProvider serviceProvider)
        {
            Items.Clear();


            Items.Add(new TabBar
            {
                Items =
        {
            new ShellContent
            {
                Title = "Welcome",
                Route = nameof(WelcomePage),
                Icon = "home-icon.png",
                ContentTemplate = new DataTemplate(() =>
                    serviceProvider.GetRequiredService<WelcomePage>())
            },

            new ShellContent
            {
                Title = "Book Flight",
                Route = nameof(FlightsSearchPage),
                Icon = "search-icon.png",
                ContentTemplate = new DataTemplate(() =>
                    serviceProvider.GetRequiredService<FlightsSearchPage>())
            },

            new ShellContent
            {
                Title = "Login",
                Route = nameof(LoginPage),
                Icon = "login-icon.png",
                ContentTemplate = new DataTemplate(() =>
                    serviceProvider.GetRequiredService<LoginPage>())
            }
        }
            });
        }
    }
}
