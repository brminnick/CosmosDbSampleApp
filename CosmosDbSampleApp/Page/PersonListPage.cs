using System;
using CosmosDbSampleApp.Shared;
using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListPage : BaseContentPage<PersonListViewModel>
    {
        readonly ToolbarItem _addButtonToolBarItem;

        public PersonListPage()
        {
            ViewModel.ErrorTriggered += HandleErrorTriggered;

            _addButtonToolBarItem = new ToolbarItem
            {
                IconImageSource = "Add",
                AutomationId = AutomationIdConstants.PersonListPage_AddButton
            };
            _addButtonToolBarItem.Clicked += HandleAddButtonClicked;
            ToolbarItems.Add(_addButtonToolBarItem);

            PersonList = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                RefreshControlColor = Color.Black,
                ItemTemplate = new DataTemplate(typeof(PersonListViewCell)),
                IsPullToRefreshEnabled = true,
                BackgroundColor = ColorConstants.PageBackgroundColor,
                HasUnevenRows = true,
                AutomationId = AutomationIdConstants.PersonListPage_PersonList
            };
            PersonList.ItemTapped += HandleItemTapped;
            PersonList.SetBinding(ListView.IsRefreshingProperty, nameof(PersonListViewModel.IsRefreshing));
            PersonList.SetBinding(ListView.ItemsSourceProperty, nameof(PersonListViewModel.PersonList));
            PersonList.SetBinding(ListView.RefreshCommandProperty, nameof(PersonListViewModel.PullToRefreshCommand));

            var activityIndicator = new ActivityIndicator { AutomationId = AutomationIdConstants.PersonListPage_ActivityIndicator };
            activityIndicator.SetBinding(IsVisibleProperty, nameof(PersonListViewModel.IsDeletingPerson));
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(PersonListViewModel.IsDeletingPerson));

            var whiteOverlayBoxView = new BoxView { BackgroundColor = new Color(1, 1, 1, 0.75) };
            whiteOverlayBoxView.SetBinding(IsVisibleProperty, nameof(PersonListViewModel.IsDeletingPerson));

            var relativeLayout = new RelativeLayout();

            relativeLayout.Children.Add(PersonList,
                                       Constraint.Constant(0),
                                       Constraint.Constant(0));

            relativeLayout.Children.Add(whiteOverlayBoxView,
                                       Constraint.Constant(0),
                                       Constraint.Constant(0),
                                       Constraint.RelativeToParent(parent => parent.Width),
                                       Constraint.RelativeToParent(parent => parent.Height));

            relativeLayout.Children.Add(activityIndicator,
                                       Constraint.RelativeToParent(parent => parent.Width / 2 - getActivityIndicatorWidth(parent) / 2),
                                       Constraint.RelativeToParent(parent => parent.Height / 2 - getActivityIndicatorHeight(parent) / 2));

            Content = relativeLayout;

            Title = PageTitles.PersonListPage;

            double getActivityIndicatorWidth(RelativeLayout p) => activityIndicator.Measure(p.Width, p.Height).Request.Width;
            double getActivityIndicatorHeight(RelativeLayout p) => activityIndicator.Measure(p.Width, p.Height).Request.Height;
        }

        public ListView PersonList { get; }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(PersonList.BeginRefresh);
        }

        void HandleErrorTriggered(object sender, string e) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", e, "OK "));

        void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (sender is ListView listView)
                listView.SelectedItem = null;
        }

        void HandleAddButtonClicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await Navigation.PushModalAsync(new NavigationPage(new AddPersonPage())
                {
                    BarTextColor = ColorConstants.BarTextColor,
                    BarBackgroundColor = ColorConstants.BarBackgroundColor
                });
            });
        }
    }
}
