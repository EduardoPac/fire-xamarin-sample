using FireXamarin.Models;
using FireXamarin.Services;
using FireXamarin.UnitTests.Helpers;
using FluentAssertions;
using Xunit;

namespace Services
{
    public class ContactFirebaseServiceTest : BaseTests
    {
        #region Setup

        private readonly ContactFirebaseService _service = new();
        private readonly Contact _contactInTest = new() { Id = "test", Name = "test", Email = "test", Phone = "test", Removed = true};

        #endregion

        #region Valid

        [Fact(DisplayName = "[Valid] Get All Contacts")]
        public async void GetAllContacts()
        {
            var result = await _service.GetAllContacts();

            result.Should().NotBeNull();
        }

        [Fact(DisplayName = "[Valid] Get By Id")]
        public async void GetByIdValid()
        {
            var result = await _service.GetById(_contactInTest.Id);

            result.Should().BeEquivalentTo(_contactInTest);
        }

        [Fact(DisplayName = "[Valid] Save Contact")]
        public async void SaveContactValid()
        {
            var result = await _service.SaveContact(_contactInTest);

            result.Should().BeTrue();
        }

        [Fact(DisplayName = "[Valid] Remove Contact")]
        public async void RemoveContactValid()
        {
            var contact = EntitiesFactory.GetNewContact();

            await _service.SaveContact(contact);
            
            var result = await _service
                .RemoveContact(contact.Id);

            result.Should().BeTrue();
        }

        #endregion

        #region Invalid

        [Theory(DisplayName = "[Invalid] Get By Id")]
        [InlineData("")]
        [InlineData(null)]
        public async void GetByIdInvalid(string id)
        {
            var result = await _service.GetById(id);

            result.Should().BeNull();
        }

        [Theory(DisplayName = "[Invalid] Save Contact")]
        [InlineData(null, "test", "test", "test")]
        [InlineData("test", null, "test", "test")]
        [InlineData("test", "test", null, "test")]
        public async void SaveContactInvalid(string id, string name, string phone, string location)
        {
            var contact = EntitiesFactory.GetNewContactParameterized(id, name, phone, location);

            var result = await _service.SaveContact(contact);

            result.Should().BeFalse();
        }

        [Theory(DisplayName = "[Invalid] Remove Contact")]
        [InlineData(null)]
        [InlineData("")]
        public async void RemoveContactInvalid(string id)
        {
            var result = await _service.RemoveContact(id);

            result.Should().BeFalse();
        }

        #endregion
    }
}