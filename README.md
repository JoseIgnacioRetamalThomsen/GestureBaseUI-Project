# Gesture Based UI Development 2020
By Mark Ndipenoch & Jose Retamal
 
 ![Camera](/imgs/deviceCord.png)

### Overview

This is a gesture base UI application aim at using hand gestures to control the cursor on the computer screen instead of using a mouse.
The application was done on Windows Presentation Foundation (WPF), which is a UI framework that creates desktop client applications. 
We used the Microsoft Azure Kinect Camera, Visual Studio 2019, because it have specific pakages and libraries to ocnnect to the Microsoft Azure Kinect Camera and Jupyter Notebook to train the hand gesture models.
The application have a train mdoel of hand gesture done in Python from Jupyter Notebook.
At the start of the application the user is presented with a screen view. The user can either choose to use the default hand gesture already in the application or to create their own handgestures that will be trained from the already trained model.
If a user decides to create their own hand gestures, there will be presented by the number of hand gestures available and they can select and record each gesture.
At the start of the application the camera is turned on the user can then navigate to the view page where they can use their hand gesture to control the cursor on their computer.
Also, at the start of the applciation the user can click on the "Voice command" button to say the name of the page where they will like to navigate to. This button have a timeout of five seconds.
![Camera](/imgs/homepage.png)

There are current 10 hand gestures available which are:

1.	Straight point - To move across the screen with you finger.
2.	Click index finger and thumb - To close the current window.
3.	Thumb up - For Okay, agree or aceept
4.	Thumb down - To deny or cancel
5.	Open palm  - To indicate the degit 5
6.	Closed fist - To indicate the degit zero
7.	Slap down - To scroll down
8.	Slap up - To scroll up
9.	Index finger up - To indicate the digit one.
10.	Peace sign - To indicate the the digit 2.

