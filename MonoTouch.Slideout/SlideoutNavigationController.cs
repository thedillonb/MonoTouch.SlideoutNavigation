//  
//  MenuViewController.cs
//  
//  Author:
//       dillonb <>
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

            _internalMenuView = new ProxyNavigationController() { ParentController = this };
            _internalMenuView.SetNavigationBarHidden(true, false);

            _internalTopView = new UIViewController();
            _internalTopView.View.UserInteractionEnabled = true;
            _internalTopView.View.Layer.MasksToBounds = false;
            _internalTopView.View.Layer.ShadowOffset = new System.Drawing.SizeF(-5, 0);
            _internalTopView.View.Layer.ShadowRadius = 4.0f;
            _internalTopView.View.Layer.ShadowOpacity = 0.5f;
            _internalTopView.View.Layer.ShadowColor = UIColor.Black.CGColor;

            _tapGesture = new UITapGestureRecognizer();
            _tapGesture.AddTarget(() => Hide());
            _tapGesture.NumberOfTapsRequired = 1;

            _panGesture = new UIPanGestureRecognizer();
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
                    if (touch.Y > 44.0f)
                        ignorePan = true;
                }

            }
            else if (!ignorePan && (_panGesture.State == UIGestureRecognizerState.Changed))
            {
                var t = _panGesture.TranslationInView(view).X;

                if (t > 0 && Visible)
                    t = 0;
                else if (t < 0 && !Visible)
                    t = 0;

                view.Frame = new RectangleF(panOriginX + t, view.Frame.Y, view.Frame.Width, view.Frame.Height);
            }
            else if (!ignorePan && (_panGesture.State == UIGestureRecognizerState.Ended || _panGesture.State == UIGestureRecognizerState.Cancelled))
            {
                var velocity = Math.Abs(_panGesture.VelocityInView(view).X);
                if (velocity > 800.0f)
                {
                    if (Visible)
                        Hide();
                    else
                        Show();
                    return;
                }

                if (Visible)
                {
                    if (view.Frame.X < _internalMenuView.View.Frame.Width / 2)
                    {
                        Hide();
                        return;
                    }
                    else
                    {
                        UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
                            view.Frame = new System.Drawing.RectangleF(SlideWidth, 0, view.Frame.Width, view.Frame.Height); 
                        }, () => { });
                        return;
                    }
                }
                else
                {
                    if (view.Frame.X > _internalMenuView.View.Frame.Width / 2)
                    {
                        Show();
                        return;
                    }
                    else
                    {
                        UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
                            view.Frame = new System.Drawing.RectangleF(0, 0, view.Frame.Width, view.Frame.Height);
                        }, () => { });
                        return;
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

        /// <summary>
        /// Show this instance.
        /// </summary>
        public void Show()
        {
            //Don't show if already shown
            if (Visible)
                return;
            Visible = true;

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

    }
}

