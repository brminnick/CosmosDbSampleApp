using System;
using Xamarin.Forms;

namespace CosmosDbSampleApp
{
    public class AddPersonPageEntry : Entry
    {
        public AddPersonPageEntry()
        {
            switch (Device.RuntimePlatform)
            {
                case Device.iOS:
                    BackgroundColor = ColorConstants.EntryBackgroundColor;
                    Margin = new Thickness(0, 0, 0, 5);
                    break;

                case Device.Android:
                    Margin = new Thickness(0, 0, 0, 10);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
