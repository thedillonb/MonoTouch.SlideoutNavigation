using System.Drawing;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace MonoTouch.SlideoutNavigation
{
    /// <summary>
    /// Slideout view controller.
    /// </summary>
    public class SlideoutNavigationController : UIViewController
    {
        private readonly ProxyNavigationController _internalMenuView;
        private readonly UIViewController _internalTopView;
        private readonly UIPanGestureRecognizer _panGesture;
        private readonly UITapGestureRecognizer _tapGesture;

        private UIViewController _externalContentView;
        private UIViewController _externalMenuView;
        private bool _ignorePan;
        private UINavigationController _internalTopNavigation;
        private float _panOriginX;
        private bool _shadowShown;
        private bool _menuEnabled = true;

        /// <summary>
        /// Initializes a new instance of the <see cref="SlideoutNavigationController"/> class.
        /// </summary>
        public SlideoutNavigationController()
        {
            SlideSpeed = 0.2f;
            SlideWidth = 260f;
            SlideHeight = 44f;
            LayerShadowing = true;

            _internalMenuView = new ProxyNavigationController
                                    {
                                        ParentController = this,
                                        View = { AutoresizingMask = UIViewAutoresizing.FlexibleHeight }
                                    };
            //_internalMenuView.SetNavigationBarHidden(true, false);

            _internalTopView = new UIViewController { View = { UserInteractionEnabled = true } };
            _internalTopView.View.Layer.MasksToBounds = false;

            _tapGesture = new UITapGestureRecognizer();
            _tapGesture.AddTarget(Hide);
            _tapGesture.NumberOfTapsRequired = 1;

            _panGesture = new UIPanGestureRecognizer
                              {
                                  Delegate = new SlideoutPanDelegate(this),
                                  MaximumNumberOfTouches = 1,
                                  MinimumNumberOfTouches = 1
                              };
            _panGesture.AddTarget(() => Pan(_internalTopView.View));
            _internalTopView.View.AddGestureRecognizer(_panGesture);
        }

        public float SlideHeight { get; set; }

        /// <summary>
        /// Gets or sets the current view.
        /// </summary>
        /// <value>
        /// The current view.
        /// </value>
        public UIViewController TopView
        {
            get { return _externalContentView; }
            set
            {
                if (_externalContentView == value)
                    return;
                SelectView(value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this
        /// <see cref="MonoTouch.SlideoutNavigation.SlideoutNavigationController"/> menu enabled.
        /// If this is true then you can reach the menu. If false then all hooks to get to the menu view will be disabled.
        /// This is only necessary when you don't want the user to get to the menu.
        /// </summary>
        public bool MenuEnabled
        {
            get { return _menuEnabled; }
            set
            {
                if (value == _menuEnabled)
                    return;

                if (!value)
                    Hide();

                if (_internalTopNavigation != null && _internalTopNavigation.ViewControllers.Length > 0)
                {
                    var view = _internalTopNavigation.ViewControllers[0];
                    view.NavigationItem.LeftBarButtonItem = value ? CreateMenuButton() : null;
                }

                _menuEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the list view.
        /// </summary>
        /// <value>
        /// The list view.
        /// </value>
        public UIViewController MenuView
        {
            get { return _externalMenuView; }
            set
            {
                if (_externalMenuView == value)
                    return;
                _internalMenuView.SetController(value);
                _externalMenuView = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether there should be shadowing effects on the top view
        /// </summary>
        /// <value>
        /// <c>true</c> if layer shadowing; otherwise, <c>false</c>.
        /// </value>
        public bool LayerShadowing { get; set; }

        /// <summary>
        /// Gets or sets the slide speed.
        /// </summary>
        /// <value>
        /// The slide speed.
        /// </value>
        public float SlideSpeed { get; set; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="SlideoutNavigationController"/> is visible.
        /// </summary>
        /// <value>
        /// <c>true</c> if visible; otherwise, <c>false</c>.
        /// </value>
        public bool Visible { get; private set; }

        /// <summary>
        /// Gets or sets the width of the slide.
        /// </summary>
        /// <value>
        /// The width of the slide.
        /// </value>
        public float SlideWidth { get; set; }

        /// <summary>
        /// Pan the specified view.
        /// </summary>
        /// <param name='view'>
        /// View.
        /// </param>
        private void Pan(UIView view)
        {
            if (!MenuEnabled)
                return;

            if (_panGesture.State == UIGestureRecognizerState.Began)
            {
                _panOriginX = view.Frame.X;
                _ignorePan = false;

                if (!Visible)
                {
                    PointF touch = _panGesture.LocationOfTouch(0, view);
                    if (touch.Y > SlideHeight || _internalTopNavigation.NavigationBarHidden)
                        _ignorePan = true;
                }
            }
            else if (!_ignorePan && (_panGesture.State == UIGestureRecognizerState.Changed))
            {
                float t = _panGesture.TranslationInView(view).X;

                if (t > 0 && Visible)
                    t = 0;
                else if (t < -_internalMenuView.View.Bounds.Width && Visible)
                    t = -_internalMenuView.View.Bounds.Width;
                else if (t < 0 && !Visible)
                    t = 0;
                else if (t > _internalMenuView.View.Bounds.Width && !Visible)
                    t = _internalMenuView.View.Bounds.Width;

                view.Frame = new RectangleF(_panOriginX + t, view.Frame.Y, view.Frame.Width, view.Frame.Height);

                //Make sure the shadow is shown while we move the frame around!
                ShowShadow();
            }
            else if (!_ignorePan &&
                     (_panGesture.State == UIGestureRecognizerState.Ended ||
                      _panGesture.State == UIGestureRecognizerState.Cancelled))
            {
                float velocity = _panGesture.VelocityInView(view).X;

                if (Visible)
                {
                    if (velocity < -800.0f)
                    {
                        Hide();
                    }
                    else if (view.Frame.X < _internalMenuView.View.Frame.Width / 2)
                    {
                        Hide();
                    }
                    else
                    {
                        UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
                                       () =>
                                       {
                                           view.Frame = new RectangleF(SlideWidth, 0, view.Frame.Width,
                                                                       view.Frame.Height);
                                       }, () => { });
                    }
                }
                else
                {
                    if (velocity > 800.0f)
                    {
                        Show();
                    }
                    else if (view.Frame.X > _internalMenuView.View.Frame.Width / 2)
                    {
                        Show();
                    }
                    else
                    {
                        UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
                                       () => { view.Frame = new RectangleF(0, 0, view.Frame.Width, view.Frame.Height); }, () => { });
                    }
                }
            }
        }

        /// <Docs>
        /// Called after the controller’s view is loaded into memory.
        /// </Docs>
        /// <summary>
        /// Views the did load.
        /// </summary>
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            _internalTopView.View.Frame = new RectangleF(0, 0, View.Frame.Width, View.Frame.Height);
            _internalMenuView.View.Frame = new RectangleF(0, 0, SlideWidth, View.Frame.Height);

            //Add the list View
            AddChildViewController(_internalMenuView);
            View.AddSubview(_internalMenuView.View);

            //Add the parent view
            AddChildViewController(_internalTopView);
            View.AddSubview(_internalTopView.View);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            if (NavigationController != null)
                NavigationController.SetNavigationBarHidden(true, true);
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);
            if (NavigationController != null)
                NavigationController.SetNavigationBarHidden(false, true);
        }

        /// <summary>
        /// Shows the shadow of the top view!
        /// </summary>
        private void ShowShadow()
        {
            //Dont need to call this twice if its already shown
            if (!LayerShadowing || _shadowShown)
                return;

            _internalTopView.View.Layer.ShadowOffset = new SizeF(-5, 0);
            _internalTopView.View.Layer.ShadowPath = UIBezierPath.FromRect(_internalTopView.View.Bounds).CGPath;
            _internalTopView.View.Layer.ShadowRadius = 4.0f;
            _internalTopView.View.Layer.ShadowOpacity = 0.5f;
            _internalTopView.View.Layer.ShadowColor = UIColor.Black.CGColor;

            _shadowShown = true;
        }

        /// <summary>
        /// Hides the shadow of the top view
        /// </summary>
        private void HideShadow()
        {
            //Dont need to call this twice if its already hidden
            if (!LayerShadowing || !_shadowShown)
                return;

            _internalTopView.View.Layer.ShadowOffset = new SizeF(0, 0);
            _internalTopView.View.Layer.ShadowRadius = 0.0f;
            _internalTopView.View.Layer.ShadowOpacity = 0.0f;
            _internalTopView.View.Layer.ShadowColor = UIColor.Clear.CGColor;
            _shadowShown = false;
        }

        /// <summary>
        /// Show this instance.
        /// </summary>
        public void Show()
        {
            //Don't show if already shown
            if (Visible)
                return;
            Visible = true;

            //Show some shadow!
            ShowShadow();

            UIView view = _internalTopView.View;
            UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut,
                           () => { view.Frame = new RectangleF(SlideWidth, 0, view.Frame.Width, view.Frame.Height); },
                           () =>
                           {
                               if (view.Subviews.Length > 0)
                                   view.Subviews[0].UserInteractionEnabled = false;
                               view.AddGestureRecognizer(_tapGesture);
                           });
        }

        /// <summary>
        /// Creates the menu button.
        /// </summary>
        protected virtual UIBarButtonItem CreateMenuButton()
        {
            return new UIBarButtonItem("Menu", UIBarButtonItemStyle.Plain, (s, e) => Show());
        }

        /// <summary>
        /// Selects the view.
        /// </summary>
        /// <param name='view'>
        /// View.
        /// </param>
        public void SelectView(UIViewController view)
        {
            if (_internalTopNavigation != null)
            {
                _internalTopNavigation.RemoveFromParentViewController();
                _internalTopNavigation.View.RemoveFromSuperview();
                _internalTopNavigation.Dispose();
            }

            _internalTopNavigation = new UINavigationController(view)
                                         {
                                             View =
                                             {
                                                 Frame = new RectangleF(0, 0,
                                                                        _internalTopView.View.Frame.Width,
                                                                        _internalTopView.View.Frame.Height)
                                             }
                                         };
            _internalTopView.AddChildViewController(_internalTopNavigation);
            _internalTopView.View.AddSubview(_internalTopNavigation.View);

            if (MenuEnabled)
                view.NavigationItem.LeftBarButtonItem = CreateMenuButton();

            _externalContentView = view;

            Hide();
        }

        /// <summary>
        /// Hide this instance.
        /// </summary>
        public void Hide(bool animate = true)
        {
            //Don't hide if its not visible.
            if (!Visible)
                return;
            Visible = false;

            UIView view = _internalTopView.View;

            NSAction animation = () => { view.Frame = new RectangleF(0, 0, view.Frame.Width, view.Frame.Height); };
            NSAction finished = () => {
                if (view.Subviews.Length > 0)
                view.Subviews[0].UserInteractionEnabled = true;
                view.RemoveGestureRecognizer(_tapGesture);
                //Hide the shadow when not needed to increase performance of the top layer!
                HideShadow();
            };

            if (animate)
                UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut, animation, finished);
            else
            {
                animation();
                finished();
            }
        }

        /// <summary>
        /// Shoulds the autorotate to interface orientation.
        /// </summary>
        /// <returns>
        /// The autorotate to interface orientation.
        /// </returns>
        /// <param name='toInterfaceOrientation'>
        /// If set to <c>true</c> to interface orientation.
        /// </param>
        public override bool ShouldAutorotateToInterfaceOrientation(UIInterfaceOrientation toInterfaceOrientation)
        {
            return true;
        }

        /// <summary>
        /// Sets the menu navigation background image.
        /// </summary>
        /// <param name='image'>Image to be displayed as the background</param>
        /// <param name='metrics'>Metrics.</param>
        public void SetMenuNavigationBackgroundImage(UIImage image, UIBarMetrics metrics)
        {
            _internalMenuView.NavigationBar.SetBackgroundImage(image, metrics);
        }

        /// <summary>
        /// Sets the top view navigation background image.
        /// </summary>
        /// <param name='image'>Image to be displayed as the background</param>
        /// <param name='metrics'>Metrics.</param>
        public void SetTopNavigationBackgroundImage(UIImage image, UIBarMetrics metrics)
        {
            _internalTopNavigation.NavigationBar.SetBackgroundImage(image, metrics);
        }

        #region Nested type: ProxyNavigationController

        ///<summary>
        /// A proxy class for the navigation controller.
        /// This allows the menu view to make requests to the navigation controller
        /// and have them forwarded to the topview.
        ///</summary>
        private class ProxyNavigationController : UINavigationController
        {
            /// <summary>
            /// Gets or sets the parent controller.
            /// </summary>
            /// <value>
            /// The parent controller.
            /// </value>
            public SlideoutNavigationController ParentController { get; set; }

            /// <summary>
            /// Sets the controller.
            /// </summary>
            /// <param name='viewController'>
            /// View controller.
            /// </param>
            public void SetController(UIViewController viewController)
            {
                base.PopToRootViewController(false);
                base.PushViewController(viewController, false);
            }

            /// <Docs>
            /// To be added.
            /// </Docs>
            /// <summary>
            /// To be added.
            /// </summary>
            /// <param name='viewController'>
            /// View controller.
            /// </param>
            /// <param name='animated'>
            /// Animated.
            /// </param>
            public override void PushViewController(UIViewController viewController, bool animated)
            {
                ParentController.SelectView(viewController);
            }
        }

        #endregion

        #region Nested type: SlideoutPanDelegate

        ///<summary>
        /// A custom UIGestureRecognizerDelegate activated only when the controller 
        /// is visible or touch is within the 44.0f boundary.
        /// 
        /// Special thanks to Gerry High for this snippet!
        ///</summary>
        private class SlideoutPanDelegate : UIGestureRecognizerDelegate
        {
            private readonly SlideoutNavigationController _controller;

            public SlideoutPanDelegate(SlideoutNavigationController controller)
            {
                _controller = controller;
            }

            public override bool ShouldReceiveTouch(UIGestureRecognizer recognizer, UITouch touch)
            {
                return (_controller.Visible ||
                        (touch.LocationInView(_controller._internalTopView.View).Y <= _controller.SlideHeight)) && _controller.MenuEnabled;
            }
        }

        #endregion
    }
}