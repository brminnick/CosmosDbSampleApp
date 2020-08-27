using Xamarin.Forms;
using Xamarin.Forms.Markup;
using static CosmosDbSampleApp.MarkupExtensions;
using static Xamarin.Forms.Markup.GridRowsColumns;

namespace CosmosDbSampleApp
{
    class PersonListDataTemplate : DataTemplate
    {
        public PersonListDataTemplate() : base(CreateDataTemplate)
        {
        }

        static Grid CreateDataTemplate() => new Grid
        {
            RowSpacing = 1,

            RowDefinitions = Rows.Define(
                    (Row.Name, AbsoluteGridLength(20)),
                    (Row.Age, AbsoluteGridLength(20)),
                    (Row.Separator, AbsoluteGridLength(1))),

            Children =
            {
                new BlackLabel(16).CenterVertical().Font(bold: true)
                    .Row(Row.Name)
                    .Bind(Label.TextProperty, nameof(PersonModel.Name)),

                new BlackLabel(13).Font(italic: true)
                    .Row(Row.Age)
                    .Invoke(detailLabel => detailLabel.Padding = new Thickness(detailLabel.Padding.Left, detailLabel.Padding.Top, detailLabel.Padding.Right, detailLabel.Padding.Bottom + 5))
                    .Bind(Label.TextProperty, nameof(PersonModel.Age)),

                new BoxView { Color = Color.DarkGray, Margin = new Thickness(5, 0) }
                    .Row(Row.Separator)
            }
        };

        enum Row { Name, Age, Separator }

        class BlackLabel : Label
        {
            public BlackLabel(double fontSize)
            {
                Padding = new Thickness(10, 0);
                FontSize = fontSize;
                TextColor = Color.Black;
            }
        }
    }
}