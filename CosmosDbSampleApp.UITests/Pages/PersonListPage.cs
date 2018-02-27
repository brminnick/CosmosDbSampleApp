using Xamarin.UITest;

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
            App.Tap(_addButton);
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
        #endregion
    }
}
