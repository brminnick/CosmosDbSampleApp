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

            var ageEntry = new CustomReturnEntry
            {
                Placeholder = "Age",
                Keyboard = Keyboard.Numeric,
                ReturnType = ReturnType.Default,
                ReturnCommand = new Command(Unfocus)
            };
            ageEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.AgeEntryText));

            var nameLabel = new Label { Text = "Name" };

            var nameEntry = new CustomReturnEntry
            {
                Placeholder = "Name",
                ReturnType = ReturnType.Next,
                ReturnCommand = new Command(() => ageEntry.Focus())
            };
            nameEntry.SetBinding(Entry.TextProperty, nameof(ViewModel.NameEntryText));

            Padding = GetPageThickness();

            var stackLayout = new StackLayout
            {
                Children ={
                    nameLabel,
                    nameEntry,
                    ageLabel,
                    ageEntry
                }
            };

            Title = "Add Person";

            Content = new ScrollView { Content = stackLayout };

        }

        protected override void SubscribeEventHandlers()
        {
            _cancelButtonToolbarItem.Clicked += HandleCancelButtonToolbarItemClicked;
            ViewModel.SaveCompleted += HandleSaveCompleted;
            ViewModel.Error += HandleError;
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

        Thickness GetPageThickness()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    return new Thickness(20, 20, 20, 0);
                default:
                    return new Thickness(20, 0);
            }
        }
    }
}
