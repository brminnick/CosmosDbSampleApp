using System.Linq;

using NUnit.Framework;

using Xamarin.UITest;

namespace CosmosDbSampleApp.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public class Tests : BaseTest
    {
        public Tests(Platform platform) : base(platform)
        {
        }

        [Test]
        public void AddNewContact()
        {
            //Arrange
            const string name = "Test Contact";
            const int age = 37;

            //Act
            PersonListPage.TapAddButton();

            AddPersonPage.EnterName(name);
            AddPersonPage.EnterAge(age);
            AddPersonPage.TapSaveButton();

            PersonListPage.WaitForPageToLoad();

            //Assert
            Assert.IsTrue(App.Query(name).Any());
            Assert.IsTrue(App.Query(age.ToString()).Any());
        }
    }
}
