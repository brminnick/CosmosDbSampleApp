using System;
using System.Threading.Tasks;

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
        readonly ActivityIndicator _activityIndicator;
        #endregion  

        public AddPersonPage()
        {
            ViewModel.SaveErrored += HandleError;
            ViewModel.SaveCompleted += HandleSaveCompleted;

            var saveButtonToolBar = new ToolbarItem
            {
                Text = _saveButtonToolBarItemText,
                Priority = 0,
                AutomationId = AutomationIdConstants.AddPersonPage_SaveButton
            };
            saveButtonToolBar.SetBinding(ToolbarItem.CommandProperty, nameof(AddPersonViewModel.SaveButtonCommand));
            ToolbarItems.Add(saveButtonToolBar);

            var cancelButtonToolbarItem = new ToolbarItem
            {
                Text = _cancelButtonToolBarItemText,
                Priority = 1,
                AutomationId = AutomationIdConstants.AddPersonPage_CancelButton
            };
            cancelButtonToolbarItem.Clicked += HandleCancelButtonToolbarItemClicked;
            ToolbarItems.Add(cancelButtonToolbarItem);

            var ageLabel = new Label { Text = "Age" };

            var ageEntry = new AddPersonPageEntry
            {
                Placeholder = "Age",
                Keyboard = Keyboard.Numeric,
                AutomationId = AutomationIdConstants.AddPersonPage_AgeEntry,
                ReturnType = ReturnType.Go,
            };
            ageEntry.SetBinding(Entry.TextProperty, nameof(AddPersonViewModel.AgeEntryText));
            ageEntry.SetBinding(Entry.ReturnCommandProperty, nameof(AddPersonViewModel.SaveButtonCommand));
            ageEntry.SetBinding(IsEnabledProperty, new Binding(nameof(AddPersonViewModel.IsBusy), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsBusy));

            var nameLabel = new Label { Text = "Name" };

            _nameEntry = new AddPersonPageEntry
            {
                Placeholder = "Name",
                AutomationId = AutomationIdConstants.AddPersonPage_NameEntry,
                ReturnCommand = new Command(() => ageEntry.Focus()),
                ReturnType = ReturnType.Next
            };
            _nameEntry.SetBinding(Entry.TextProperty, nameof(AddPersonViewModel.NameEntryText));
            _nameEntry.SetBinding(IsEnabledProperty, new Binding(nameof(AddPersonViewModel.IsBusy), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsBusy));

            _activityIndicator = new ActivityIndicator { AutomationId = AutomationIdConstants.AddPersonPage_ActivityIndicator };
            _activityIndicator.SetBinding(IsVisibleProperty, nameof(AddPersonViewModel.IsBusy));
            _activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(AddPersonViewModel.IsBusy));

            Padding = new Thickness(20, 20, 20, 0);

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

            Device.BeginInvokeOnMainThread(() => _nameEntry.Focus());
        }

        async void HandleCancelButtonToolbarItemClicked(object sender, EventArgs e)
        {
            if (_activityIndicator.IsRunning)
                return;

            await PopPageFromNavigationStack();
        }

        void HandleError(object sender, string message)
        {
            Device.BeginInvokeOnMainThread(async () => await DisplayAlert("Error", message, "ok"));
        }

        async void HandleSaveCompleted(object sender, EventArgs e) => await PopPageFromNavigationStack();

        Task PopPageFromNavigationStack() => Device.InvokeOnMainThreadAsync(Navigation.PopModalAsync);
    }
}
