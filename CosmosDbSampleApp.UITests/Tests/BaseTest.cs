using NUnit.Framework;

using Xamarin.UITest;
using CosmosDbSampleApp.Shared;

namespace CosmosDbSampleApp.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public abstract class BaseTest
    {
        #region Constant Fields
        readonly Platform _platform;
        #endregion

        #region Constructors
        protected BaseTest(Platform platform) => _platform = platform;
        #endregion

        #region Properties
        protected IApp App { get; private set; }
        protected PersonListPage PersonListPage { get; private set; }
        protected AddPersonPage AddPersonPage { get; private set; }
        #endregion

        #region Methods
        [SetUp]
        public virtual void TestSetup()
        {
            App = AppInitializer.StartApp(_platform);

            PersonListPage = new PersonListPage(App, PageTitles.PersonListPage);
            AddPersonPage = new AddPersonPage(App, PageTitles.AddPersonPage);

            PersonListPage.WaitForPageToLoad();
            App.Screenshot("App Launched");
        }

        [TearDown]
        public virtual void TestTearDown()
        {
            try
            {
                PersonListPage.WaitForPageToLoad();
            }
            catch
            {
                AddPersonPage.TapCancelButton();
            }

            if (PersonListPage.DoesContactExist(TestConstants.TestContactName))
            {
                PersonListPage.DeletePerson(TestConstants.TestContactName);
                PersonListPage.WaitForNoActivityIndicator();
            }
        }
        #endregion
    }
}

