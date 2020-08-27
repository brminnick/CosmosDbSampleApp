using System;
using System.Threading.Tasks;
using CosmosDbSampleApp.Shared;
using Xamarin.Forms;
using Xamarin.Forms.Markup;

namespace CosmosDbSampleApp
{
    public class AddPersonPage : BaseContentPage<AddPersonViewModel>
    {
        const string _saveButtonToolBarItemText = "Save";
        const string _cancelButtonToolBarItemText = "Cancel";
        readonly AddPersonPageEntry _nameEntry;
        readonly ActivityIndicator _activityIndicator;

        public AddPersonPage()
        {
            ViewModel.SaveErrored += HandleError;
            ViewModel.SaveCompleted += HandleSaveCompleted;

            ToolbarItems.Add(new ToolbarItem
            {
                Text = _saveButtonToolBarItemText,
                Priority = 0,
                AutomationId = AutomationIdConstants.AddPersonPage_SaveButton
            }.Bind(ToolbarItem.CommandProperty, nameof(AddPersonViewModel.SaveButtonCommand)));

            ToolbarItems.Add(new ToolbarItem
            {
                Text = _cancelButtonToolBarItemText,
                Priority = 1,
                AutomationId = AutomationIdConstants.AddPersonPage_CancelButton
            }.Invoke(cancelButton => cancelButton.Clicked += HandleCancelButtonToolbarItemClicked));

            Padding = new Thickness(20, 20, 20, 0);

            Title = PageTitles.AddPersonPage;

            Content = new ScrollView
            {
                Content = new StackLayout
                {
                    Children =
                    {
                        new Label { Text = "Name" },

                        new AddPersonPageEntry
                        {
                            Placeholder = "Name",
                            AutomationId = AutomationIdConstants.AddPersonPage_NameEntry,
                            ReturnType = ReturnType.Next
                        }.Bind(Entry.TextProperty, nameof(AddPersonViewModel.NameEntryText))
                         .Bind<AddPersonPageEntry, bool, bool>(IsEnabledProperty, nameof(AddPersonViewModel.IsBusy), convert: isBusy => !isBusy)
                         .Assign(out _nameEntry),

                        new Label { Text = "Age" },

                        new AddPersonPageEntry
                        {
                            Placeholder = "Age",
                            Keyboard = Keyboard.Numeric,
                            AutomationId = AutomationIdConstants.AddPersonPage_AgeEntry,
                            ReturnType = ReturnType.Go,
                        }.Bind(Entry.TextProperty, nameof(AddPersonViewModel.AgeEntryText))
                         .Bind(Entry.ReturnCommandProperty, nameof(AddPersonViewModel.SaveButtonCommand))
                         .Bind<Entry, bool, bool>(IsEnabledProperty, nameof(AddPersonViewModel.IsBusy), convert: isBusy => !isBusy),

                        new ActivityIndicator
                        {
                            AutomationId = AutomationIdConstants.AddPersonPage_ActivityIndicator
                        }.Bind(IsVisibleProperty, nameof(AddPersonViewModel.IsBusy))
                         .Bind(ActivityIndicator.IsRunningProperty, nameof(AddPersonViewModel.IsBusy))
                         .Assign(out _activityIndicator)
                    }
                }
            };
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
