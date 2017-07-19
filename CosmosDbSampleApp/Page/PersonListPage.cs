using System;

using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListPage : BaseContentPage<PersonListViewModel>
    {
        #region Constant Fields
        readonly ListView _personList;
        readonly ToolbarItem _addButtonToolBarItem;
        #endregion

        #region Constructors
        public PersonListPage()
        {
            _addButtonToolBarItem = new ToolbarItem { Icon = "Add" };
            ToolbarItems.Add(_addButtonToolBarItem);

            _personList = new ListView(ListViewCachingStrategy.RecycleElement)
            {
                ItemTemplate = new DataTemplate(typeof(PersonListViewCell)),
                IsPullToRefreshEnabled = true,
                BackgroundColor = ColorConstants.PageBackgroundColor,
                HasUnevenRows = true
            };
            _personList.SetBinding(ListView.ItemsSourceProperty, nameof(ViewModel.PersonList));
            _personList.SetBinding(ListView.RefreshCommandProperty, nameof(ViewModel.PullToRefreshCommand));

            var activityIndicator = new ActivityIndicator();
            activityIndicator.SetBinding(IsVisibleProperty, nameof(ViewModel.IsDeletingPerson));
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsDeletingPerson));

            var whiteOverlayBoxView = new BoxView { BackgroundColor = new Color(1, 1, 1, 0.75) };
            whiteOverlayBoxView.SetBinding(IsVisibleProperty, nameof(ViewModel.IsDeletingPerson));

            var relativeLayout = new RelativeLayout();

            Func<RelativeLayout, double> getActivityIndicatorWidth = (p) => activityIndicator.Measure(p.Width, p.Height).Request.Width;
            Func<RelativeLayout, double> getActivityIndicatorHeight = (p) => activityIndicator.Measure(p.Width, p.Height).Request.Height;

            relativeLayout.Children.Add(_personList,
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

            Title = "Person List";
        }
        #endregion

        #region Properties
        public ListView PersonList => _personList;
        #endregion

        #region Methods
        protected override void OnAppearing()
        {
            base.OnAppearing();

            Device.BeginInvokeOnMainThread(_personList.BeginRefresh);
        }

        protected override void SubscribeEventHandlers()
        {
            ViewModel.Error += HandleError;
            _personList.ItemTapped += HandleItemTapped;
            _addButtonToolBarItem.Clicked += HandleAddButtonClicked;
            ViewModel.PullToRefreshCompleted += HandlePullToRefreshCommand;
        }

        protected override void UnsubscribeEventHandlers()
        {
            ViewModel.Error -= HandleError;
            _personList.ItemTapped -= HandleItemTapped;
            _addButtonToolBarItem.Clicked -= HandleAddButtonClicked;
            ViewModel.PullToRefreshCompleted -= HandlePullToRefreshCommand;
        }

        void HandleError(object sender, string e) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", e, "OK "));

        void HandleItemTapped(object sender, ItemTappedEventArgs e)
        {
            var listView = sender as ListView;
            listView.SelectedItem = null;
        }

        void HandleAddButtonClicked(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(async () =>
                await Navigation.PushModalAsync(new NavigationPage(new AddPersonPage())
                {
                    BarTextColor = ColorConstants.BarTextColor,
                    BarBackgroundColor = ColorConstants.BarBackgroundColor
                }));

        void HandlePullToRefreshCommand(object sender, EventArgs e) =>
            Device.BeginInvokeOnMainThread(_personList.EndRefresh);
        #endregion
    }
}
