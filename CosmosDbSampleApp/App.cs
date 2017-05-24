using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class App : Application
    {
        public App()=>
            MainPage = new NavigationPage(new PersonListPage());
    }
}
