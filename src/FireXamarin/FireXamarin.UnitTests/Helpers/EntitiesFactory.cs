using System.Collections.Generic;
using Bogus;
using BurgerMonkeys.Tools;
using FireXamarin.Models;

namespace FireXamarin.UnitTests.Helpers
{
    public class EntitiesFactory
    {
        private readonly Faker _faker;
        private readonly int _numItems;
        
        public EntitiesFactory(Faker faker, in int numItems)
        {
            _faker = faker;
            _numItems = numItems;
        }

        public Contact GetNewContact() =>
            new Contact
            {
                Id = Generator.GetId(8),
                Name = _faker.Person.FullName,
                Phone = _faker.Person.Phone,
                Email = _faker.Person.Email
            };

        public Contact GetNewContactParameterized(string id, string name, string phone) =>
            new Contact
            {
                Id = id,
                Name = name,
                Phone = phone
            };

        public List<Contact> GetContactList()
        {
            var contacts = new List<Contact>();
            for (var i = 0; i < _numItems; i++)
            {
                contacts.Add(GetNewContact());
            }

            return contacts;
        }
    }
}