//  
//  MenuViewController.cs
//  
//  Author:
//       dillonb <thedillonb@gmail.com>
// 
//  Copyright (c) 2012 dillonb
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
// 
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;


namespace MonoTouch.SlideoutNavigation
{
    /// <summary>
    /// Slideout view controller.
    /// </summary>
    public class SlideoutNavigationController : UIViewController
    {
        private readonly UIViewController _internalTopView;
        private readonly ProxyNavigationController _internalMenuView;
        private readonly UITapGestureRecognizer _tapGesture;
        private readonly UIPanGestureRecognizer _panGesture;

        private UINavigationController _internalTopNavigation;
        private UIViewController _externalContentView;
        private UIViewController _externalMenuView;
        private bool _shadowShown;

        public float SlideHeight { get; set; }

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
            public override void PushViewController (UIViewController viewController, bool animated)
            {
                ParentController.SelectView(viewController);
            }
        }

        ///<summary>
        /// A custom UIGestureRecognizerDelegate activated only when the controller 
        /// is visible or touch is within the 44.0f boundary.
        /// 
        /// Special thanks to Gerry High for this snippet!
        ///</summary>
        private class SlideoutPanDelegate : UIGestureRecognizerDelegate
        {
            private SlideoutNavigationController _controller;

            public SlideoutPanDelegate(SlideoutNavigationController controller)
            {
                _controller = controller;
            }

            public override bool ShouldReceiveTouch (UIGestureRecognizer recognizer, UITouch touch)
            {
                return (this._controller.Visible || (touch.LocationInView(this._controller._internalTopView.View).Y <= this._controller.SlideHeight));
            }
        }

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
        /// Gets a value indicating whether this <see cref="MonoTouch.Slideout.SlideoutViewController"/> is visible.
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
        /// Initializes a new instance of the <see cref="MonoTouch.Slideout.SlideoutViewController"/> class.
        /// </summary>
        public SlideoutNavigationController()
        {
            SlideSpeed = 0.2f;
            SlideWidth = 260f;
            SlideHeight = 44f;
            LayerShadowing = true;

            _internalMenuView = new ProxyNavigationController() { ParentController = this };
            //_internalMenuView.SetNavigationBarHidden(true, false);
            _internalMenuView.View.AutoresizingMask = UIViewAutoresizing.FlexibleHeight;

            _internalTopView = new UIViewController();
            _internalTopView.View.UserInteractionEnabled = true;
            _internalTopView.View.Layer.MasksToBounds = false;

            _tapGesture = new UITapGestureRecognizer();
            _tapGesture.AddTarget(() => Hide());
            _tapGesture.NumberOfTapsRequired = 1;

            _panGesture = new UIPanGestureRecognizer();
			_panGesture.Delegate = new SlideoutPanDelegate(this);
            _panGesture.MaximumNumberOfTouches = 1;
            _panGesture.MinimumNumberOfTouches = 1;
            _panGesture.AddTarget(() => Pan(_internalTopView.View));
            _internalTopView.View.AddGestureRecognizer(_panGesture);
        }

        //Helper variables for the method below
        private float panOriginX = 0.0f;
        private bool ignorePan = false;

