using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using WVR_Log;
using wvr;
public class DrawLineManager : MonoBehaviour

{
    WaveVR_Controller.EDeviceType curFocusControllerType = WaveVR_Controller.EDeviceType.Head;
    WaveVR_Controller.EDeviceType DomFocusControllerType = WaveVR_Controller.EDeviceType.Dominant;
    WaveVR_Controller.EDeviceType NonFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;
    private WaveVR_PermissionManager pmInstance = null;

    private GraphicsLineRenderer currLine;
    private List<GraphicsLineRenderer> pastLines = new List<GraphicsLineRenderer>();
    private int numClicks;
    public GameObject trackedObj;
    public Material lMat;
    float width = .1f;

    private float prevYAngle; //keeps track of controller angle while rotating sketch
    private float prevDist; //keeps track of distance btw controllers while scaling sketch

    void Start()
    {
        trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        OnEnable();
    }

    void Update()
    {
        //generate stroke mesh when dominant hand trigger is down
        if (WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            GameObject go = new GameObject();
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            currLine = go.AddComponent<GraphicsLineRenderer>();

            currLine.lmat = new Material(lMat);
            //currLine.lmat = lMat;
            currLine.SetWidth(width);

            numClicks = 0;
        }
        else if (WaveVR_Controller.Input(DomFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            currLine.AddPoint(WaveVR_Controller.Input(DomFocusControllerType).transform.pos);
            numClicks++;
        }
        else if (WaveVR_Controller.Input(DomFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            pastLines.Add(currLine);
            numClicks = 0;
        }

        //scale sketch when both grips are down
        if (WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Grip)
            && WaveVR_Controller.Input(NonFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            WaveVR_Utils.RigidTransform _rpose = WaveVR_Controller.Input(DomFocusControllerType).transform;
            Vector3 posR = _rpose.pos;
            WaveVR_Utils.RigidTransform _lpose = WaveVR_Controller.Input(NonFocusControllerType).transform;
            Vector3 posL = _lpose.pos;

            prevDist = Vector3.Distance(posR, posL);
        }
        else if (WaveVR_Controller.Input(DomFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Grip)
            && WaveVR_Controller.Input(NonFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            WaveVR_Utils.RigidTransform _rpose = WaveVR_Controller.Input(DomFocusControllerType).transform;
            Vector3 posR = _rpose.pos;
            WaveVR_Utils.RigidTransform _lpose = WaveVR_Controller.Input(NonFocusControllerType).transform;
            Vector3 posL = _lpose.pos;

            float dist = Vector3.Distance(posR, posL);
            float dDist = dist - prevDist;

            float scale = 1.05f;
            if (dDist < 0)
            {
                scale = 0.95f;
            }

            //get sketch center to use later to keep sketch around same position
            //(otherwise it floats up when it scales up)
            Vector3 sumVector = Vector3.zero;
            float count = 0.0f;
            foreach (GraphicsLineRenderer glr in pastLines)
            {
                foreach(Vector3 vec in glr.ml.vertices)
                {
                    sumVector += vec;
                    count++;
                }
            }
            Vector3 avgVector = sumVector / count;

            //scale each stroke (GraphicsLineRenderer) by scaling its mesh vertices
            foreach (GraphicsLineRenderer glr in pastLines)
            {
                Vector3[] vertices = glr.ml.vertices;
                for(int v = 0; v < vertices.Length; v++) {//vertices being the array of vertices of your mesh
                    vertices[v] = (vertices[v] - avgVector) * scale + avgVector;
                }
                glr.ml.vertices = vertices;
            }

            prevDist = dist;
        }
        //rotate sketch when dominant hand grip is down
        else if (WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            //store prev angle
            WaveVR_Utils.RigidTransform _rpose = WaveVR_Controller.Input(WaveVR_Controller.EDeviceType.Dominant).transform;
            float prevYAngle = _rpose.rot.eulerAngles[1];
        }
        else if (WaveVR_Controller.Input(DomFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            WaveVR_Utils.RigidTransform _rpose = WaveVR_Controller.Input(WaveVR_Controller.EDeviceType.Dominant).transform;
            float yAngle = _rpose.rot.eulerAngles[1];
            float dyAngle = yAngle - prevYAngle;
            
            //rotate sketch based on change from previous angle
            Vector3 center = new Vector3(0, 0, 0);//any V3 you want as the pivot point.
            Quaternion newRotation = new Quaternion();
            newRotation.eulerAngles = new Vector3(0,dyAngle,0);//the degrees the vertices are to be rotated, for example (0,90,0)

            //rotate each stroke (GraphicsLineRenderer) by rotating its mesh vertices
            foreach (GraphicsLineRenderer glr in pastLines)
            {
                Vector3[] vertices = glr.ml.vertices;
                for(int v = 0; v < vertices.Length; v++) {//vertices being the array of vertices of your mesh
                    vertices[v] = newRotation * (vertices[v] - center) + center;
                }
                glr.ml.vertices = vertices;
            }

            prevYAngle = yAngle;
        }

        //give current stroke color from color picker
        if (currLine != null)
        {
            currLine.lmat.color = ColorManager.Instance.GetCurrentColor();
        }
    }

    void OnEvent(params object[] args)
    {
        var _event = (WVR_EventType)args[0];
        Log.d("Event_Test", "OnEvent() _event = " + _event);

        switch (_event)
        {
            case WVR_EventType.WVR_EventType_RightToLeftSwipe:
                //transform.Rotate(0, 180 * (10 * Time.deltaTime), 0);
                int len = pastLines.Count;
                if (len > 0)
                {
                    GraphicsLineRenderer toRemove = pastLines[len - 1];
                    toRemove.ClearLine();
                    toRemove = null;
                    pastLines.RemoveAt(len-1);
                }
                break;
            case WVR_EventType.WVR_EventType_DownToUpSwipe:
                width += 0.02f;
                //transform.Rotate(0, 0, 180 * (10 * Time.deltaTime));
                break;
            case WVR_EventType.WVR_EventType_UpToDownSwipe:
                width -= 0.02f; ///TODO: shouldn't this have a check to make sure it doesn't become 0?
                //transform.Rotate(0, 0, -180 * (10 * Time.deltaTime));
                break;
        }
    }

    void OnEnable()
    {
        WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.SWIPE_EVENT, OnEvent);
    }

    void OnDisable()
    {
        WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.SWIPE_EVENT, OnEvent);
    }
}
