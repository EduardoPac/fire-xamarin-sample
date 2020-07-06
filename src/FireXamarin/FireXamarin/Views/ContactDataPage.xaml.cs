using System.Threading.Tasks;
using FireXamarin.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FireXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactDataPage : ContentPage
    {
        public ContactDataPage()
        {
            InitializeComponent();
            BindingContext = new ContactDataViewModel(); 
        }
        
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            await OnAppearingAsync();
        }
        
        async Task OnAppearingAsync()
        {
            if (BindingContext is IInitialize viewModel)
                await viewModel.InitializeAsync();
        }
    }
}