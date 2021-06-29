using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using BurgerMonkeys.Tools;
using Xamarin.CommunityToolkit.ObjectModel;
using Contact = FireXamarin.Models.Contact;

namespace FireXamarin.ViewModels
{
    public class ContactListViewModel : BaseViewModel, IInitialize
    {
        public List<Contact> AllContacts { get; set; }
        public ObservableRangeCollection<Contact> Contacts { get; set; }

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

        public ContactListViewModel()
        {
            EmptyMessage = "Carregando...";
            AllContacts = new List<Contact>();
            Contacts = new ObservableRangeCollection<Contact>();
            AddContactCommand = new AsyncCommand(AddContactExecute);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task InitializeAsync()
        {
            await LoadContacts();
        }

        private async Task LoadContacts()
        {
            if (AllContacts.Any())
            {
                await Task.Delay(10);

                AllContacts.Clear();
            }

            AllContacts = new List<Contact>
            {
                new Contact()
                {
                    Name = "Contato 1",
                    Phone = "(43) 99165-5865"
                },
                new Contact()
                {
                    Name = "Contato 1",
                    Phone = "(43) 99165-5865",
                    Email = "eduardolll"
                },
                new Contact()
                {
                    Name = "Contato 1",
                    Phone = "(43) 99165-5865",
                    Location = "dfdfdfdf"
                },
                new Contact()
                {
                    Name = "Contato 1",
                    Phone = "(43) 99165-5865",
                    Email = "eduardolll",
                    Location = "dfdfdfdf"
                }
            };

            if (AllContacts.Any())
                SearchExecute("");
            else
            {
                EmptyMessage = "Não foi encontrado nenhum contato";

                await Task.Delay(10);

                Contacts.Clear();
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

        private Task AddContactExecute()
        {
            throw new NotImplementedException();
        }
    }
}