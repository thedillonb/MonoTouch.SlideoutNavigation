using MonoTouch.UIKit;

namespace MonoTouch.SlideoutNavigation
{
	public class MainNavigationController : UINavigationController
	{
		private readonly SlideoutNavigationController _slideoutNavigationController;

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.MenuNavigationController"/> class.
		/// </summary>
		/// <param name="rootViewController">Root view controller.</param>
		/// <param name="slideoutNavigationController">Slideout navigation controller.</param>
		public MainNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController)
			: this(rootViewController, slideoutNavigationController, null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="MonoTouch.SlideoutNavigation.MainNavigationController"/> class.
		/// </summary>
		/// <param name="rootViewController">Root view controller.</param>
		/// <param name="slideoutNavigationController">Slideout navigation controller.</param>
		/// <param name="openMenuButton">Open menu button.</param>
		public MainNavigationController(UIViewController rootViewController, SlideoutNavigationController slideoutNavigationController, UIBarButtonItem openMenuButton)
			: base(rootViewController)
		{
			_slideoutNavigationController = slideoutNavigationController;
			openMenuButton = openMenuButton ?? new UIBarButtonItem("Menu", UIBarButtonItemStyle.Plain, (s, e) => _slideoutNavigationController.Open(true));
			rootViewController.NavigationItem.LeftBarButtonItem = openMenuButton;
		}
	}
}

