using System;
using Xamarin.Forms;
namespace CosmosDbSampleApp
{
    public class PersonListTextCell : TextCell
    {
        public PersonListTextCell()
        {
            var model = BindingContext as PersonModel;

            this.SetBinding(TextCell.TextProperty, nameof(model.Name));
            this.SetBinding(TextCell.DetailProperty, nameof(model.Age));
        }
    }
}
