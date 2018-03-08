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

        #region Methods
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

        public void DeletePerson(string name)
        {
            switch(App)
            {
                case AndroidApp androidApp:
                    androidApp.TouchAndHold(name);
                    break;
                case iOSApp iosApp:
                    iosApp.SwipeRightToLeft(name);
                    break;
            }
            App.Screenshot("Exposed Delete Menu");

            App.Tap("Delete");
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
        #endregion
    }
}
