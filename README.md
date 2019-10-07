# VR-3DPaint
## Xiyu Zhai, Aakash Parikh, Melody Mao
This project aims to creat a VR-based tilt brush app that allows people to draw strokes in 3D.
on a HTC Vive Focus Plus
## Part one Hello Focus -- Xiyu Zhai
HTC Vive Focus Plus runs an Android system, thus if you want it to run PC VR, use VRidge

### Vive to Wave VR 
Before we start, there is actually a [tutorial](https://hub.vive.com/storage/app/doc/en-us/ViveToWaveVR.html#vivetowavevr) talks about how to port an app from Vive(Using Steam VR) to Wave VR.
And there is a excellent [tutorial](https://www.youtube.com/watch?v=eMJATZI0A7c) teaches how to build tilt brush from scratch.

While the porting process is complicated and leads to many bugs, I decide to continue using Wave VR directly.

### Setup Unity
1. Follow [this](https://hub.vive.com/storage/docs/en-us/UnityPluginGettingStart.html) to set up the android environment for Focus Plus
*This doc works for Windows, little different in mac os.*
**Direct Preview won't work on Mac directly (as those scripts are for Windows)**
**Remember manually download NDK in Unity-Preference-External Tools**
2. Download [Wave unity sdk](https://hub.vive.com/en-US/download)

3. Connect Vive Focus Plus to Computer (Install Android File Transfer on Mac)

~4. Enable [DirectPreview](https://hub.vive.com/storage/app/doc/en-US/UnityDocWaveVRDirectPreview.html)failed on Mac~

### VR_Learning to test setup works well
This is a sample Scene build from [Hello VR](https://hub.vive.com/storage/app/doc/en-us/UnitySampleStarting.html)

Simply Build and Run after connecting Focus to Mac

### Draw Line

#### DrawLineManager
Capture the button
        public bool isControllerFocus_R;

#### Capture Button Events
    WaveVR_Controller.EDeviceType curFocusControllerType = WaveVR_Controller.EDeviceType.Head;
    WaveVR_Controller.EDeviceType DomFocusControllerType = WaveVR_Controller.EDeviceType.Dominant;
~//WaveVR_Controller.Device Dom = WaveVR_Controller.Input(WaveVR_Controller.EDeviceType.Dominant);~

    WaveVR_Controller.EDeviceType NonFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;

#### Line Render