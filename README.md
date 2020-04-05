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

![Camera](/imgs/homepage.PNG)

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

### Design and Technologies

An image is take from the camera and using Bgra32 and converted into a BitMap format.
The image is teh trnaform into a pixels of or black and whit, where zero is the blck part and 1 is the white.


### Tools used

1.	Visual Studio 2019.
2.	Microsoft Azure Kinect Camera.
3.	Python
4.	Jupyter Notebook

### What we learned

Developin this project we learned many things which will go a long way to enrish or skiils as software developments.
While we were already familiar with some of the experience, this was the first time we applied them to this
particular project in this particular condition "global epedimic of Convid-19 2020".
Here are a list of some of the things we learned.

1. We learned how to work with each other and collaborate on GitHuh even when we disagree.
2. We learned ho to integrate Microsoft Azure Kinect Camera to a WPF project.
3. We learned how to implement Speech Recognition on WPF.
4. We learned how to capture an image from a camera and trained it from a trained model.
5. We learned to work as pair programming team.

### Links and Research

To implement this project we have done research on different topics and scenarios. Here are some of the links we looked at.
Azure Camera Hand tracking [Hand tracking](https://pterneas.com/2014/03/21/kinect-for-windows-version-2-hand-tracking/)<br/>
WPF SpeechRecognition Engine [SpeechRecognition Engine](https://docs.microsoft.com/en-us/dotnet/api/system.speech.recognition.speechrecognitionengine.recognize?view=netframework-4.8)
