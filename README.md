# VR-3DPaint
## Xiyu Zhai, Aakash Parikh, Melody Mao
This project aims to creat a VR-based tilt brush app that allows people to draw strokes in 3D.
on a HTC Vive Focus Plus

HTC Vive Focus Plus runs an Android system, if you want it to run PC VR, use VRidge

---
## Vive to Wave VR
Before we start, there is actually a [tutorial](https://hub.vive.com/storage/app/doc/en-us/ViveToWaveVR.html#vivetowavevr) talks about how to port an app from Vive(Using Steam VR) to Wave VR.

And there is a excellent [tutorial](https://www.youtube.com/watch?v=eMJATZI0A7c) teaches how to build tilt brush from scratch.

While the porting process is complicated and leads to many bugs, I decide to continue using Wave VR directly.

## Setup Unity
**Direct Preview won't work on Mac (as those scripts are for Windows)**

**Need to manually download NDK in Unity-Preference-External Tools**
1. Follow [this](https://hub.vive.com/storage/docs/en-us/UnityPluginGettingStart.html) to set up the android environment for Focus Plus(*This doc works for Windows, little different in mac os.*)
2. Download [Wave unity sdk](https://hub.vive.com/en-US/download)
3. Connect Vive Focus Plus to Computer (Install Android File Transfer on Mac)

   ~~4. Enable [DirectPreview](https://hub.vive.com/storage/app/doc/en-US/UnityDocWaveVRDirectPreview.html).~~
### **VR_Learning to test setup works well**
This is a sample Scene build from [Hello VR](https://hub.vive.com/storage/app/doc/en-us/UnitySampleStarting.html)

Simply Build and Run after connecting Focus to Mac
***
## Drawing Lines
Be able to draw continuous strokes in 3D using a handheld controller

**Wired "Gradle build failed" happens sometimes, just try saving all and restart Unity.**

### **Capture Button Events**
See [this](https://hub.vive.com/storage/app/doc/en-us/WaveVR_Controller.html) for more details
```cs
WaveVR_Controller.EDeviceType curFocusControllerType = WaveVR_Controller.EDeviceType.Head;
WaveVR_Controller.EDeviceType DomFocusControllerType = WaveVR_Controller.EDeviceType.Dominant;
WaveVR_Controller.EDeviceType NonFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;
if (WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger)){}
```

### **Line Renderer**
1. Press down: Create new object and add LineRender
```cs
LineRenderer currLine;
GameObject go = new GameObject();
currLine = go.AddComponent<LineRenderer>();
currLine.SetWidth(.05f, .01f);
currLine.material = lMat;
```
2. Pressing: Add points of the LineRender
```cs
currLine.SetVertexCount(numClicks + 1);
currLine.SetPosition(numClicks, WaveVR_Controller.Input(DomFocusControllerType).transform.pos);
```
3, Press up: Reset
```cs
numClicks = 0;
currLine = null;
```
### **Using Graphics Line Renderer**
Follow [this blog](http://www.everyday3d.com/blog/index.php/2010/03/15/3-ways-to-draw-3d-lines-in-unity3d/) to use a "better" line renderer that generate lines which won't "rotate" when camera moving

LineRenderer also works well~
## Color Picker
Change stroke's color

Using Package Simple Color Picker
1. Rewrite Draggable.cs in Color Picker-Script for the VR controller instead of mouse
2. Implement ColorManager.cs to colorpicker
3. Rewrite DrawLineManager.cs to receive color
```cs
currLine.lmat = new Material(lMat); //create new material every time
currLine.lmat.color = ColorManager.Instance.GetCurrentColor();
```
## Stroke Size
Follow [this doc](https://hub.vive.com/storage/app/doc/en-us/WaveVR_SystemEvent.html) to use Swipe Event on both controller to change stroke size.
```cs
void OnEvent(params object[] args){}
...
case WVR_EventType.WVR_EventType_LeftToRightSwipe:
        width += 0.02f;
...
void OnEnable(){}
```
---
## Erasing
Ability to select and erase both individual brush strokes or the entire scene.
### **Reset the entire scene**
Using same logic in Go_event.cs(from helloVR). Attach Reset Scene to test sphere.

Use Nondominant controller to point the test sphere and Press Touchpad will reset the whole Scene.
```cs
SceneManager.LoadScene(SceneName);
```
### **Erase individual  brush strokes**
You can undo the last brush stroke by swiping from Right to Left on the controller
## Navigation:
* Scale, rotate, and move your sketch (or your position within the sketch).
* Teleport to different locations within the sketch.
