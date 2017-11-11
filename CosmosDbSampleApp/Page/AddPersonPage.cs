using System;

using Xamarin.Forms;

using EntryCustomReturn.Forms.Plugin.Abstractions;

namespace CosmosDbSampleApp
{
    public class AddPersonPage : BaseContentPage<AddPersonViewModel>
    {
        #region Constant Fields
        const string _saveButtonToolBarItemText = "Save";
        const string _cancelButtonToolBarItemText = "Cancel";
        readonly AddPersonPageEntry _nameEntry;
        readonly ToolbarItem _cancelButtonToolbarItem;
        #endregion  

        public AddPersonPage()
        {
            var saveButtonToolBar = new ToolbarItem
            {
                Text = _saveButtonToolBarItemText,
                Priority = 0,
            };
            saveButtonToolBar.SetBinding(ToolbarItem.CommandProperty, nameof(ViewModel.SaveButtonCommand));
            ToolbarItems.Add(saveButtonToolBar);

            _cancelButtonToolbarItem = new ToolbarItem
            {
                Text = _cancelButtonToolBarItemText,
                Priority = 1
            };
            ToolbarItems.Add(_cancelButtonToolbarItem);

            var ageLabel = new Label { Text = "Age" };

            var ageEntry = new AddPersonPageEntry
            {
                Placeholder = "Age",
                Keyboard = Keyboard.Numeric
            };
            ageEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.AgeEntryText));
            ageEntry.SetBinding(CustomReturnEffect.ReturnCommandProperty, nameof(ViewModel.SaveButtonCommand));
            ageEntry.SetBinding(IsEnabledProperty, new Binding(nameof(ViewModel.IsInternetConnectionActive), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsInternetConnectionActive));
            CustomReturnEffect.SetReturnType(ageEntry, ReturnType.Go);

            var nameLabel = new Label { Text = "Name" };

            _nameEntry = new AddPersonPageEntry { Placeholder = "Name" };
            _nameEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.NameEntryText));
            _nameEntry.SetBinding(IsEnabledProperty, new Binding(nameof(ViewModel.IsInternetConnectionActive), BindingMode.Default, new InverseBooleanConverter(), ViewModel.IsInternetConnectionActive));
            CustomReturnEffect.SetReturnCommand(_nameEntry, new Command(() => ageEntry.Focus()));
            CustomReturnEffect.SetReturnType(_nameEntry, ReturnType.Next);

            var activityIndicator = new ActivityIndicator();
            activityIndicator.SetBinding(IsVisibleProperty, nameof(ViewModel.IsInternetConnectionActive));
            activityIndicator.SetBinding(ActivityIndicator.IsRunningProperty, nameof(ViewModel.IsInternetConnectionActive));

            Padding = GetPageThickness();

            var stackLayout = new StackLayout
            {
                Children ={
                    nameLabel,
                    _nameEntry,
                    ageLabel,
                    ageEntry,
                    activityIndicator
                }
            };

            Title = "Add Person";

            Content = new ScrollView { Content = stackLayout };
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _nameEntry?.Focus();
        }

        protected override void SubscribeEventHandlers()
        {
            _cancelButtonToolbarItem.Clicked += HandleCancelButtonToolbarItemClicked;
            ViewModel.SaveCompleted += HandleSaveCompleted;
            ViewModel.SaveError += HandleError;
        }

        protected override void UnsubscribeEventHandlers()
        {
            _cancelButtonToolbarItem.Clicked -= HandleCancelButtonToolbarItemClicked;
            ViewModel.SaveCompleted += HandleSaveCompleted;
        }

        void HandleCancelButtonToolbarItemClicked(object sender, EventArgs e)
        {
            if (ViewModel.IsInternetConnectionActive)
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
