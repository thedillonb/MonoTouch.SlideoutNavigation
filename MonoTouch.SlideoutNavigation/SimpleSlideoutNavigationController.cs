using System;
using System.Drawing;
using MonoTouch.UIKit;

namespace MonoTouch.SlideoutNavigation
{
	public class SimpleSlideoutNavigationController : SlideoutNavigationController
    {
		public float HorizontalOpenOffset { get; set; }

        public SimpleSlideoutNavigationController()
        {
			HorizontalOpenOffset = 280f;
        }

		protected override void Animate(UIView menuView, UIView mainView, float percentage)
		{
			var x = View.Bounds.X + (HorizontalOpenOffset * percentage);
			mainView.Frame = new RectangleF(new PointF(x, mainView.Frame.Y), mainView.Frame.Size);
		}
    }
}

