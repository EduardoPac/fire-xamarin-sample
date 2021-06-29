using System.Threading.Tasks;
using FireXamarin.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FireXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactViewPage : ContentPage
    {
        public ContactViewPage()
        {
            InitializeComponent();
            BindingContext = new ContactViewViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await OnAppearingAsync();
        }

        private async Task OnAppearingAsync()
        {
            if (BindingContext is IInitialize viewModel)
                await viewModel.InitializeAsync();
        }
    }
}
