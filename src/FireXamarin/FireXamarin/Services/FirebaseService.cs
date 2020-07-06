using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BurgerMonkeys.Tools;
using Firebase.Database;
using Firebase.Database.Query;
using FireXamarin.Models;

namespace FireXamarin.Services
{
    public interface IFireBaseService
    {
        Task<IEnumerable<Contact>> GetAllContacts();
        Task<Contact> GetById(string id);
        Task<bool> AddContact(string name, string phone, string email);
        Task<bool> EditContact(string id, string name, string phone, string email);
        Task<bool> RemoveContact(string id);
    }


    public class FirebaseService : IFireBaseService
    {
        private readonly FirebaseClient _firebase = new FirebaseClient("https://xamarinfirebase-909d2.firebaseio.com/");
        private const string Table = "Contacts";

        public async Task<IEnumerable<Contact>> GetAllContacts()
        {
            return (await _firebase
                .Child(Table)
                .OnceAsync<Contact>()).Select(item =>
                new Contact()
                {
                    Id = item.Object.Id,
                    Name = item.Object.Name,
                    Phone = item.Object.Phone,
                    Email = item.Object.Email
                });
        }

        public async Task<Contact> GetById(string id)
        {
            var result = (await _firebase
                    .Child(Table)
                    .OnceAsync<Contact>())
                .FirstOrDefault(a => a.Object.Id == id);

            return result?.Object;
        }

        public async Task<bool> AddContact(string name, string phone, string email)
        {
            var result = await _firebase
                .Child(Table)
                .PostAsync(
                    new Contact()
                    {
                        Id = Generator.GetId(8),
                        Name = name,
                        Phone = phone,
                        Email = email
                    });

            return result != null;
        }

        public async Task<bool> EditContact(string id, string name, string phone, string email)
        {
            var contactToEdit = (await _firebase
                    .Child(Table)
                    .OnceAsync<Contact>())
                .FirstOrDefault(a => a.Object.Id == id);

            try
            {
                await _firebase
                    .Child(Table)
                    .Child(contactToEdit?.Key)
                    .PutAsync(new Contact()
                    {
                        Id = id,
                        Name = name,
                        Phone = phone,
                        Email = email
                    });

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
            var contactToRemove = (await _firebase
                    .Child(Table)
                    .OnceAsync<Contact>())
                    .FirstOrDefault(a => a.Object.Id == id);

            try
            {
                await _firebase.Child(Table)
                    .Child(contactToRemove?.Key)
                    .DeleteAsync();
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
            
        }
    }
}