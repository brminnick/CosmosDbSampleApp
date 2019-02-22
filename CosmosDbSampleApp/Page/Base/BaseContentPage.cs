using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public abstract class BaseContentPage<T> : ContentPage where T : BaseViewModel, new()
    {
        protected BaseContentPage()
        {
            BindingContext = ViewModel;
            BackgroundColor = Color.FromHex("F8E28B");
        }

        protected T ViewModel { get; } = new T();
    }
}
