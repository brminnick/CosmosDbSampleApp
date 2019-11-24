using System;
using System.Linq;
using CosmosDbSampleApp.Shared;
using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListPage : BaseContentPage<PersonListViewModel>
    {
        public PersonListPage()
        {
            ViewModel.ErrorTriggered += HandleErrorTriggered;

            var addButtonToolBarItem = new ToolbarItem
            {
                IconImageSource = "Add",
                AutomationId = AutomationIdConstants.PersonListPage_AddButton
            };
            addButtonToolBarItem.Clicked += HandleAddButtonClicked;
            ToolbarItems.Add(addButtonToolBarItem);

            var personList = new CollectionView
            {
                ItemTemplate = new PersonListDataTemplate(),
                BackgroundColor = ColorConstants.PageBackgroundColor,
                AutomationId = AutomationIdConstants.PersonListPage_PersonList
            };
            personList.SelectionChanged += HandleSelectionChanged;
            personList.SetBinding(CollectionView.ItemsSourceProperty, nameof(PersonListViewModel.PersonList));

            var refreshView = new RefreshView
            {
                RefreshColor = Color.Black,
                Content = personList
            };
            refreshView.SetBinding(RefreshView.IsRefreshingProperty, nameof(PersonListViewModel.IsRefreshing));
            refreshView.SetBinding(RefreshView.CommandProperty, nameof(PersonListViewModel.PullToRefreshCommand));

            var activityIndicator = new ActivityIndicator { AutomationId = AutomationIdConstants.PersonListPage_ActivityIndicator };
            activityIndicator.SetBinding(IsVisibleProperty, nameof(PersonListViewModel.IsDeletingPerson));
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(PersonListViewModel.IsDeletingPerson));

            var whiteOverlayBoxView = new BoxView { BackgroundColor = new Color(1, 1, 1, 0.75) };
            whiteOverlayBoxView.SetBinding(IsVisibleProperty, nameof(PersonListViewModel.IsDeletingPerson));

            var relativeLayout = new RelativeLayout();

            relativeLayout.Children.Add(refreshView,
                                       Constraint.Constant(0),
                                       Constraint.Constant(0));

            relativeLayout.Children.Add(whiteOverlayBoxView,
                                       Constraint.Constant(0),
                                       Constraint.Constant(0),
                                       Constraint.RelativeToParent(parent => parent.Width),
                                       Constraint.RelativeToParent(parent => parent.Height));

            relativeLayout.Children.Add(activityIndicator,
                                       Constraint.RelativeToParent(parent => parent.Width / 2 - getActivityWidth(parent, activityIndicator) / 2),
                                       Constraint.RelativeToParent(parent => parent.Height / 2 - getActivityHeight(parent, activityIndicator) / 2));

            Content = relativeLayout;

            Title = PageTitles.PersonListPage;

            static double getActivityWidth(RelativeLayout parent, View view) => view.Measure(parent.Width, parent.Height).Request.Width;
            static double getActivityHeight(RelativeLayout parent, View view) => view.Measure(parent.Width, parent.Height).Request.Height;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (Content is Layout<View> layout
                && layout.Children.OfType<RefreshView>().First() is RefreshView refreshView)
            {
                refreshView.IsRefreshing = true;
            }
        }

        void HandleErrorTriggered(object sender, string e) =>
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", e, "OK "));


        void HandleSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = (CollectionView)sender;
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
