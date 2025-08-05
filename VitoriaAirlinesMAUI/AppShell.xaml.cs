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
                    //Icon = "home.png",
                    ContentTemplate = new DataTemplate(() =>
                        serviceProvider.GetRequiredService<MainPage>())
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
                }
            }
            });
        }
    }
}
