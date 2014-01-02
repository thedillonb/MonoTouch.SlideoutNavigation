using System;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;
using System.Drawing;

namespace MonoTouch.SlideoutNavigation
{
	public class FlyinSlideoutNavigationController : SlideoutNavigationController
    {
		public float ZoomScale { get; set; }

		public float HorizontalOpenOffset { get; set; }

        public FlyinSlideoutNavigationController()
        {
			ZoomScale = 0.1f;
			HorizontalOpenOffset = 280f;
        }

		protected override void Animate(UIView menuView, UIView mainView, float percentage)
		{
			var x = View.Bounds.X + (HorizontalOpenOffset * percentage);
			mainView.Transform = CGAffineTransform.MakeScale(1 - (percentage * 0.6f), 1 - (percentage * 0.6f));
			mainView.Frame = new RectangleF(new PointF(x, mainView.Frame.Y), mainView.Frame.Size);
		}
    }
}

