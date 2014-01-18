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
		/// Gets or sets the amount of visible space the menu is given when the user opens it.
		/// This number is how many pixles you want the top view to slide away from the left side.
		/// A good number is typically 280 which gives you 40px of top view still visible in a portrait view
		/// and a lot more in a landscape view.
		/// </summary>
		/// <value>The width of the menu open.</value>
		public float MenuOpenWidth { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.SimpleSlideoutNavigationController"/> class.
		/// </summary>
        public SimpleSlideoutNavigationController()
        {
			MenuOpenWidth = 260f;
        }

		/// <summary>
		/// Animate the specified menuView and mainView based on a percentage.
		/// </summary>
		/// <param name="menuView">The menu view.</param>
		/// <param name="mainView">The main view.</param>
		/// <param name="percentage">The floating point number (0-1) of how far to animate.</param>
		protected override void Animate(UIView menuView, UIView mainView, float percentage)
		{
			var x = View.Bounds.X + (MenuOpenWidth * percentage);
			mainView.Frame = new RectangleF(new PointF(x, mainView.Frame.Y), mainView.Frame.Size);
		}
    }
}

