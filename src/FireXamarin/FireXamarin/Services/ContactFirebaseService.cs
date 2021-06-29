using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Firebase.Database;
using Firebase.Database.Query;
using FireXamarin.Models;

namespace FireXamarin.Services
{
    public interface IContactFireBaseService
    {
        Task<IEnumerable<Contact>> GetAllContacts();
        Task<Contact> GetById(string id);
        Task<bool> SaveContact(Contact contact);
        Task<bool> RemoveContact(string id);
    }

    public class ContactFirebaseService : IContactFireBaseService
    {
        private readonly FirebaseClient _firebase = new("https://firexamarin-contacts.firebaseio.com/");
        private const string Table = "Contacts";

        public async Task<IEnumerable<Contact>> GetAllContacts() =>
                (await _firebase
                .Child(Table)
                .OnceAsync<Contact>()).Select(item =>
                new Contact()
                {
                    Id = item.Object.Id,
                    Name = item.Object.Name,
                    Phone = item.Object.Phone,
                    Email = item.Object.Email
                });

        public async Task<Contact> GetById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var result = (await _firebase
                    .Child(Table)
                    .OnceAsync<Contact>())
                .FirstOrDefault(a => a.Object.Id == id);

            return result?.Object;
        }
        
        public async Task<bool> SaveContact(Contact contact)
        {
            if (!contact.ValidatePropertiesRequired())
                return false;
            
            var contactExists = (await _firebase
                    .Child(Table)
                    .OnceAsync<Contact>())
                .FirstOrDefault(a => a.Object.Id == contact.Id);

            try
            {
                if (contactExists == null)
                {
                    var result = await _firebase
                        .Child(Table)
                        .PostAsync(contact);

                    return result != null;
                }
                
                await _firebase
                    .Child(Table)
                    .Child(contactExists?.Key)
                    .PutAsync(contact);

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public async Task<bool> RemoveContact(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;
            
            var contactToRemove = (await _firebase
                    .Child(Table)
                    .OnceAsync<Contact>())
                    .FirstOrDefault(a => a.Object.Id == id);

            if (contactToRemove == null)
                return false;

            try
            {
                await _firebase.Child(Table)
                    .Child(contactToRemove?.Key)
                    .DeleteAsync();
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }
    }
}