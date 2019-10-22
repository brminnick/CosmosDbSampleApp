using Xamarin.UITest;
using System.Threading.Tasks;

namespace CosmosDbSampleApp.UITests
{
    public abstract class BasePage
    {
        protected BasePage(IApp app, string pageTitle)
        {
            App = app;
            Title = pageTitle;
        }

        public string Title { get; }
        protected IApp App { get; }

        public virtual Task WaitForPageToLoad() => Task.FromResult(App.WaitForElement(Title));
    }
}

