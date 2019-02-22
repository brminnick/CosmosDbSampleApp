using System;
using System.Net;
using System.Linq;

using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListViewCell : ViewCell
    {
        #region Constant Fields
        readonly Label _titleLabel, _descriptionLabel;
        readonly MenuItem _deleteAction;
        #endregion

        #region Fields
        PersonListPage _personListPage;
        PersonListViewModel _personListViewModel;
        #endregion

        #region Constructors
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
        #endregion

        #region Finalizers
        ~PersonListViewCell()
        {
            ContextActions.Remove(_deleteAction);
            _deleteAction.Clicked -= HandleDeleteClicked;
        }
        #endregion

        #region Properties
        PersonListPage PersonListPage => _personListPage ??
            (_personListPage = GetPersonListPage());

        PersonListViewModel PersonListViewModel => _personListViewModel ??
            (_personListViewModel = GetPersonListViewModel());
        #endregion

        #region Methods

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

            if (PersonListViewModel?.IsDeletingPerson ?? false)
            {
                await Application.Current.MainPage.DisplayAlert("Delete Failed", "Previous Delete Request In Progress", "Ok");
            }
            else
            {
                PersonListViewModel.IsDeletingPerson = true;

                try
                {
                    await DocumentDbService.Delete(personSelected.Id);

                    Device.BeginInvokeOnMainThread(() => PersonListPage?.PersonList?.BeginRefresh());
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

        PersonListViewModel GetPersonListViewModel() => GetPersonListPage().BindingContext as PersonListViewModel;

        PersonListPage GetPersonListPage()
        {
            var navigationPage = Application.Current.MainPage as NavigationPage;
            return navigationPage.Navigation.NavigationStack.FirstOrDefault() as PersonListPage;
        }
        #endregion
    }
}