using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace MonoTouch.SlideoutNavigation
{
	/// <summary>
	/// A "Simple" slideout controller is a controller in which the top view simply slides
	/// to the right when the user opens the menu. Nothing fancy.
	/// </summary>
	public class SimpleSlideoutNavigationController : SlideoutNavigationController
    {
		/// <summary>
		/// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.SimpleSlideoutNavigationController"/> class.
		/// </summary>
        public SimpleSlideoutNavigationController()
        {
			MenuWidth = 260f;
        }

		/// <summary>
		/// Animate the specified menuView and mainView based on a percentage.
		/// </summary>
		/// <param name="menuView">The menu view.</param>
		/// <param name="mainView">The main view.</param>
		/// <param name="percentage">The floating point number (0-1) of how far to animate.</param>
		protected override void Animate(UIView menuView, UIView mainView, float percentage)
		{
            if (percentage > 1)
                percentage = 1;
			var x = View.Bounds.X + (MenuWidth * percentage);
			mainView.Frame = new RectangleF(new PointF(x, mainView.Frame.Y), mainView.Frame.Size);
		}
    }
}

