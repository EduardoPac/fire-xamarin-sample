using System.Threading.Tasks;
using FireXamarin.Models;
using FireXamarin.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace FireXamarin.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ContactDataPage : ContentPage
    {
        public ContactDataPage(Contact selected = null)
        {
            InitializeComponent();
            BindingContext = new ContactDataViewModel(selected); 
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