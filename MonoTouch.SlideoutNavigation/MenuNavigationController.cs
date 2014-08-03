using MonoTouch.UIKit;
using System;

namespace MonoTouch.SlideoutNavigation
{
	public class MenuNavigationController : UINavigationController
    {
		private readonly SlideoutNavigationController _slideoutNavigationController;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.MenuNavigationController"/> class.
		/// </summary>
		/// <param name="rootViewController">Root view controller.</param>
		/// <param name="slideoutNavigationController">Slideout navigation controller.</param>
		public MenuNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController)
			: base(rootViewController)
		{
			_slideoutNavigationController = slideoutNavigationController;
		}

		/// <summary>
        /// Pushes the view controller.
		/// </summary>
		/// <param name="viewController">View controller.</param>
		/// <param name="animated">If set to <c>true</c> animated.</param>
		public override void PushViewController (UIViewController viewController, bool animated)
		{
            if (_slideoutNavigationController == null)
            {
                base.PushViewController(viewController, animated);
            }
            else
            {
                _slideoutNavigationController.SetMainViewController(
                    new MainNavigationController(viewController, _slideoutNavigationController), animated);
            }
		}
    }
}

