//  
//  SlideoutNavigationController.cs
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


namespace MonoTouch.Slideout
{
    public class SlideoutNavigationController : UINavigationController
    {
        private readonly UIPanGestureRecognizer _panGesture;
        private readonly UIViewController _rootViewController;
        private readonly SlideoutViewController _slideoutViewController;


        public SlideoutNavigationController(SlideoutViewController slideoutViewController, UIViewController rootViewController)
            : base(rootViewController)
        {
            _slideoutViewController = slideoutViewController;
            _rootViewController = rootViewController;

            _panGesture = new UIPanGestureRecognizer();
            _panGesture.AddTarget(() => Pan());
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            _rootViewController.NavigationItem.LeftBarButtonItem = new UIBarButtonItem(UIImage.FromBundle("/Images/three_lines.png"), UIBarButtonItemStyle.Plain, (s,e) => {
                _slideoutViewController.Show();
            });
        }

        private void Pan()
        {

        }
    }
}

