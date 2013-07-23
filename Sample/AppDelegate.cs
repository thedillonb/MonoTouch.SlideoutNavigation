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
            Menu = new SlideoutNavigationController ();
            Menu.TopView = new HomeViewController ();
            Menu.MenuViewLeft = new DummyControllerLeft ();
            Menu.MenuViewRight = new DummyControllerRight ();

            window.RootViewController = Menu;
            window.MakeKeyAndVisible ();

            return true;
        }
    }

    public class DummyControllerLeft : DialogViewController
    {
        public DummyControllerLeft () 
            : base(UITableViewStyle.Plain,new RootElement(""))
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            // Add a tap listener, we will place it on the complete menu while editing a textfield
            // just don't add it yet, else your buttons won't work ;)
            var tap = new UITapGestureRecognizer ();
            tap.AddTarget (() =>{
                this.View.EndEditing (true);
            });

            // Get a link to the AppDelegate to access the menu property.
            var del = UIApplication.SharedApplication.Delegate as AppDelegate;

            // Create a textfield which will contain your search.
            EntryElement editContent = new EntryElement ("Search", "Tap here!", "");

            // When the textfield becomes active we will tell the menu to take up the full width
            // and add the tap listener to the complete view so we can easily dismiss the keyboard
            editContent.EntryStarted += (sender, e) => {
                del.Menu.MenuLeftToFullScreen();
                this.View.AddGestureRecognizer (tap);
            };

            // When the editing ended (but not pressed on the return key) we will slide the menu back 
            // don't forget to remove the tap listener
            editContent.EntryEnded += (sender, e) => {
                del.Menu.MenuLeftBackToNormal();
                this.View.RemoveGestureRecognizer(tap);
                // optionally we can clear the text here.
                // editContent.Value = "";
            };

            // When the return key is pressed we are gonna do a bunch of things.
            editContent.ShouldReturn += () => {
                // make sure they keyboard dismisses
                editContent.ResignFirstResponder(true);
                // slide the menu back into place first.
                del.Menu.MenuLeftBackToNormal();
                // remove the tap listener so it won't be stuck next time we open the menu
                this.View.RemoveGestureRecognizer(tap);
                // if the textfield contains text we will push a new screen to the top.
                if(editContent.Value.Length > 0)
                    NavigationController.PushViewController(new SearchViewController(editContent.Value), true);
                // optional: clear the textfield again so there won't be any text in it next time we open the menu.
                editContent.Value = "";
                return true;
            };

            Root.Add (new Section () {
                editContent,
                new StyledStringElement("Home", () => { NavigationController.PushViewController(new HomeViewController(), true); }),
                new StyledStringElement("About", () => { NavigationController.PushViewController(new AboutViewController(), true); }),
                new StyledStringElement("Stuff", () => { NavigationController.PushViewController(new StuffViewController(), true); }),
                new StyledStringElement("Full Screen", () => { NavigationController.PushViewController(new FullscreenViewController(), true); }),

            });
        }
    }

    public class DummyControllerRight : DialogViewController
    {
        public DummyControllerRight ()
            : base(UITableViewStyle.Plain,new RootElement(""))
        {
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            var del = UIApplication.SharedApplication.Delegate as AppDelegate;

            BooleanElement leftMenuEnabled = new BooleanElement ("Left menu enabled", del.Menu.LeftMenuEnabled);
            leftMenuEnabled.ValueChanged += (sender, e) => {
                del.Menu.LeftMenuEnabled = leftMenuEnabled.Value;
            };

            // when working with MonoTouch.Dialog you can only tell the interface to take up the whole width of the view.
            // by enabling the 'xMenuFullWidth', where x is the side its on, you can tell that side to make the view a bit smaller
            // this way your menu is nicely aligned. When you create your own menu in either the interface builder or programmatically
            // you might design it from right to the left on the right side menu, in that case you will most likely simply use the full width.
            BooleanElement rightFullWidth = new BooleanElement ("Full width design", del.Menu.RightMenuFullWidth);
            rightFullWidth.ValueChanged += (sender, e) => {
                del.Menu.RightMenuFullWidth = rightFullWidth.Value;
            };

            Root.Add (new Section () {
                new StyledStringElement("Home", () => { NavigationController.PushViewController(new HomeViewController(), true); }),
                leftMenuEnabled, rightFullWidth
            });
        }
    }
}