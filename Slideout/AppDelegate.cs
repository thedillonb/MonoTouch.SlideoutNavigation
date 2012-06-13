//  
//  AppDelegate.cs
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
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SlideoutNavigation;
using MonoTouch.Dialog;

namespace Slideout.Sample
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register ("AppDelegate")]
    public partial class AppDelegate : UIApplicationDelegate
    {
        // class-level declarations
        UIWindow window;
        SlideoutNavigationController menu;
        UITableViewController ctrl;

        // This is the main entry point of the application.
        static void Main (string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main (args, null, "AppDelegate");
        }

        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching (UIApplication app, NSDictionary options)
        {
            window = new UIWindow (UIScreen.MainScreen.Bounds);
            ctrl = new UITableViewController() { Title = "Hello" };
            menu = new SlideoutNavigationController();
            menu.TopView = ctrl;
            menu.MenuView = new DummyController();

            //Create some sort of menu button
            //menu.CurrentView = nav;


            window.RootViewController = menu;
            window.MakeKeyAndVisible ();

            return true;
        }
    }

    public class DummyController : DialogViewController
    {
        public DummyController() 
            : base(UITableViewStyle.Plain,new RootElement(""))
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            Root.Add(new Section() {
                new StyledStringElement("Home", () => { NavigationController.PushViewController(new UITableViewController() { Title = "Home!" }, true); }),
                new StyledStringElement("About", () => { NavigationController.PushViewController(new UITableViewController() { Title = "About!" }, true); }),
                new StyledStringElement("Stuff", () => { NavigationController.PushViewController(new UITableViewController() { Title = "Stuff!" }, true); })
            });
        }
    }
}

