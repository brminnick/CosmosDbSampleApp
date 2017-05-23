using System;
using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListPage : BaseContentPage<PersonListViewModel>
    {
        readonly ListView _personList;

        public PersonListPage()
        {
            _personList = new ListView
            {
                ItemTemplate = new DataTemplate(typeof(PersonListTextCell))
            };
            _personList.SetBinding(ListView.ItemsSourceProperty, nameof (ViewModel.PersonList));

            Content = _personList;

            Title = "Person List";
        }

        protected override void SubscribeEventHandlers()
        {
            _personList.ItemTapped += HandleItemTapped;
        }


        protected override void UnsubscribeEventHandlers()
        {
            _personList.ItemTapped -= HandleItemTapped;
        }

		void HandleItemTapped(object sender, ItemTappedEventArgs e)
		{
            var listView = sender as ListView;
            listView.SelectedItem = null;
		}
    }
}
