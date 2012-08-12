
using System;
using System.Drawing;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using MonoTouch.CoreGraphics;

namespace Slideout.Sample
{
    public partial class FullscreenViewController : UIViewController
    {
        public FullscreenViewController () : base ("FullscreenViewController", null)
        {
        }
		
        public override void DidReceiveMemoryWarning ()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning ();
			
            // Release any cached data, images, etc that aren't in use.
        }
		
        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();

            this.BackButton.TouchUpInside += (sender, e) => {
                var del = UIApplication.SharedApplication.Delegate as AppDelegate;
                del.Menu.Show();
            };
        }

        public override void ViewDidAppear (bool animated)
        {
            base.ViewDidAppear (animated);

            //Only start the animation once the slide-out is done animating or else you get weird shit
            NSTimer.CreateScheduledTimer(0.2, () => {

                this.WantsFullScreenLayout = true;
                NavigationController.SetNavigationBarHidden(true, animated);
                UIApplication.SharedApplication.SetStatusBarHidden(true, animated);

                //Animate the view frame to occupy the negative space caused by the status bar
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveLinear, () => {
                    var del = UIApplication.SharedApplication.Delegate as AppDelegate;
                    var frame = del.Menu.View.Frame;

                    //I have absolutely no clue why this is needed. Apparently when the orientation changes so does the
                    //coordinate system. When in landscape, we just adjust the width of the frame?!?! That doesnt make any sense
                    //to me but what ever. It seems to behave as expected now...
                    if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft ||
                        UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
                    {
                        frame.Width += 20f;
                    }
                    else
                    {
                        frame.Y -= 20f;
                        frame.Height += 20f;
                    }

                    del.Menu.View.Frame = frame;
                }, () => { });


            });

        }

        public override void ViewDidDisappear (bool animated)
        {
            base.ViewDidDisappear (animated);

            NavigationController.SetNavigationBarHidden(false, animated);
             
            //Only start the animation once the slide-out is done animating or else you get weird shit.
            NSTimer.CreateScheduledTimer(0.2, () => {
                UIApplication.SharedApplication.SetStatusBarHidden(false, animated);

                //Undo what we did in the ViewDidAppear
                UIView.Animate(0.3, 0, UIViewAnimationOptions.CurveLinear, () => {
                    var del = UIApplication.SharedApplication.Delegate as AppDelegate;
                    var frame = del.Menu.View.Frame;

                    //I have absolutely no clue why this is needed. Apparently when the orientation changes so does the
                    //coordinate system. When in landscape, we just adjust the width of the frame?!?! That doesnt make any sense
                    //to me but what ever. It seems to behave as expected now...
                    if (UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeLeft||
                        UIApplication.SharedApplication.StatusBarOrientation == UIInterfaceOrientation.LandscapeRight)
                    {
                        frame.Width -= 20f;
                    }
                    else
                    {
                        frame.Y += 20f;
                        frame.Height -= 20f;
                    }

                    del.Menu.View.Frame = frame;
                }, () => { });
            });
        }
		
        public override void ViewDidUnload ()
        {
            base.ViewDidUnload ();
			
            // Clear any references to subviews of the main view in order to
            // allow the Garbage Collector to collect them sooner.
            //
            // e.g. myOutlet.Dispose (); myOutlet = null;
			
            ReleaseDesignerOutlets ();
        }

        public override bool ShouldAutorotateToInterfaceOrientation (UIInterfaceOrientation toInterfaceOrientation)
        {
            // Return true for supported orientations
            return (toInterfaceOrientation != UIInterfaceOrientation.PortraitUpsideDown);
        }
    }
}

