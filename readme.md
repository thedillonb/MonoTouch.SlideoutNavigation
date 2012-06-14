MonoTouch.SlideoutNavigation
============================

Description
-----------

Provides a facebook style slide out navigation control.
The UINavigationController is built into the library and
the proper buttons and slide mechanics are already setup for you.

You can press "menu" button or make a right pan motion to reveal the menu.
To close, you can click the former view or make a left pan motion.

In Action
---------

![Home Screen](http://www.dillonbuchanan.com/wp-content/uploads/2012/06/img1.png "Home Screen") ![Open](http://www.dillonbuchanan.com/wp-content/uploads/2012/06/img2.png "Open")


Usage
-----

    //Instantiate the controller
    var slideoutNav = new SlideoutNavigationController();
    
    //Assign the menu ViewController to our nib controller
    //See the sample project for a more detailed menu controller
    //such as a tableview using MonoTouch.Dialog
    slideoutNav.MenuView = new UIViewController("menunib", null);

    //Assign the Top view controller which is seen first before
    //you slide over to see the menu.
    slideoutNav.TopView = new UIViewController("firstviewnib", null);

    //Assign the controller to be displayed!
    window.RootViewController = slideoutNav;
