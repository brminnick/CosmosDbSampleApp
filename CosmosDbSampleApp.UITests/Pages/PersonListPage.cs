using System;
using System.Linq;
using System.Threading.Tasks;

using Xamarin.UITest;

using Xamarin.UITest.iOS;
using Xamarin.UITest.Android;

using CosmosDbSampleApp.Shared;

using Query = System.Func<Xamarin.UITest.Queries.AppQuery, Xamarin.UITest.Queries.AppQuery>;

namespace CosmosDbSampleApp.UITests
{
    public class PersonListPage : BasePage
    {
        #region Constant Fields
        readonly Query _personList, _activityIndicator, _addButton;
        #endregion

        #region Constructors
        public PersonListPage(IApp app, string pageTitle) : base(app, pageTitle)
        {
            _personList = x => x.Marked(AutomationIdConstants.PersonListPage_PersonList);
            _activityIndicator = x => x.Marked(AutomationIdConstants.PersonListPage_ActivityIndicator);
            _addButton = x => x.Marked(AutomationIdConstants.PersonListPage_AddButton);
        }
        #endregion

        #region Properties
        public bool IsRefreshIndicatorDisplayed => GetIsRefreshIndicatorDisplayed();
        #endregion

        #region Methods
        public override async Task WaitForPageToLoad()
        {
            await base.WaitForPageToLoad().ConfigureAwait(false);

            WaitForNoActivityIndicator();
            await WaitForPullToRefreshIndicatorToDisappear().ConfigureAwait(false);
        }

        public void TapAddButton()
        {
            switch (App)
            {
                case iOSApp iosApp:
                    iosApp.Tap(_addButton);
                    break;
                case AndroidApp androidApp:
                    androidApp.Tap(x => x.Class("ActionMenuItemView"));
                    break;
            }

            App.Screenshot("Add Button Tapped");
        }

        public void WaitForActivityIndicator()
        {
            App.WaitForElement(_activityIndicator);
            App.Screenshot("Activity Indicator Appeared");
        }

        public void WaitForNoActivityIndicator()
        {
            App.WaitForNoElement(_activityIndicator);
            App.Screenshot("Activity Indicator Disappeared");
        }

        public async Task DeletePerson(string name)
        {
            const string deleteText = "Delete";

            await ShowContextActions(name).ConfigureAwait(false);

            try
            {
                App.Tap(deleteText);
            }
            catch
            {
                App.Tap(deleteText.ToUpper());
            }

            await Task.Delay(1000).ConfigureAwait(false);

            App.Screenshot($"{name} Deleted");
        }

        public bool DoesContactExist(string name)
        {
            try
            {
                App.ScrollDownTo(name);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task WaitForPullToRefreshIndicatorToDisappear(int timeoutInSeconds = 30)
        {
            int counter = 0;

            while (IsRefreshIndicatorDisplayed)
            {
                await Task.Delay(1000).ConfigureAwait(false);
                counter++;

                if (counter >= timeoutInSeconds)
                    throw new Exception($"Loading the list took longer than {timeoutInSeconds}");
            }
        }

        async Task ShowContextActions(string name, int timeoutInSeconds = 180)
        {
            ScrollToQuery(name, timeoutInSeconds);

            App.Screenshot($"Scrolled to {name}");

            await Task.Delay(500).ConfigureAwait(false);

            var viewCellQuery = App.Query(name).First();
            var viewCellCenterY = viewCellQuery.Rect.CenterY;

            var displayQuery = App.Query().First();
            var displayQueryWidth = displayQuery.Rect.Width;

            switch (App)
            {
                case iOSApp iosApp:
                    iosApp.DragCoordinates(displayQueryWidth - 50, viewCellCenterY, 50, viewCellCenterY);
                    break;

                case AndroidApp androidApp:
                    androidApp.TouchAndHold(name);
                    break;

                default:
                    throw new NotSupportedException();
            }

            App.Screenshot($"Showed Context Actions for {name}");
        }

        void ScrollToQuery(string text, int timeoutInSeconds = 10) => ScrollToQuery(x => x.Marked(text), timeoutInSeconds);


        void ScrollToQuery(Query query, int timeoutInSeconds = 10)
        {
            try
            {
                App.ScrollDownTo(query, timeout: TimeSpan.FromSeconds(timeoutInSeconds));
            }
            catch
            {
                App.ScrollUpTo(query, timeout: TimeSpan.FromSeconds(timeoutInSeconds));
            }
        }

        bool GetIsRefreshIndicatorDisplayed()
        {
            switch (App)
            {
                case AndroidApp androidApp:
                    return (bool)(androidApp?.Query(x => x.Class("SwipeRefreshLayout")?.Invoke("isRefreshing"))?.FirstOrDefault() ?? false);
                case iOSApp iosApp:
                    return App?.Query(x => x.Class("UIRefreshControl"))?.Any() ?? false;
                default:
                    throw new NotSupportedException("Platform Not Supported");
            }
        }
        #endregion
    }
}
