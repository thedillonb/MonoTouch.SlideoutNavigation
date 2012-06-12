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


namespace MonoTouch.Slideout
{
    /// <summary>
    /// Slideout view controller.
    /// </summary>
    public class SlideoutViewController : UIViewController
    {
        private readonly UIViewController _internalContentView;
        private readonly UIViewController _internalMenuView;
        private readonly UITapGestureRecognizer _tapGesture;
        private UIViewController _externalContentView;
        private UIViewController _externalMenuView;
        private UINavigationController _navigation;

        /// <summary>
        /// Gets or sets the current view.
        /// </summary>
        /// <value>
        /// The current view.
        /// </value>
        public UIViewController CurrentView
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
                foreach (var x in _internalMenuView.View.Subviews)
                    x.RemoveFromSuperview();
                foreach (var x in _internalMenuView.ChildViewControllers)
                    x.RemoveFromParentViewController();

                _internalMenuView.AddChildViewController(value);
                _internalMenuView.View.AddSubview(value.View);
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
        public SlideoutViewController()
            : base()
        {
            SlideSpeed = 0.2f;
            SlideWidth = 260f;

            _internalContentView = new UIViewController();
            _internalContentView.View.UserInteractionEnabled = true;
            _internalContentView.View.Layer.MasksToBounds = false;
            _internalContentView.View.Layer.ShadowOffset = new System.Drawing.SizeF(-5, 0);
            _internalContentView.View.Layer.ShadowRadius = 4.0f;
            _internalContentView.View.Layer.ShadowOpacity = 0.5f;
            _internalContentView.View.Layer.ShadowColor = UIColor.Black.CGColor;

            _navigation = new UINavigationController();

            _internalMenuView = new UIViewController();

            _tapGesture = new UITapGestureRecognizer();
            _tapGesture.AddTarget(() => Hide());
            _tapGesture.NumberOfTapsRequired = 1;
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

            _internalContentView.View.Frame = new RectangleF(0, 0, this.View.Frame.Width, this.View.Frame.Height);
            _internalMenuView.View.Frame = new System.Drawing.RectangleF(0, 0, SlideWidth, this.View.Frame.Height);

            //Add the list View
            this.AddChildViewController(_internalMenuView);
            this.View.AddSubview(_internalMenuView.View);

            //Add the parent view
            this.AddChildViewController(_internalContentView);
            this.View.AddSubview(_internalContentView.View);

            //if (CurrentView != null)
            //    SelectView(CurrentView);
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

            var view = _internalContentView.View;
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
            foreach (var x in _internalContentView.View.Subviews)
                x.RemoveFromSuperview();
            foreach (var x in _internalContentView.ChildViewControllers)
                x.RemoveFromParentViewController();

            if (view != null)
            {
                _internalContentView.AddChildViewController(view);
                _internalContentView.View.AddSubview(view.View);
            }

            _externalContentView = view;
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

            var view = _internalContentView.View;
            UIView.Animate(SlideSpeed, 0, UIViewAnimationOptions.CurveEaseInOut, () => {
                view.Frame = new System.Drawing.RectangleF(0, 0, view.Frame.Width, view.Frame.Height);
            }, () => {
                if (view.Subviews.Length > 0)
                    view.Subviews[0].UserInteractionEnabled = true;
                view.RemoveGestureRecognizer(_tapGesture);
            });
        }


    }
}

