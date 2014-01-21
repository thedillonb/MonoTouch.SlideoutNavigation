using System;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.Foundation;

namespace MonoTouch.SlideoutNavigation
{
	public abstract class SlideoutNavigationController : UIViewController
    {
		private readonly static NSAction EmptyAction = () => { };

		private UIView _containerView;
		private UIViewController _mainViewController;
		private UIViewController _menuViewController;
		private UITapGestureRecognizer _tapGesture;
		private UIPanGestureRecognizer _panGesture;
		private PointF _panFirstTouch;
		private float _panTranslationX;
		private float _panBeganX;
		private float _slideHandleHeight;
		private float _menuWidth;
		private SlideHandle _slideHandle;

		public bool IsOpen { get; private set; }

		public float OpenAnimationDuration { get; set; }

		public float VelocityTrigger { get; set; }

		/// <summary>
		/// Gets or sets the amount of visible space the menu is given when the user opens it.
		/// This number is how many pixles you want the top view to slide away from the left side.
		/// </summary>
		/// <value>The width of the menu open.</value>
		public float MenuWidth
		{
			get { return _menuWidth; }
			set
			{
				_menuWidth = value;
				if (_menuViewController != null)
				{
					var frame = _menuViewController.View.Frame;
					frame.Width = value; 
					_menuViewController.View.Frame = frame;
				}
			}
		}

		public SlideHandle SlideHandle
		{
			get { return _slideHandle; }
			set
			{
				_slideHandle = value;
				if (value == SlideHandle.None)
					_slideHandleHeight = 0;
				else if (value == SlideHandle.NavigationBar)
					_slideHandleHeight = 44f + 20f;
				else if (value == SlideHandle.Full)
					_slideHandleHeight = float.MaxValue;
			}
		}

		protected UIViewAnimationOptions AnimationOption { get; set; }

		protected float SlideHalfwayOffset { get; set; }

		public UIViewController MenuViewController
		{
			get { return _menuViewController; }
			set 
			{ 
				if (IsViewLoaded)
					SetMenuViewController(value, false);
				else
					_menuViewController = value;
			}
		}

		public UIViewController MainViewController
		{
			get { return _mainViewController; }
			set 
			{
				if (IsViewLoaded)
					SetMainViewController(value, false);
				else
					_mainViewController = value;
			}
		}
		
        protected SlideoutNavigationController ()
        {
			OpenAnimationDuration = 0.3f;
			AnimationOption = UIViewAnimationOptions.CurveEaseInOut;
			SlideHandle = SlideHandle.Full;
			SlideHalfwayOffset = 120f;
			VelocityTrigger = 800f;
        }

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_containerView = new UIView(View.Bounds);
			_containerView.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleHeight;
			View.AddSubview(_containerView);

			_tapGesture = new UITapGestureRecognizer();
			_tapGesture.AddTarget (() => Close(true));
			_tapGesture.NumberOfTapsRequired = 1;

			_panGesture = new UIPanGestureRecognizer {
				Delegate = new PanDelegate(this),
				MaximumNumberOfTouches = 1,
				MinimumNumberOfTouches = 1
			};
			_panGesture.AddTarget (() => Pan (_containerView));
			_containerView.AddGestureRecognizer(_panGesture);

			if (_menuViewController != null)
				SetMenuViewController(_menuViewController, false);
			if (_mainViewController != null)
				SetMainViewController(_mainViewController, false);
		}

        /// <summary>
        /// Animate the specified menuView and mainView based on a percentage.
        /// </summary>
        /// <param name="menuView">The menu view.</param>
        /// <param name="mainView">The main view.</param>
        /// <param name="percentage">The floating point number (0-1) of how far to animate.</param>
		protected abstract void Animate(UIView menuView, UIView mainView, float percentage);

		private void Pan (UIView view)
		{
			if (_panGesture.State == UIGestureRecognizerState.Began)
			{
				_panFirstTouch = _panGesture.LocationOfTouch (0, View);
				_panBeganX = view.Frame.X;

				if (!IsOpen)
				{
					if (_menuViewController != null)
						_menuViewController.ViewWillAppear(true);
				}
			}
			else if (_panGesture.State == UIGestureRecognizerState.Changed)
			{
				_panTranslationX = _panGesture.TranslationInView(View).X;
                float total = MenuWidth;
                float numerator = IsOpen ? MenuWidth + _panTranslationX : _panTranslationX;
                float percentage = numerator / total;
				if (percentage < 0)
					percentage = 0;

				NSAction animation = () => Animate(_menuViewController.View, _containerView, percentage);
				UIView.Animate(0.01f, 0, UIViewAnimationOptions.BeginFromCurrentState | UIViewAnimationOptions.AllowUserInteraction, animation, EmptyAction);
			}
			else if (_panGesture.State == UIGestureRecognizerState.Ended || _panGesture.State == UIGestureRecognizerState.Cancelled)
			{
				float velocity = _panGesture.VelocityInView(View).X;
                float total = MenuWidth;
                float numerator = IsOpen ? MenuWidth + _panTranslationX : _panTranslationX;
                float percentage = numerator / total;
				var animationTime = Math.Min(1 / (Math.Abs(velocity) / 100), OpenAnimationDuration);

				if (IsOpen)
				{
					if (percentage > .66f && velocity > -VelocityTrigger)
					{
						NSAction animation = () => Animate(_menuViewController.View, _containerView, 1);
						UIView.Animate(OpenAnimationDuration, 0, AnimationOption, animation, EmptyAction);
					}
					else
						Close(true, animationTime);
				}
				else
				{
					if (percentage < .33f && velocity < VelocityTrigger)
					{
						NSAction animation = () => Animate(_menuViewController.View, _containerView, 0);
						UIView.Animate(OpenAnimationDuration, 0, AnimationOption, animation, EmptyAction);
					}
					else
						Open(true, animationTime);
				}
			}
		}

