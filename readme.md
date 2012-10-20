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

Checkout the video at youtube: [http://www.youtube.com/watch?v=YnDURn_Y60E](http://www.youtube.com/watch?v=YnDURn_Y60E)


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

Licensing
--------

Copyright 2012 Dillon Buchanan

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

   http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
