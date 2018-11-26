using NUnit.Framework;

using Xamarin.UITest;
using CosmosDbSampleApp.Shared;
using System.Threading.Tasks;

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

            PersonListPage.WaitForPageToLoad().GetAwaiter().GetResult();
            App.Screenshot("App Launched");
        }

        protected async Task TestTearDown()
        {
            try
            {
                await PersonListPage.WaitForPageToLoad();
            }
            catch
            {
                AddPersonPage.TapCancelButton();
            }

            if (PersonListPage.DoesContactExist(TestConstants.TestContactName))
            {
                await PersonListPage.DeletePerson(TestConstants.TestContactName).ConfigureAwait(false);

                PersonListPage.WaitForNoActivityIndicator();
                await PersonListPage.WaitForPullToRefreshIndicatorToDisappear().ConfigureAwait(false);
            }
        }
        #endregion
    }
}