        /// <summary>
        /// Pan the specified view.
        /// </summary>
        /// <param name='view'>
        /// View.
        /// </param>
        private void Pan(UIView view)
        {
            if (_panGesture.State == UIGestureRecognizerState.Began)
            {
                panOriginX = view.Frame.X;
                ignorePan = false;

                if (!Visible)
                {
                    var touch = _panGesture.LocationOfTouch(0, view);
                    if (touch.Y > SlideHeight || _internalTopNavigation.NavigationBarHidden)
                        ignorePan = true;
                }

            }
            else if (!ignorePan && (_panGesture.State == UIGestureRecognizerState.Changed))
            {
                var t = _panGesture.TranslationInView(view).X;

                if (t > 0 && Visible)
                    t = 0;
                else if (t < -_internalMenuView.View.Bounds.Width && Visible)
                    t = -_internalMenuView.View.Bounds.Width;
                else if (t < 0 && !Visible)
                    t = 0;
                else if (t > _internalMenuView.View.Bounds.Width && !Visible)
                    t = _internalMenuView.View.Bounds.Width;

                view.Frame = new RectangleF(panOriginX + t, view.Frame.Y, view.Frame.Width, view.Frame.Height);

                //Make sure the shadow is shown while we move the frame around!
                ShowShadow();
            }
            else if (!ignorePan && (_panGesture.State == UIGestureRecognizerState.Ended || _panGesture.State == UIGestureRecognizerState.Cancelled))
            {
                var velocity = _panGesture.VelocityInView(view).X;

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
                        UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
                            view.Frame = new System.Drawing.RectangleF(SlideWidth, 0, view.Frame.Width, view.Frame.Height); 
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
                        UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
                            view.Frame = new System.Drawing.RectangleF(0, 0, view.Frame.Width, view.Frame.Height);
                        }, () => { });
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
            base.ViewDidLoad ();

            _internalTopView.View.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            _internalMenuView.View.Frame = new System.Drawing.RectangleF(0, 0, SlideWidth, this.View.Frame.Height);

            //Add the list View
            this.AddChildViewController(_internalMenuView);
            this.View.AddSubview(_internalMenuView.View);

            //Add the parent view
            this.AddChildViewController(_internalTopView);
            this.View.AddSubview(_internalTopView.View);
        }

		public override void ViewWillAppear (bool animated)
		{
			base.ViewWillAppear (animated);
			if (NavigationController != null)
				NavigationController.SetNavigationBarHidden(true, true);
		}

		public override void ViewWillDisappear (bool animated)
		{
			base.ViewWillDisappear (animated);
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

            _internalTopView.View.Layer.ShadowOffset = new System.Drawing.SizeF(-5, 0);
            _internalTopView.View.Layer.ShadowPath = UIBezierPath.FromRect(_internalTopView.View.Bounds).CGPath;
            _internalTopView.View.Layer.ShadowRadius = 4.0f;
            _internalTopView.View.Layer.ShadowOpacity = 0.5f;
            _internalTopView.View.Layer.ShadowColor = UIColor.Black.CGColor;

            _shadowShown = true;
        }

        /// <summary>
        /// Hides the shadow of the top view
        /// </summary>
        private void HideShadow ()
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

            var view = _internalTopView.View;
            UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
                view.Frame = new System.Drawing.RectangleF(SlideWidth, 0, view.Frame.Width, view.Frame.Height);
            }, () => {
                if (view.Subviews.Length > 0)
                    view.Subviews[0].UserInteractionEnabled = false;
                view.AddGestureRecognizer(_tapGesture);
            }); 
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

            _internalTopNavigation = new UINavigationController(view);
            _internalTopNavigation.View.Frame = new RectangleF(0, 0, _internalTopView.View.Frame.Width, _internalTopView.View.Frame.Height);
            _internalTopView.AddChildViewController(_internalTopNavigation);
            _internalTopView.View.AddSubview(_internalTopNavigation.View);

            view.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("/Images/three_lines.png"), UIBarButtonItemStyle.Plain, (s,e) => {
                Show();
            });

            _externalContentView = view;

            Hide ();
        }

        /// <summary>
        /// Hide this instance.
        /// </summary>
        public void Hide()
        {
            //Don't hide if its not visible.
            if (!Visible)
                return;
            Visible = false;



            var view = _internalTopView.View;
            UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
                view.Frame = new System.Drawing.RectangleF(0, 0, view.Frame.Width, view.Frame.Height);
            }, () => {
                if (view.Subviews.Length > 0)
                    view.Subviews[0].UserInteractionEnabled = true;
                view.RemoveGestureRecognizer(_tapGesture);

                //Hide the shadow when not needed to increase performance of the top layer!
                HideShadow();
            });
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
        public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
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

    }
}

