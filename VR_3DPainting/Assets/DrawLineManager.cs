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
    private int numClicks = 0;
    public GameObject trackedObj;
    public Material lMat;
    void Start()
    {

        trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);

        //originalPos = transform.position;
        //pmInstance = WaveVR_PermissionManager.instance;
        //if (pmInstance.isInitialized())
        //{
        //    pmInstance.requestUsbPermission(requestUsbDoneCallback);
        //}
    }

    void Update()
    {
            //trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);

        if (WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            GameObject go = new GameObject();
            go.AddComponent<MeshFilter>();
            go.AddComponent<MeshRenderer>();
            currLine = go.AddComponent<GraphicsLineRenderer>();

            currLine.lmat = new Material(lMat);
            //currLine.lmat = lMat;
            currLine.SetWidth(.06f);

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
}
