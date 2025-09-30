using VitoriaAirlinesMAUI.ViewModel;

namespace VitoriaAirlinesMAUI.View
{
    public partial class MainPage : ContentPage
    {

        public MainPage(MainPageViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }


    }

}
