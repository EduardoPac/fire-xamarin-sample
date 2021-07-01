using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using BurgerMonkeys.Tools;
using FireXamarin.Services;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Contact = FireXamarin.Models.Contact;

namespace FireXamarin.ViewModels
{
    public class ContactDataViewModel : BaseViewModel, IInitialize
    {
        Contact _contact;
        public Contact CurrentContact
        {
            get => _contact;
            set => SetProperty(ref _contact, value);
        }

        private readonly IContactFireBaseService _contactFirebaseService;

        public bool IsEdit { get; set; }

        public ICommand SaveCommand { get; set; }
        public ICommand GetLocationCommand { get; set; }
        public ICommand RemoveContactCommand { get; set; }

        public ContactDataViewModel(Contact contact)
        {
            CurrentContact = contact;
            SaveCommand = new AsyncCommand(SaveCommandExecute);
            GetLocationCommand = new AsyncCommand(GetLocationCommandExecute);
            RemoveContactCommand = new AsyncCommand(RemoveContactCommandExecute);
            _contactFirebaseService = new ContactFirebaseService();
        }

        public async Task InitializeAsync()
        {
            if (CurrentContact != null)
                IsEdit = true;
            else
                CurrentContact = new Contact();
        }

        private async Task SaveCommandExecute()
        {
            if (Validate())
            {
                await Application.Current.MainPage.DisplayAlert("Salvar contato", "Nome e telefone não obrigatórios", "OK");
                return;
            }

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert("Salvar contato", "Sem internet para remover o contato", "OK");
                return;
            }

            if (IsEdit)
            {
                await _contactFirebaseService.SaveContact(CurrentContact);
                await Application.Current.MainPage.Navigation.PopAsync();
            }

            if (CurrentContact.Id.IsNullOrWhiteSpace())
            {
                CurrentContact.Id = Generator.GetId(8);

                await _contactFirebaseService.SaveContact(CurrentContact);
                await Application.Current.MainPage.Navigation.PopAsync();
            }
        }

        private bool Validate()
        {
            return CurrentContact.Name.IsNullOrWhiteSpace() || CurrentContact.Phone.IsNullOrWhiteSpace();
        }

        private async Task RemoveContactCommandExecute()
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert(CurrentContact.Name, "Sem internet para remover o contato", "OK");
                return;
            }

            var result = await Application.Current.MainPage.DisplayAlert(CurrentContact.Name, "Remover o contato?",
                        "Sim", "Não");

            if (!result)
                return;

            await _contactFirebaseService.RemoveContact(CurrentContact.Id);
            await Application.Current.MainPage.Navigation.PopAsync();
        }

        CancellationTokenSource cts;
        private async Task GetLocationCommandExecute()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
                cts = new CancellationTokenSource();
                var location = await Geolocation.GetLocationAsync(request, cts.Token);

                if (location != null)
                {
                    var placemarks = await Geocoding.GetPlacemarksAsync(location.Latitude, location.Longitude);
                    var placemark = placemarks?.FirstOrDefault();

                    CurrentContact.LocationName = string.Format("{0},{1} - {2}", placemark.Thoroughfare, placemark.FeatureName, placemark.SubAdminArea); ;
                    CurrentContact.LocationLatitude = location.Latitude;
                    CurrentContact.LocationLongitude = location.Longitude;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert("Erro", "Não foi possivel usar o GPS", "OK");
                return;
            }
        }
    }
}