using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class App : Application
    {
        public App()
        {
            Device.SetFlags(new[] { "Markup_Experimental" });

            MainPage = new NavigationPage(new PersonListPage())
            {
                BarBackgroundColor = ColorConstants.BarBackgroundColor,
                BarTextColor = ColorConstants.BarTextColor
            };
        }
    }
}
