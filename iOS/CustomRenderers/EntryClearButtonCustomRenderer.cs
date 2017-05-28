using UIKit;

using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

using CosmosDbSampleApp.iOS;

[assembly: ExportRenderer(typeof(Entry), typeof(EntryClearButtonCustomRenderer))]
namespace CosmosDbSampleApp.iOS
{
	public class EntryClearButtonCustomRenderer : EntryRenderer
	{
		protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
		{
			base.OnElementChanged(e);

			if (Control == null)
				return;

			Control.ClearButtonMode = UITextFieldViewMode.WhileEditing;
		}
	}
}