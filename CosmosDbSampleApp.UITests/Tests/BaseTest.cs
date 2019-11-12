using System;
using System.Threading.Tasks;
using CosmosDbSampleApp.Shared;
using NUnit.Framework;
using Xamarin.UITest;

namespace CosmosDbSampleApp.UITests
{
    [TestFixture(Platform.Android)]
    [TestFixture(Platform.iOS)]
    public abstract class BaseTest
    {
        readonly Platform _platform;

        IApp? _app;
        PersonListPage? _personListPage;
        AddPersonPage? _addPersonPage;

        protected BaseTest(Platform platform) => _platform = platform;

        protected IApp App => _app ?? throw new NullReferenceException();
        protected PersonListPage PersonListPage => _personListPage ?? throw new NullReferenceException();
        protected AddPersonPage AddPersonPage => _addPersonPage ?? throw new NullReferenceException();

        [SetUp]
        public async Task TestSetup()
        {
            _app = AppInitializer.StartApp(_platform);

            _personListPage = new PersonListPage(App, PageTitles.PersonListPage);
            _addPersonPage = new AddPersonPage(App, PageTitles.AddPersonPage);

            await PersonListPage.WaitForPageToLoad().ConfigureAwait(false);
            App.Screenshot("App Launched");
        }

        [TearDown]
        public async Task TestTearDown()
        {
            try
            {
                await PersonListPage.WaitForPageToLoad().ConfigureAwait(false);
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
    }
}

