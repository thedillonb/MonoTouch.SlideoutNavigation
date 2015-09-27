
using System;

using Foundation;
using UIKit;

namespace Slideout
{
    public class TableViewControllerSource : UITableViewSource
    {
        public TableViewControllerSource()
        {
        }

        public override nint NumberOfSections(UITableView tableView)
        {
            // TODO: return the actual number of sections
            return 1;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            // TODO: return the actual number of items in the section
            return 1;
        }

        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {
            var cell = tableView.DequeueReusableCell(TableViewControllerCell.Key) as TableViewControllerCell;
            if (cell == null)
                cell = new TableViewControllerCell();
			
            // TODO: populate the cell with the appropriate data based on the indexPath
            cell.DetailTextLabel.Text = "Can you delete me?";
			
            return cell;
        }

        public override void CommitEditingStyle(UITableView tableView, UITableViewCellEditingStyle editingStyle, NSIndexPath indexPath)
        {
            
        }

        public override UITableViewCellEditingStyle EditingStyleForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return UITableViewCellEditingStyle.Delete;
        }

        public override bool CanEditRow(UITableView tableView, NSIndexPath indexPath)
        {
            return true;
        }
    }
}

