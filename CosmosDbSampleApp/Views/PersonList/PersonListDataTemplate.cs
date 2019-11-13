using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class PersonListDataTemplate : DataTemplate
    {
        public PersonListDataTemplate() : base(CreateDataTemplate)
        {
        }

        static Grid CreateDataTemplate()
        {
            var titleLabel = new BlackLabel(16)
            {
                VerticalTextAlignment = TextAlignment.Start,
                FontAttributes = FontAttributes.Bold
            };
            titleLabel.SetBinding(Label.TextProperty, nameof(PersonModel.Name));

            var detailLabel = new BlackLabel(13)
            {
                FontAttributes = FontAttributes.Italic,
            };
            detailLabel.Padding = new Thickness(detailLabel.Padding.Left, detailLabel.Padding.Top, detailLabel.Padding.Right, detailLabel.Padding.Bottom + 5);
            detailLabel.SetBinding(Label.TextProperty, nameof(PersonModel.Age));

            var dividingLine = new BoxView
            {
                Color = Color.DarkGray,
                Margin = new Thickness(5, 0)
            };

            var grid = new Grid
            {
                RowSpacing = 1,

                RowDefinitions =
                {
                    new RowDefinition { Height = new GridLength(20, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(20, GridUnitType.Absolute) },
                    new RowDefinition { Height = new GridLength(1, GridUnitType.Absolute) },
                },
                ColumnDefinitions =
                {
                    new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
                }
            };

            grid.Children.Add(titleLabel, 0, 0);
            grid.Children.Add(detailLabel, 0, 1);
            grid.Children.Add(dividingLine, 0, 2);

            return grid;
        }

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