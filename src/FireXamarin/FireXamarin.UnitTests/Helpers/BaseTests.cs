using Bogus;

namespace FireXamarin.UnitTests.Helpers
{
    public abstract class BaseTests
    {
        public readonly EntitiesFactory EntitiesFactory;
        private const int NumItems = 10; 

        protected BaseTests()
        {
            var faker = new Faker();
            EntitiesFactory = new EntitiesFactory(faker, NumItems);
        }
    }
}