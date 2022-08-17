*****WEB APPLICATION NAME:MoodRecognizer*****


***Creating MoodRecognizer web application in BalitcLSc by using C# ,.Net core3.1,CAL,Docker,REST API,MongoDB,Dockerhub***


This is about my Research project details.

Deployed in this below Balticlsc environment.
https://balticlsc.iem.pw.edu.pl/#/store

**Introduction:**
The project’s goal was to create a simple application in the BalticLSC environment.The project was aimed to research the difficulty of
navigating the environment and creating new modules. While working on the application, we gathered information
about the positive sides of using Baltic and its problems


**BALTICLSC:**
The Baltic Large Scale Computing (BalticLSC) project aims to solve the problem of lack of access to large-scale computing by small and medium-sized companies lacking 
expertise or the resources to create a similar environment and use the surplus computing power of large companies. For this purpose, an internet platform is being created to allow easy access to
computing power in a simple and easy-to-use way.
To create the BalticLSC system, an international consortium was created. This consortium gathers universities, technology centers, and companies from Baltic region countries, which
offer different competencies necessary to complete an ongoing project

Architectural module plan diagram as below by using CAL language
![Emotion detection application in CAL languages](https://user-images.githubusercontent.com/63377540/185163705-079762a5-172f-44c7-8627-fd97ce7ba616.png)


Figure 1 shows the construction of the sample application.Input and Output job is successively to enter data into the application and return the result. The large rectangles represent
the modules that do a specific job and are adjacent to them are the token’s inputs and outputs. Each module has a certain amount of tokens that it needs to start working. The arrows
show the course of token transfer between modules.

**Face_Detection**
 **Module1**
Face detection
The Baltic environment has already created modules for downloading from the device and returning the processed product, in this case, a photo. The first part of the application
was the face detection module. Using the Dlib library, it is possible to detect faces on images. This is done by creating an Oriented Gradient histogram (HOG). To do this, the program
searches the pixels for their needs, creating arrows between them from each pixel to its darkest neighbor. The use of a gradient relation instead of pixel intensity is justified by the
fact that even when changing the brightness, gradients should not change. For the program to run efficiently enough, 16x16 blocks are used instead of individual pixels.The Dlib library
includes a pre-trained HOG face search model


**Determining characteristic points**
**Module2**
The library used in the project includes a trained face characteristic point detector. By recognizing 68 characteristic
points across the face, it is possible to find out emotions. This method is also used for face recognition, i.e., comparing the face selected for analysis with all faces in the application
database. In this case, the points obtained in this step are used to obtain 128 points representing the face, which is then fed to the CNN network.

**Mood recognition**
**Module 3**
Human emotion depends upon the eye, mouth, and eyebrow,which indicates the mood expression of a human face. Facial 
images and control recognize human emotion by fuzzy logic.In our module application, we took the face of images from the first module ‘’Face recognition “. and the point of eyes,mouths, and eyebrows from the second module, “Determine of
characteristic point“, then we calculated the Certainty Factor(CF) and compared it with fuzzy rules. Based on the CF value,
we determined emotion. In the processed images, we have 68point for each face. From those points, we calculated CF and detect the emotion.

**Data summarizing module**
**Module 4**
The purpose of the last module is to return processed photos and JSON files containing results of emotion detection. It receives from previous module information where to find an
image and detected emotions. The photo is sent directly to the output. JSON is created by adding to a new file name of the photo, several detected faces, and emotions numbered by
detected faces.

**Final application**

After we finished our application, it was tested on the BalticLSC system. After many attempts, we managed to fix all the problems, and the application returned processed images with marked faces and a JSON file with detected emotions.
Figure 1 represents created by authors Emotion Detection application in CAL language. It describes the data flow between all modules. All of the modules have one input, and only the last has two outputs to return both processed photo
and JSON file with results.


