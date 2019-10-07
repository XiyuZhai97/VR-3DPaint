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
    private int numClicks;
    public GameObject trackedObj;
    public Material lMat;
    float width = .1f;


    void Start()
    {
        trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        OnEnable();
    }

    void Update()
    {
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
            //WaveVR_Controller.Input(DomFocusControllerType).TriggerHapticPulse();
        }
        else if (WaveVR_Controller.Input(DomFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
        //currLine.SetVertexCount(numClicks + 1);
        //currLine.SetPosition(numClicks, WaveVR_Controller.Input(DomFocusControllerType).transform.pos);
            currLine.AddPoint(WaveVR_Controller.Input(DomFocusControllerType).transform.pos);
            numClicks++;
        }
        else if (WaveVR_Controller.Input(DomFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            numClicks = 0;
            currLine = null;
        }
        if (currLine != null)
        {
            currLine.lmat.color = ColorManager.Instance.GetCurrentColor();
        }
        //if (WaveVR_Controller.Input(NonFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
        //{
        //    trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        //}
        //else if (WaveVR_Controller.Input(NonFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger))
        //{
        //    trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        //}

        //else if (WaveVR_Controller.Input(NonFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger))
        //{
        //    trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        //}
    }
    void OnEvent(params object[] args)
    {
        var _event = (WVR_EventType)args[0];
        Log.d("Event_Test", "OnEvent() _event = " + _event);

        switch (_event)
        {
            case WVR_EventType.WVR_EventType_LeftToRightSwipe:
                //transform.Rotate(0, 180 * (10 * Time.deltaTime), 0);
                width += 0.02f;
                break;
            case WVR_EventType.WVR_EventType_RightToLeftSwipe:
                width -= 0.02f;
                //transform.Rotate(0, -180 * (10 * Time.deltaTime), 0);
                break;
            case WVR_EventType.WVR_EventType_DownToUpSwipe:
                width += 0.02f;
                //transform.Rotate(0, 0, 180 * (10 * Time.deltaTime));
                break;
            case WVR_EventType.WVR_EventType_UpToDownSwipe:
                width -= 0.02f;
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
