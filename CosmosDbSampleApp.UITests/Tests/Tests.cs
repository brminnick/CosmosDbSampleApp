using System.Threading.Tasks;

using NUnit.Framework;

using Xamarin.UITest;

namespace CosmosDbSampleApp.UITests
{
    public class Tests : BaseTest
    {
        public Tests(Platform platform) : base(platform)
        {
        }

        [Test]
        public async Task AddNewContact()
        {
            //Arrange

            //Act
            PersonListPage.TapAddButton();

            AddPersonPage.EnterName(TestConstants.TestContactName);
            AddPersonPage.EnterAge(TestConstants.TestContactAge);
            AddPersonPage.TapSaveButton();

            await PersonListPage.WaitForPageToLoad().ConfigureAwait(false);

            //Assert
            Assert.IsTrue(PersonListPage.DoesContactExist(TestConstants.TestContactName));
        }

        [Test]
        public async Task CancelAddNewContact()
        {
            //Arrange

            //Act
            PersonListPage.TapAddButton();

            AddPersonPage.EnterName(TestConstants.TestContactName);
            AddPersonPage.EnterAge(TestConstants.TestContactAge);
            AddPersonPage.TapCancelButton();

            await PersonListPage.WaitForPageToLoad().ConfigureAwait(false);

            //Assert
            Assert.IsFalse(PersonListPage.DoesContactExist(TestConstants.TestContactName));
        }
    }
}
