using FireXamarin.UnitTests.Helpers;
using FluentAssertions;
using Xunit;

namespace FireXamarin.UnitTests.Tests.Entitites
{
    public class ContactTest : BaseTests
    {
        [Fact]
        public void PropertiesRequiredValid()
        {
            var user = EntitiesFactory.GetNewContact();
            
            bool result = user.ValidatePropertiesRequired();
            
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(null,"test","test")]
        [InlineData("test",null,"test")]
        [InlineData("test","test",null)]
        public void PropertiesRequiredInvalid(string id, string name, string phone)
        {
            var contact = EntitiesFactory.GetNewContactParameterized(id, name, phone);
            
            bool result = contact.ValidatePropertiesRequired();
            
            result.Should().BeFalse();
        }
    }
}