
using System;

using Foundation;
using UIKit;

namespace Slideout
{
    public class TableViewControllerCell : UITableViewCell
    {
        public static readonly NSString Key = new NSString("TableViewControllerCell");

        public TableViewControllerCell() : base(UITableViewCellStyle.Value1, Key)
        {
            // TODO: add subviews to the ContentView, set various colors, etc.
            TextLabel.Text = "TextLabel";
        }
    }
}

