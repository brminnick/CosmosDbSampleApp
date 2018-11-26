using System;

using Xamarin.Forms;

using CosmosDbSampleApp.Shared;

namespace CosmosDbSampleApp
{
    public class AddPersonPage : BaseContentPage<AddPersonViewModel>
    {
        #region Constant Fields
        const string _saveButtonToolBarItemText = "Save";
        const string _cancelButtonToolBarItemText = "Cancel";
        readonly AddPersonPageEntry _nameEntry;
        readonly ToolbarItem _cancelButtonToolbarItem;
        readonly ActivityIndicator _activityIndicator;
        #endregion  

        public AddPersonPage()
        {
            var saveButtonToolBar = new ToolbarItem
            {
                Text = _saveButtonToolBarItemText,
                Priority = 0,
                AutomationId = AutomationIdConstants.AddPersonPage_SaveButton
            };
            saveButtonToolBar.SetBinding(ToolbarItem.CommandProperty, nameof(ViewModel.SaveButtonCommand));
            ToolbarItems.Add(saveButtonToolBar);

            _cancelButtonToolbarItem = new ToolbarItem
            {
                Text = _cancelButtonToolBarItemText,
                Priority = 1,
                AutomationId = AutomationIdConstants.AddPersonPage_CancelButton
            };
            ToolbarItems.Add(_cancelButtonToolbarItem);

            var ageLabel = new Label { Text = "Age" };

            var ageEntry = new AddPersonPageEntry
            {
                Placeholder = "Age",
                Keyboard = Keyboard.Numeric,
                AutomationId = AutomationIdConstants.AddPersonPage_AgeEntry,
                ReturnType = ReturnType.Go,
            };
            ageEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.AgeEntryText));
            ageEntry.SetBinding(Entry.ReturnCommandProperty, nameof(ViewModel.SaveButtonCommand));
            ageEntry.SetBinding(IsEnabledProperty, new Binding(nameof(ViewModel.IsBusy), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsBusy));

            var nameLabel = new Label { Text = "Name" };

            _nameEntry = new AddPersonPageEntry
            {
                Placeholder = "Name",
                AutomationId = AutomationIdConstants.AddPersonPage_NameEntry,
                ReturnCommand = new Command(() => ageEntry.Focus()),
                ReturnType = ReturnType.Next
            };
            _nameEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.NameEntryText));
            _nameEntry.SetBinding(IsEnabledProperty, new Binding(nameof(ViewModel.IsBusy), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsBusy));

            _activityIndicator = new ActivityIndicator { AutomationId = AutomationIdConstants.AddPersonPage_ActivityIndicator };
            _activityIndicator.SetBinding(IsVisibleProperty, nameof(ViewModel.IsBusy));
            _activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsBusy));

            Padding = GetPageThickness();

            var stackLayout = new StackLayout
            {
                Children ={
                    nameLabel,
                    _nameEntry,
                    ageLabel,
                    ageEntry,
                    _activityIndicator
                }
            };

            Title = PageTitles.AddPersonPage;

            Content = new ScrollView { Content = stackLayout };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _nameEntry?.Focus();
        }

        protected override void SubscribeEventHandlers()
        {
            ViewModel.SaveErrored += HandleError;
            ViewModel.SaveCompleted += HandleSaveCompleted;
            _cancelButtonToolbarItem.Clicked += HandleCancelButtonToolbarItemClicked;
        }

        protected override void UnsubscribeEventHandlers()
        {
            ViewModel.SaveErrored -= HandleError;
            ViewModel.SaveCompleted += HandleSaveCompleted;
            _cancelButtonToolbarItem.Clicked -= HandleCancelButtonToolbarItemClicked;
        }

        void HandleCancelButtonToolbarItemClicked(object sender, EventArgs e)
        {
            if (_activityIndicator.IsRunning)
                return;

            PopPageFromNavigationStack();
        }

        void HandleError(object sender, string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
                                           await DisplayAlert("Error", message, "ok"));
        }

        void HandleSaveCompleted(object sender, EventArgs e) => PopPageFromNavigationStack();

        void PopPageFromNavigationStack() =>
            Device.BeginInvokeOnMainThread(async () =>
                                           await Navigation.PopModalAsync());

        Thickness GetPageThickness() => new Thickness(20, 20, 20, 0);
    }
}
