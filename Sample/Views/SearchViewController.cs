using System;
using System.Drawing;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Slideout
{
    public partial class SearchViewController : UIViewController
    {
        private string searchText = "No text provided";

        public SearchViewController () : base ("SearchViewController", null)
        {
            this.Title = "Search";
        }

        public SearchViewController (string searchText) : base ("SearchViewController", null)
        {
            this.Title = "Search";

            this.searchText = searchText;
        }

        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
			
            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            searchTextContainer.Text = searchText;
			
            // Perform any additional setup after loading the view, typically from a nib.
        }
    }
}

