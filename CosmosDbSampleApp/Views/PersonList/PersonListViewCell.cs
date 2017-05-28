using System;

using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListViewCell : ViewCell
    {
        readonly Label _titleLabel, _descriptionLabel;
        readonly MenuItem _deleteAction;

        public PersonListViewCell()
        {
            _titleLabel = new Label
            {
                FontAttributes = FontAttributes.Bold,
            };

            _descriptionLabel = new Label();

            _deleteAction = new MenuItem
            {
                Text = "Delete",
                IsDestructive = true
            };

            var gridLayout = new Grid
            {
                Padding = new Thickness(20, 5),
                RowSpacing = 2,
                ColumnSpacing = 10,

                RowDefinitions = {
                    new RowDefinition{ Height = new GridLength(0, GridUnitType.Auto) },
                    new RowDefinition{ Height = new GridLength(0, GridUnitType.Auto) }

                },
                ColumnDefinitions = {
                    new ColumnDefinition{ Width = new GridLength(0, GridUnitType.Auto) }
                }
            };

            gridLayout.Children.Add(_titleLabel, 0, 0);
            gridLayout.Children.Add(_descriptionLabel, 0, 1);

            View = gridLayout;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            _deleteAction.Clicked += HandleDeleteClicked;
            ContextActions.Add(_deleteAction);
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            _deleteAction.Clicked -= HandleDeleteClicked;
            ContextActions.Remove(_deleteAction);
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var model = BindingContext as PersonModel;

            _titleLabel.Text = model?.Name;
            _descriptionLabel.Text = $"Age {model?.Age.ToString()}";
        }

        async void HandleDeleteClicked(object sender, EventArgs e)
        {
            var personSelected = BindingContext as PersonModel;

            var result = await DocumentDbService.DeletePersonModel(personSelected.Id);

            if (result == null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Invalid DocumentDb Read/Write Key", "Ok");
            }
            else
            {
                var navigationPage = Application.Current.MainPage as NavigationPage;
                var addPersonPage = navigationPage.Navigation.ModalStack as AddPersonPage;
                await addPersonPage.Navigation.PopModalAsync(true);
            }
                
        }
    }
}
