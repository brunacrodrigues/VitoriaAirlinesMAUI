using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View;

public partial class ProfilePage : ContentPage
{
    private readonly EditProfileViewModel _viewModel;

    public ProfilePage(EditProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = _viewModel = viewModel;
    }


    /// <summary>
    /// Handles the page loaded event to trigger the loading of the user's profile data.
    /// </summary>
    /// <param name="sender">The object that raised the event.</param>
    /// <param name="e">The event arguments.</param>
    private async void OnPageLoaded(object sender, EventArgs e)
    {
        await _viewModel.LoadProfileAsync();
    }

}