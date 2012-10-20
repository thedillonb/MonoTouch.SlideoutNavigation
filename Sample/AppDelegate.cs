using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.SlideoutNavigation;
using MonoTouch.Dialog;
using Slideout.Views;

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
        public SlideoutNavigationController Menu { get; private set; }

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
            Menu = new SlideoutNavigationController();
            Menu.TopView = new HomeViewController();
            Menu.MenuView = new DummyController();

            window.RootViewController = Menu;
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
                new StyledStringElement("Home", () => { NavigationController.PushViewController(new HomeViewController(), true); }),
                new StyledStringElement("About", () => { NavigationController.PushViewController(new AboutViewController(), true); }),
                new StyledStringElement("Stuff", () => { NavigationController.PushViewController(new StuffViewController(), true); }),
                new StyledStringElement("Full Screen", () => { NavigationController.PushViewController(new FullscreenViewController(), true); })
            });
        }
    }
}

