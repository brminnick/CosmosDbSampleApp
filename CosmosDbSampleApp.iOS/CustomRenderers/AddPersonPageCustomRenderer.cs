using System.Collections.Generic;

using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using CosmosDbSampleApp;
using CosmosDbSampleApp.iOS;


[assembly: ExportRenderer(typeof(AddPersonPage), typeof(AddPersonPageCustomRenderer))]
namespace CosmosDbSampleApp.iOS
{
    public class AddPersonPageCustomRenderer : PageRenderer
    {

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            var thisElement = (AddPersonPage)Element;

            var leftNavList = new List<UIBarButtonItem>();
            var rightNavList = new List<UIBarButtonItem>();

            var navigationItem = NavigationController.TopViewController.NavigationItem;

            for (var i = 0; i < thisElement.ToolbarItems.Count; i++)
            {
                var reorder = (thisElement.ToolbarItems.Count - 1);
                var itemPriority = thisElement.ToolbarItems[reorder - i].Priority;

                if (itemPriority is 1)
                {
                    var leftNavItems = navigationItem?.RightBarButtonItems?[i];

                    if (leftNavItems != null)
                        leftNavList.Add(leftNavItems);
                }
                else if (itemPriority is 0)
                {
                    var rightNavItems = navigationItem?.RightBarButtonItems?[i];

                    if (rightNavItems != null)
                        rightNavList.Add(rightNavItems);
                }
            }

            if (navigationItem != null)
            {
                navigationItem.SetLeftBarButtonItems(leftNavList.ToArray(), false);
                navigationItem.SetRightBarButtonItems(rightNavList.ToArray(), false);
            }
        }
    }
}