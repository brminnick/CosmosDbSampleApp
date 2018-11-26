using Xamarin.UITest;
using System.Threading.Tasks;

namespace CosmosDbSampleApp.UITests
{
    public abstract class BasePage
    {
        #region Constructors
        protected BasePage(IApp app, string pageTitle)
        {
            App = app;
            Title = pageTitle;
        }
        #endregion

        #region Properties
        public string Title { get; }
        protected IApp App { get; }
        #endregion

        #region Methods
        public virtual Task WaitForPageToLoad() => Task.FromResult(App.WaitForElement(Title));
        #endregion
    }
}

