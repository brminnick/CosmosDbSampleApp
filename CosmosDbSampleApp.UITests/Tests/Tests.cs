using System.Linq;

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
        public void AddNewContact()
        {
            //Arrange

            //Act
            PersonListPage.TapAddButton();

            AddPersonPage.EnterName(TestConstants.TestContactName);
            AddPersonPage.EnterAge(TestConstants.TestContactAge);
            AddPersonPage.TapSaveButton();

            PersonListPage.WaitForPageToLoad();

            //Assert
            Assert.IsTrue(PersonListPage.DoesContactExist(TestConstants.TestContactName));
        }

        [Test]
        public void CancelAddNewContact()
        {
            //Arrange

            //Act
            PersonListPage.TapAddButton();

            AddPersonPage.EnterName(TestConstants.TestContactName);
            AddPersonPage.EnterAge(TestConstants.TestContactAge);
            AddPersonPage.TapCancelButton();

            PersonListPage.WaitForPageToLoad();

            //Assert
            Assert.IsFalse(PersonListPage.DoesContactExist(TestConstants.TestContactName));
        }
    }
}