		public void Open(bool animated)
		{
			Open(animated, OpenAnimationDuration);
		}

		private void Open(bool animated, float animationTime)
		{
			if (IsOpen)
				return;

			if (_menuViewController != null)
				_menuViewController.ViewWillAppear(animated);


			NSAction animation = () => Animate(_menuViewController.View, _containerView, 1);
			NSAction completion = () =>
			{
				IsOpen = true;
				_containerView.AddGestureRecognizer(_tapGesture);
//
				if (_menuViewController != null)
					_menuViewController.ViewDidAppear(animated);
			};


			if (_containerView.Subviews.Length > 0)
				_containerView.Subviews[0].UserInteractionEnabled = false;

			if (animated)
			{
				UIView.Animate(animationTime, 0, AnimationOption, animation, completion);
			}
			else
			{
				animation();
				completion();
			}
		}

		public void Close(bool animated)
		{
			Close(animated, OpenAnimationDuration);
		}

		private void Close(bool animated, float animationTime)
		{
			if (!IsOpen)
				return;

			if (_menuViewController != null)
				_menuViewController.ViewWillDisappear(animated);

			NSAction animation = () => Animate(_menuViewController.View, _containerView, 0);
			NSAction completion = () =>
			{
				IsOpen = false;

				if (_containerView.Subviews.Length > 0)
					_containerView.Subviews[0].UserInteractionEnabled = true;
				_containerView.RemoveGestureRecognizer(_tapGesture);

				if (_menuViewController != null)
					_menuViewController.ViewDidDisappear(animated);
			};

			if (animated)
			{
				UIView.Animate(animationTime, 0, AnimationOption, animation, completion);
			}
			else
			{
				animation();
				completion();
			}
		}

		public void SetMainViewController(UIViewController viewController, bool animated)
		{
			this.AddChildViewController(viewController);

			viewController.View.Frame = _containerView.Bounds;
			_containerView.AddSubview(viewController.View);

			if (_mainViewController != null && viewController != _mainViewController)
			{
				_mainViewController.RemoveFromParentViewController();
				_mainViewController.View.RemoveFromSuperview();
				_mainViewController.DidMoveToParentViewController(null);
			}

			Close(animated);
			_mainViewController = viewController;
		}

		public void SetMenuViewController(UIViewController viewController, bool animated)
		{
			this.AddChildViewController(viewController);
			viewController.View.Frame = new RectangleF(View.Bounds.Location, new SizeF(MenuWidth, View.Bounds.Height));
			viewController.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;
			this.View.InsertSubviewBelow(viewController.View, _containerView);

			if (_menuViewController != null && viewController != _menuViewController)
			{
				_menuViewController.RemoveFromParentViewController();
				_menuViewController.View.RemoveFromSuperview();
				_menuViewController.DidMoveToParentViewController(null);
			}

			Open(animated);
			_menuViewController = viewController;
		}


		public override bool ShouldAutorotate ()
		{
			return true;
		}

		public override UIInterfaceOrientationMask GetSupportedInterfaceOrientations ()
		{
			return UIInterfaceOrientationMask.All;
		}

		///<summary>
		/// A custom UIGestureRecognizerDelegate activated only when the controller 
		/// is visible or touch is within the 44.0f boundary.
		/// 
		/// Special thanks to Gerry High for this snippet!
		///</summary>
		private class PanDelegate : UIGestureRecognizerDelegate
		{
			private readonly SlideoutNavigationController _controller;

			public PanDelegate (SlideoutNavigationController controller)
			{
				_controller = controller;
			}

			public override bool ShouldBegin(UIGestureRecognizer recognizer)
			{
				if (_controller.IsOpen)
					return true;

				var rec = (UIPanGestureRecognizer)recognizer;
				var velocity = rec.VelocityInView(_controller._containerView);
				return Math.Abs(velocity.X) > Math.Abs(velocity.Y);
			}

			public override bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
			{
				return (_controller.IsOpen || (touch.LocationInView (_controller._containerView).Y <= _controller._slideHandleHeight));
			}
		}
    }
}

