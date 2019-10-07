using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using WVR_Log;
using wvr;
public class DrawLineManager : MonoBehaviour,
IPointerUpHandler,
IPointerEnterHandler,
IPointerExitHandler,
IPointerDownHandler,
IBeginDragHandler,
IDragHandler,
IEndDragHandler,
IDropHandler,
IPointerHoverHandler
{
    private const string LOG_TAG = "WaveVR_HelloVR";
    Vector3 originalPos;
    WaveVR_Controller.EDeviceType curFocusControllerType = WaveVR_Controller.EDeviceType.Head;
    WaveVR_Controller.EDeviceType DomFocusControllerType = WaveVR_Controller.EDeviceType.Dominant;
    //WaveVR_Controller.Device Dom = WaveVR_Controller.Input(WaveVR_Controller.EDeviceType.Dominant);

    WaveVR_Controller.EDeviceType NonFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;


    private WaveVR_PermissionManager pmInstance = null;
    public bool isControllerFocus_R;
    public bool isControllerFocus_L;
    private GameObject m_RightController;
    private GameObject m_LeftController;

    private LineRenderer currLine;
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

            if (WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Touchpad) ||
                WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))

            {
                GameObject go = new GameObject();
                go.AddComponent<MeshFilter>();
                go.AddComponent<MeshRenderer>();
                currLine = go.AddComponent<LineRenderer>();
                currLine.SetWidth(.1f, .1f);
                currLine.material = lMat;

                //currLine.SetWidth(.1f);
                numClicks = 0;
                trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
            }
            else if (WaveVR_Controller.Input(DomFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Touchpad) ||
                WaveVR_Controller.Input(DomFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger))
            {
                currLine.SetVertexCount(numClicks + 1);
                currLine.SetPosition(numClicks, WaveVR_Controller.Input(DomFocusControllerType).transform.pos);
            
                numClicks++;

                trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);

            }

            else if (WaveVR_Controller.Input(DomFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Touchpad) ||
            WaveVR_Controller.Input(DomFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger))
            {
                    trackedObj.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            }

        
    }
    void moveSphere()
    {
        if (originalPos != transform.position)
        {
            transform.position = originalPos;
            return;
        }
        if (curFocusControllerType == WaveVR_Controller.EDeviceType.Dominant)
        {
            transform.position = new Vector3(1, originalPos.y, originalPos.z);
        }
        else
        {
            transform.position = new Vector3(-1, originalPos.y, originalPos.z);

        }
        //GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        //Debug.Log("OnPointerUp");
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        WaveVR_Controller.EDeviceType type = eventData.enterEventCamera.gameObject.GetComponent<WaveVR_PoseTrackerManager>().Type;
        GameObject target = eventData.enterEventCamera.gameObject;
        if (target.GetComponent<WaveVR_PoseTrackerManager>())
        {
            if (type == WaveVR_Controller.EDeviceType.Dominant)
            {
                m_RightController = target;
                isControllerFocus_R = true;
            }
            else if (type == WaveVR_Controller.EDeviceType.NonDominant)
            {
                m_LeftController = target;
                isControllerFocus_L = true;
            }

            // Right-Hand mode
            if (!WaveVR_Controller.IsLeftHanded)
            {
                if (isControllerFocus_R)
                {
                    GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                }
                else if (isControllerFocus_L)
                {
                    GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                }
            }

            // Left-Hand mode
            else if (WaveVR_Controller.IsLeftHanded)
            {
                if (isControllerFocus_R)
                {
                    GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                }
                else if (isControllerFocus_L)
                {
                    GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                }
            }
        }
        curFocusControllerType = type;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RaycastHit hit;
        if (m_RightController && isControllerFocus_R)
        { //R_Controller Leave
            Vector3 fwd_R = m_RightController.transform.TransformDirection(Vector3.forward);
            if (!Physics.Raycast(m_RightController.transform.position, fwd_R, out hit))
            {

                isControllerFocus_R = false;
                if (isControllerFocus_L)
                {
                    curFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;
                    GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
                    return;
                }
            }
        }

        if (m_LeftController && isControllerFocus_L)
        { //L_Controller Leave
            Vector3 fwd_L = m_LeftController.transform.TransformDirection(Vector3.forward);
            if (!Physics.Raycast(m_LeftController.transform.position, fwd_L, out hit))
            {
                isControllerFocus_L = false;
                if (isControllerFocus_R)
                {
                    curFocusControllerType = WaveVR_Controller.EDeviceType.Dominant;
                    GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                    return;
                }
            }
        }
        curFocusControllerType = WaveVR_Controller.EDeviceType.Head;

        GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        //Debug.Log("OnPointerDown");

    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        //Debug.Log("OnBeginDrag");
    }

    public void OnDrag(PointerEventData eventData)
    {
        //Debug.Log("OnDrag");
    }


    public void OnEndDrag(PointerEventData eventData)
    {
        //Debug.Log("OnEndDrag");
    }

    public void OnDrop(PointerEventData eventData)
    {
        //Debug.Log("OnDrop");
    }

    public void OnPointerHover(PointerEventData eventData)
    {
        // Debug.Log("OnPointerHover: "+eventData.enterEventCamera.gameObject);
    }

    public static void requestUsbDoneCallback(bool result)
    {
#if !UINTY_EDITOR
        Log.d(LOG_TAG, "requestUsbDoneCallback, result= " + result);
#endif
    }
}
