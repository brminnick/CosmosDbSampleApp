using System;
using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListPage : BaseContentPage<PersonListViewModel>
    {
        readonly ListView _personList;
        readonly ToolbarItem _addButtonToolBarItem;

        public PersonListPage()
        {
            _addButtonToolBarItem = new ToolbarItem { Icon = "Add" };
            ToolbarItems.Add(_addButtonToolBarItem);

            _personList = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(PersonListTextCell)),
                IsPullToRefreshEnabled = true
            };
            _personList.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.PersonList));
            _personList.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.PullToRefreshCommand));

            Content = _personList;

            Title = "Person List";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(_personList.BeginRefresh);
        }

        protected override void SubscribeEventHandlers()
        {
            _personList.ItemTapped += HandleItemTapped;
            _addButtonToolBarItem.Clicked += HandleAddButtonClicked;
            ViewModel.PullToRefreshCompleted += HandlePullToRefreshCommand;
        }

        protected override void UnsubscribeEventHandlers()
        {
            _personList.ItemTapped -= HandleItemTapped;
            _addButtonToolBarItem.Clicked -= HandleAddButtonClicked;
        }

        void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {
            var listView = sender as ListView;
            listView.SelectedItem = null;
        }

        void HandleAddButtonClicked(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () =>
                await Navigation.PushModalAsync(new NavigationPage(new AddPersonPage())));

        void HandlePullToRefreshCommand(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(_personList.EndRefresh);
    }
}
