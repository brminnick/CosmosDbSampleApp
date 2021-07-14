using Xamarin.CommunityToolkit.Markup;
using Xamarin.Forms;
using static Xamarin.CommunityToolkit.Markup.GridRowsColumns;

namespace CosmosDbSampleApp
{
    class PersonListDataTemplate : DataTemplate
    {
        public PersonListDataTemplate() : base(CreateDataTemplate)
        {
        }

        static Grid CreateDataTemplate() => new()
        {
            RowSpacing = 1,

            RowDefinitions = Rows.Define(
                (Row.Name, 20),
                (Row.Age, 20),
                (Row.Separator, 1)),

            Children =
            {
                new BlackLabel(16).CenterVertical().Font(bold: true)
                    .Row(Row.Name)
                    .Bind(Label.TextProperty, nameof(PersonModel.Name)),

                new BlackLabel(13).Font(italic: true)
                    .Row(Row.Age)
                    .Bind(Label.TextProperty, nameof(PersonModel.Age))
                    .Invoke(detailLabel => detailLabel.Padding = new Thickness(detailLabel.Padding.Left, detailLabel.Padding.Top, detailLabel.Padding.Right, detailLabel.Padding.Bottom + 5)),

                new BoxView { Color = Color.DarkGray }.Margin(5, 0)
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