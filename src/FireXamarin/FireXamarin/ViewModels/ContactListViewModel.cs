using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BurgerMonkeys.Tools;
using FireXamarin.Services;
using FireXamarin.Views;
using Rg.Plugins.Popup.Extensions;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Contact = FireXamarin.Models.Contact;

namespace FireXamarin.ViewModels
{
    public class ContactListViewModel : BaseViewModel, IInitialize
    {
        public List<Contact> AllContacts { get; set; }
        public ObservableRangeCollection<Contact> Contacts { get; set; }
        private readonly IContactFireBaseService _contactFirebaseService;

        private bool isFirstAccess = true;

        Contact _selected;
        public Contact Selected
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                SearchExecute(value);
            }
        }

        string _emptyMessage;

        public string EmptyMessage
        {
            get => _emptyMessage;
            set => SetProperty(ref _emptyMessage, value);
        }

        public ICommand AddContactCommand { get; set; }
        public ICommand RefreshCommand { get; set; }
        public ICommand SelectionChangedCommand { get; set; }

        public ContactListViewModel()
        {
            EmptyMessage = "Carregando...";
            AllContacts = new List<Contact>();
            Contacts = new ObservableRangeCollection<Contact>();
            AddContactCommand = new AsyncCommand(AddContactExecute);
            RefreshCommand = new AsyncCommand(RefreshCommandExecute);
            SelectionChangedCommand = new AsyncCommand(ExecuteSelectionChangedCommand);
            _contactFirebaseService = new ContactFirebaseService();
        }

        public async Task InitializeAsync()
        {
            await LoadContacts();
        }

        #region LoadList
        private async Task RefreshCommandExecute()
        {
            if (!isFirstAccess && !IsBusy)
                await LoadContacts();
        }

        private async Task LoadContacts()
        {
            IsBusy = true;

            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                EmptyMessage = "Sem Internet :(";
                isFirstAccess = false;
            }
          

            if (AllContacts.Any())
            {
                await Task.Delay(10);

                AllContacts.Clear();
            }

            var contacts = (await _contactFirebaseService.GetAllContacts()).ToList();
            //AllContacts = contacts.Where(c => !c.Removed).ToList();
            AllContacts = contacts;

            if (AllContacts.Any())
                SearchExecute("");
            else
            {
                EmptyMessage = "Não foi encontrado nenhum contato";

                await Task.Delay(10);

                Contacts.Clear();

                isFirstAccess = false;
                IsBusy = false;
            }
        }

        private List<Contact> SortContacts(List<Contact> contacts)
        {
            contacts.ForEach(c => { c.IsFirst = false; c.IsLast = false; });
            contacts.OrderBy(c => c.Name);
            contacts.First().IsFirst = true;
            contacts.Last().IsLast = true;

            return contacts;
        }

        async void SearchExecute(string search)
        {
            try
            {
                search ??= "";

                if (!string.IsNullOrWhiteSpace(search))
                    search = search.IgnoreCaseSensitiveAndAccents();

                List<Contact> searchResult = AllContacts.Where(i =>
                       (i.Name != null && (!string.IsNullOrWhiteSpace(i.Name) && i.Name.IgnoreCaseSensitiveAndAccents().Contains(search)))
                    || (i.Phone != null && (!string.IsNullOrWhiteSpace(i.Phone) && i.Phone.IgnoreCaseSensitiveAndAccents().Contains(search)))
                    || (i.Email != null && (!string.IsNullOrWhiteSpace(i.Email) && i.Email.IgnoreCaseSensitiveAndAccents().Contains(search)))
                    || (i.LocationName != null && (!string.IsNullOrWhiteSpace(i.LocationName) && i.Name.IgnoreCaseSensitiveAndAccents().Contains(search)))
                ).ToList();

                var data = SortContacts(searchResult);

                if (data == null | !data.Any())
                {
                    await Task.Delay(50);
                    Contacts.Clear();
                    EmptyMessage = "Não foi encontrado nenhum contato";
                    return;
                }

                if (data.Any())
                {
                    EmptyMessage = "Carregando...";
                    Contacts.Clear();

                    await Task.Delay(50);
                    Contacts.AddRange(data);
                }

                IsBusy = false;
                isFirstAccess = false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                isFirstAccess = false;
                IsBusy = false;
            }
        }
        #endregion

        private async Task AddContactExecute() =>
            await Application.Current.MainPage.Navigation.PushAsync(new ContactDataPage());

        #region Options

        const string action_edit = "Editar";
        const string action_call = "Fazer ligação";
        const string action_mail = "Enviar email";
        const string action_route = "Traçar rota";
        const string action_remove = "Remover";
        BottomSheetView _bottomSheet;

        async Task ExecuteSelectionChangedCommand()
        {
            if (Selected == null)
                return;

            var title = Selected.Name;
            if (Device.RuntimePlatform == Device.Android)
            {
                var items = new List<BottomSheetItem>
                {
                    new BottomSheetItem {Icon = "&#xf304;", Name = action_edit},
                    new BottomSheetItem {Icon = "&#xf879;", Name = action_call}
                };

                if (Selected.HasEmail)
                    items.Add(new BottomSheetItem { Icon = "&#xf0e0;", Name = action_mail });
                if (Selected.HasLocation)
                    items.Add(new BottomSheetItem { Icon = "&#xf3c5;", Name = action_route });

                items.Add(new BottomSheetItem { Icon = "&#xf2ed;", Name = action_remove });

                await ShowOptionsBottomSheet(items, title);
            }
            else
            {
                var items = new List<string>
                {
                    action_edit,
                    action_call
                };

                if (Selected.HasEmail)
                    items.Add(action_mail);
                if (Selected.HasLocation)
                    items.Add(action_route);

                items.Add(action_remove);

                await ShowOptionsActionSheet(
                    items?.ToArray(),
                    title);
            }
        }

        async Task ShowOptionsActionSheet(string[] options, string title)
        {
            var optionSelected = await Application.Current.MainPage
                .DisplayActionSheet(
                    title,
                    "Cancelar",
                    null,
                    options);
            ExecuteAction(optionSelected);
        }

        async Task ShowOptionsBottomSheet(List<BottomSheetItem> items, string title)
        {
            _bottomSheet = new BottomSheetView(title, items);
            _bottomSheet.SeletectItem += BottomSheetSelectedItem;
            await Application.Current.MainPage.Navigation.PushPopupAsync(_bottomSheet);
        }

        void BottomSheetSelectedItem(object sender, ItemTappedEventArgs e)
        {
            if (_bottomSheet != null)
                _bottomSheet.SeletectItem -= BottomSheetSelectedItem;

            if (e.Item == null)
                return;

            ExecuteAction((e.Item as BottomSheetItem)?.Name);
        }

        private async void ExecuteAction(string action)
        {
            if (action == action_edit)
                await EditContact(Selected);
            else if (action == action_call)
                await CallToContact(Selected.Phone);
            else if (action == action_mail)
                await MailToContact(Selected.Email);
            else if (action == action_route)
                await RouteToContact(Selected);
            else if (action == action_remove)
                await RemoveContact(Selected);

            Selected = null;
        }

        private async Task RemoveContact(Contact selected)
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await Application.Current.MainPage.DisplayAlert(selected.Name, "Sem internet para remover o contato", "OK");
                return;
            }

            var result = await Application.Current.MainPage.DisplayAlert(selected.Name, "Remover o contato?",
                        "Sim", "Não");

            if (!result)
                return;

            await _contactFirebaseService.RemoveContact(Selected.Id);

            await LoadContacts();
        }

        private async Task RouteToContact(Contact selected)
        {
            try
            {
                await Map.OpenAsync(
                    new Location(selected.LocationLatitude, selected.LocationLongitude),
                    new MapLaunchOptions { Name = Selected.LocationName });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert(selected.Name, "O endereço não é valido", "OK");
            }

        }

        private async Task MailToContact(string email)
        {
            try
            {
                await Email.ComposeAsync("Fire Contacts", "É um email enviado pelo app", new string[] { email });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert(email, "O email não é valido", "OK");
            }
        }

        private async Task CallToContact(string phone)
        {
            try
            {
                PhoneDialer.Open(phone);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                await Application.Current.MainPage.DisplayAlert(phone, "O telefone não é valido", "OK");
            }
        }

        private async Task EditContact(Contact selected) =>
            await Application.Current.MainPage.Navigation.PushAsync(new ContactDataPage(selected));
        #endregion
    }
}