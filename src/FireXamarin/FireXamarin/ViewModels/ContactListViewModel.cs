using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BurgerMonkeys.Tools;
using FireXamarin.Services;
using FireXamarin.Views;
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
        public ICommand ItemTappedCommand { get; set; }

        public ContactListViewModel()
        {
            EmptyMessage = "Carregando...";
            AllContacts = new List<Contact>();
            Contacts = new ObservableRangeCollection<Contact>();
            AddContactCommand = new AsyncCommand(AddContactExecute);
            RefreshCommand = new AsyncCommand(RefreshCommandExecute);
            ItemTappedCommand = new AsyncCommand(ExecuteItemTappedCommand);
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

            AllContacts = (await _contactFirebaseService.GetAllContacts()).ToList();

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
                    || (i.Location != null && (!string.IsNullOrWhiteSpace(i.Location) && i.Name.IgnoreCaseSensitiveAndAccents().Contains(search)))
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

        private async Task AddContactExecute() => await Application.Current.MainPage.Navigation.PushAsync(new ContactDataPage());

        #region Options
        async Task ExecuteItemTappedCommand()
        {
            if (Selected == null)
                return;

            if (Device.RuntimePlatform == Device.iOS)
                await ShowActionSheet(Selected);

            else
                await ShowBottomSheet(Selected);
        }

        private async Task ShowBottomSheet(Contact selected)
        {
            /*
            var items = GetBottomSheetItems(selected);

            await _navigationService.NavigateAsync(nameof(BottomSheetView), new NavigationParameters
            {
                {Constants.BottomSheetTitle, selected.CodeWithName},
                {Constants.BottomSheetOptions, items},
                {Constants.BottomSheetContactSelected, Selected }
            });
            */
            Selected = null;
        }

        private async Task ShowActionSheet(Contact selected)
        {
            /*
            var actions = GetActionSheetOptions(selected);
            if (actions == null || !actions.Any())
                return;
            
            var actionSelected = await _dialogService.DisplayActionSheetAsync(
                                                      selected.CodeWithName,
                                                      AppResources.ActionClose,
                                                      null,
                                                      actions);

            await ExecuteOption(actionSelected, selected);
            */
            Selected = null;
        }

        /*
        List<BottomSheetItem> GetBottomSheetItems(Contact selected)
        {
            var items = new List<BottomSheetItem>{
                     new BottomSheetItem { Name = AppResources.ActionChangeStatus, Icon = "f101".ToUnicode() },
                     new BottomSheetItem { Name = AppResources.ActionAttachments, Icon = "f0c6".ToUnicode()},
                };

            if (selected.IsResponsible)
            {
                items.Insert(0, new BottomSheetItem { Name = AppResources.ActionEdit, Icon = "f044".ToUnicode() });
                items.Add(new BottomSheetItem { Name = AppResources.ActionRemove, Icon = "f2ed".ToUnicode() });
            }

            else if (selected.IsChecker)
            {
                items.Insert(0, new BottomSheetItem { Name = AppResources.ActionView, Icon = "f06e".ToUnicode() });
            }

            return items;

        }

        string[] GetActionSheetOptions(Contact selected)
        {
            var items = new List<string>{
                     AppResources.ActionChangeStatus,
                     AppResources.ActionAttachments
                };

            if (selected.IsResponsible)
            {
                items.Insert(0, AppResources.ActionEdit);
                items.Add(AppResources.ActionRemove);
            }

            else if (selected.IsChecker)
            {
                items.Insert(0, AppResources.ActionView);
            }

            return items.ToArray();

        }

        private async Task ExecuteOption(string actionSelected, Contact selected)
        {
            if (actionSelected == AppResources.ActionView)
            {
                await ViewAction(selected);
            }
            else if (actionSelected == AppResources.ActionRemove)
            {
                await RemovePlan(selected);
            }
            else if (actionSelected == AppResources.ActionEdit)
            {
                await EditAction(selected);
            }

            else if (actionSelected == AppResources.ActionAttachments)
            {
                await ShowAttachments(selected);
            }

            else if (actionSelected == AppResources.ActionChangeStatus)
            {
                await ChangeStatusTapped(selected);
            }

            Selected = null;
        }
        */
        #endregion
    }
}