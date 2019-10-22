using System;
using System.Linq;
using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListViewCell : ViewCell
    {
        readonly Label _titleLabel, _descriptionLabel;
        readonly MenuItem _deleteAction;

        PersonListPage? _personListPage;
        PersonListViewModel? _personListViewModel;

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
            _deleteAction.Clicked += HandleDeleteClicked;
            ContextActions.Add(_deleteAction);

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

        PersonListPage PersonListPage => _personListPage ??= GetPersonListPage();
        PersonListViewModel PersonListViewModel => _personListViewModel ??= GetPersonListViewModel();

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var person = (PersonModel)BindingContext;

            _titleLabel.Text = person.Name;
            _descriptionLabel.Text = $"Age {person.Age.ToString()}";
        }

        async void HandleDeleteClicked(object sender, EventArgs e)
        {
            var personSelected = (PersonModel)BindingContext;

            if (PersonListViewModel?.IsDeletingPerson ?? false)
            {
                await Application.Current.MainPage.DisplayAlert("Delete Failed", "Previous Delete Request In Progress", "Ok");
            }
            else if (PersonListViewModel != null)
            {
                PersonListViewModel.IsDeletingPerson = true;

                try
                {
                    await DocumentDbService.Delete(personSelected.Id);

                    Device.BeginInvokeOnMainThread(() => PersonListPage?.PersonList.BeginRefresh());
                }
                catch (Exception ex)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "Ok");
                    DebugService.PrintException(ex);
                }
                finally
                {
                    PersonListViewModel.IsDeletingPerson = false;
                }
            }
        }

        PersonListViewModel GetPersonListViewModel() => (PersonListViewModel)GetPersonListPage().BindingContext;

        PersonListPage GetPersonListPage()
        {
            var navigationPage = (NavigationPage)Application.Current.MainPage;
            return (PersonListPage)navigationPage.Navigation.NavigationStack.First();
        }
    }
}