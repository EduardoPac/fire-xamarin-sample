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
        [InlineData(null,"test","test","test")]
        [InlineData("test",null,"test","test")]
        [InlineData("test","test",null,"test")]
        public void PropertiesRequiredInvalid(string id, string name, string phone, string location)
        {
            var contact = EntitiesFactory.GetNewContactParameterized(id, name, phone, location);
            
            bool result = contact.ValidatePropertiesRequired();
            
            result.Should().BeFalse();
        }
    }
}