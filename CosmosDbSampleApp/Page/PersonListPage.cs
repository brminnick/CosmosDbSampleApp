using System;
using System.Linq;
using CosmosDbSampleApp.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Markup;
using static Xamarin.Forms.Markup.GridRowsColumns;

namespace CosmosDbSampleApp
{
    class PersonListPage : BaseContentPage<PersonListViewModel>
    {
        public PersonListPage()
        {
            Title = PageTitles.PersonListPage;
            ViewModel.ErrorTriggered += HandleErrorTriggered;

            ToolbarItems.Add(new ToolbarItem
            {
                IconImageSource = "Add",
                AutomationId = AutomationIdConstants.PersonListPage_AddButton
            }.Invoke(addButtonToolBarItem => addButtonToolBarItem.Clicked += HandleAddButtonClicked));

            Content = new Grid
            {
                RowDefinitions = Rows.Define(Star),
                ColumnDefinitions = Columns.Define(Star),

                Children =
                {
                    new RefreshView
                    {
                        RefreshColor = Color.Black,
                        Content = new CollectionView
                        {
                            ItemTemplate = new PersonListDataTemplate(),
                            BackgroundColor = ColorConstants.PageBackgroundColor,
                            AutomationId = AutomationIdConstants.PersonListPage_PersonList
                        }.Bind(CollectionView.ItemsSourceProperty, nameof(PersonListViewModel.PersonList))
                         .Invoke(personList => personList.SelectionChanged += HandleSelectionChanged)

                    }.Bind(RefreshView.IsRefreshingProperty, nameof(PersonListViewModel.IsRefreshing))
                     .Bind(RefreshView.CommandProperty, nameof(PersonListViewModel.PullToRefreshCommand)),

                    new BoxView { BackgroundColor = new Color(1, 1, 1, 0.75) }.CenterExpand()
                        .Bind(IsVisibleProperty, nameof(PersonListViewModel.IsDeletingPerson)),

                    new ActivityIndicator { AutomationId = AutomationIdConstants.PersonListPage_ActivityIndicator }.Center()
                        .Bind(IsVisibleProperty, nameof(PersonListViewModel.IsDeletingPerson))
                        .Bind(ActivityIndicator.IsRunningProperty, nameof(PersonListViewModel.IsDeletingPerson))
                }
            };
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
