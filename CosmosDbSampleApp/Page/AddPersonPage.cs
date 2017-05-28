using System;

using Xamarin.Forms;

using EntryCustomReturn.Forms.Plugin.Abstractions;

namespace CosmosDbSampleApp
{
    public class AddPersonPage : BaseContentPage<AddPersonViewModel>
    {
        #region Constant Fields
        const string _saveToolBarItemText = "Save";
        const string _cancelToolBarItemText = "Cancel";
        readonly AddPersonPageEntry _nameEntry;
        readonly ToolbarItem _cancelButtonToolbarItem;
        #endregion  

        public AddPersonPage()
        {
            var saveButtonToolBar = new ToolbarItem
            {
                Text = _saveToolBarItemText,
                Priority = 0,
            };
            saveButtonToolBar.SetBinding(ToolbarItem.CommandProperty, nameof(ViewModel.SaveButtonCommand));
            ToolbarItems.Add(saveButtonToolBar);

            _cancelButtonToolbarItem = new ToolbarItem
            {
                Text = _cancelToolBarItemText,
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
            CustomReturnEffect.SetReturnType(ageEntry, ReturnType.Go);

            var nameLabel = new Label { Text = "Name" };

            _nameEntry = new AddPersonPageEntry { Placeholder = "Name" };
            _nameEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.NameEntryText));
            CustomReturnEffect.SetReturnCommand(_nameEntry, new Command(() => ageEntry.Focus()));
            CustomReturnEffect.SetReturnType(_nameEntry, ReturnType.Next);

            Padding = GetPageThickness();

            var stackLayout = new StackLayout
            {
                Children ={
                    nameLabel,
                    _nameEntry,
                    ageLabel,
                    ageEntry
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
            PopPageFromNavigationStack();
        }

        void HandleError(object sender, string message)
        {
            Device.BeginInvokeOnMainThread(async () =>
                                           await DisplayAlert("Error", message, "ok"));
        }

        void HandleSaveCompleted(object sender, EventArgs e)
        {
            PopPageFromNavigationStack();
        }

        void PopPageFromNavigationStack() =>
            Device.BeginInvokeOnMainThread(async () =>
                                           await Navigation.PopModalAsync());

        Thickness GetPageThickness() => 
            new Thickness(20, 20, 20, 0);
    }
}
