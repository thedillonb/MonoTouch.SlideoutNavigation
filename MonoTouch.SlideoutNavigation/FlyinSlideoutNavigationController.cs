using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace MonoTouch.SlideoutNavigation
{
	/// <summary>
	/// A "Flyin" slideout controller that, using a transformation, makes it look like the top view and menu fly-in from
	/// the right and left, respectively. This makes for a pretty interesting animation!
	/// </summary>
	public class FlyinSlideoutNavigationController : SlideoutNavigationController
    {
		/// <summary>
		/// Gets or sets the zoom scale when the menu is opened.
		/// </summary>
		/// <value>The zoom scale.</value>
		public float ZoomScale { get; set; }

		/// <summary>
		/// Gets or sets the amount of top view visible when the menu is open.
		/// </summary>
		/// <value>The horizontal open offset.</value>
		public float HorizontalOpenOffset { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.FlyinSlideoutNavigationController"/> class.
		/// </summary>
        public FlyinSlideoutNavigationController()
        {
			ZoomScale = 0.1f;
			HorizontalOpenOffset = 40f;
        }

		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			// Because we transform the view it becomes distorted when we rotate. To compensate for this
			// we'd have to do a little work but it becomes unpleasent and I'm not sure if it's good code.
			// Instead, we'll just close the menu. The user must then re-open it which is not that big of a deal.
			// Hopefully, someday, someone will write the correct code to compensate for the rotation - I'm lazy.
			Close(true);

			base.WillRotate(toInterfaceOrientation, duration);
		}

		/// <summary>
		/// Animate the specified menuView and mainView based on a percentage.
		/// </summary>
		/// <param name="menuView">The menu view.</param>
		/// <param name="mainView">The main view.</param>
		/// <param name="percentage">The floating point number (0-1) of how far to animate.</param>
		protected override void Animate(UIView menuView, UIView mainView, float percentage)
		{
			var x = View.Bounds.X + ((View.Bounds.Width - HorizontalOpenOffset) * percentage);
			mainView.Transform = CGAffineTransform.MakeScale(1 - (percentage * 0.6f), 1 - (percentage * 0.6f));
			mainView.Frame = new RectangleF(new PointF(x, mainView.Frame.Y), mainView.Frame.Size);
		}
    }
}

