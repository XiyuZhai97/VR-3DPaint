using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WVR_Log;
using wvr;
public class Navigator : MonoBehaviour,
// IPointerUpHandler,
IPointerEnterHandler,
IPointerExitHandler,
// IPointerDownHandler,
// IBeginDragHandler,
// IDragHandler,
// IEndDragHandler,
// IDropHandler,
IPointerHoverHandler
{
    WaveVR_Controller.EDeviceType DomFocusControllerType = WaveVR_Controller.EDeviceType.Dominant;
    WaveVR_Controller.EDeviceType NonFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;
    public bool isControllerFocus_R;
    public bool isControllerFocus_L;
    public GameObject m_RightController;
    public GameObject m_LeftController;
    // private GameObject m_RightController;
    // private GameObject m_LeftController;

    public GameObject teleportSprite; //indicator of where you'll teleport to
    private bool teleportOn = false;
    private bool hasPosition = false;

    void Update()
    {
        if (teleportOn)
        {
            //grip release, try to teleport
            if (WaveVR_Controller.Input(DomFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Grip))
            {
                teleportOn = false;
                hasPosition = false;
            }
            else //otherwise update sprite
            {
                hasPosition = UpdateSprite();
                teleportSprite.SetActive(hasPosition); //only visible if we can teleport there
            }
        }
        //to turn on teleport, check for just right grip down
        else if (WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Grip))
            //&& !WaveVR_Controller.Input(NonFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Grip))
        {
            teleportOn = true;
        }
        //teleport on, ignore left grip
        //send out pointer
        //put teleport sprite at end of it
        //when teleport on, if grip released, teleport
    }

    /* moves the teleport sprite to the teleport position, if possible
     * returns whether a teleport position was found
     */
    private bool UpdateSprite()
    {
        //WaveVR_Utils.RigidTransform _rpose = WaveVR_Controller.Input(DomFocusControllerType).transform;
        //Vector3 pos = WaveVR_Controller.Input(DomFocusControllerType).transform.pos;
        Vector3 fwd_L = m_LeftController.transform.TransformDirection(Vector3.forward);
        //Ray ray = new Ray(pos, forward);
        RaycastHit hit;

        //if ray from controller hits a surface, move sprite there
        if (!Physics.Raycast(m_LeftController.transform.position, fwd_L, out hit))
        {
            teleportSprite.transform.position = hit.point;
            return true;
        }
        
        //no hit
        return false;
    }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     //Debug.Log("OnPointerUp");
    // }

    /*
    pointer enters floor
    see if it's the left hand
    
     */

    public void OnPointerEnter(PointerEventData eventData)
    {
        WaveVR_Controller.EDeviceType type = eventData.enterEventCamera.gameObject.GetComponent<WaveVR_PoseTrackerManager>().Type;
        GameObject target = eventData.enterEventCamera.gameObject;
        if (target.GetComponent<WaveVR_PoseTrackerManager>())
        {
            if (type == WaveVR_Controller.EDeviceType.NonDominant)
            {
                m_LeftController = target;
                isControllerFocus_L = true;
            }
        }
        NonFocusControllerType = type;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RaycastHit hit;
        //if (m_RightController && isControllerFocus_R)
        //{ //R_Controller Leave
        //    Vector3 fwd_R = m_RightController.transform.TransformDirection(Vector3.forward);
        //    if (!Physics.Raycast(m_RightController.transform.position, fwd_R, out hit))
        //    {

        //        isControllerFocus_R = false;
        //        if (isControllerFocus_L)
        //        {
        //            NonFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;
        //            GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
        //            return;
        //        }
        //    }
        //}

        if (m_LeftController && isControllerFocus_L)
        { //L_Controller Leave
            Vector3 fwd_L = m_LeftController.transform.TransformDirection(Vector3.forward);
            if (!Physics.Raycast(m_LeftController.transform.position, fwd_L, out hit))
            {
                isControllerFocus_L = false;
                if (isControllerFocus_R)
                {
                    NonFocusControllerType = WaveVR_Controller.EDeviceType.Dominant;
                    GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
                    return;
                }
            }
        }
        NonFocusControllerType = WaveVR_Controller.EDeviceType.Head;

        GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
    }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     //Debug.Log("OnPointerDown");

    // }

    // public void OnBeginDrag(PointerEventData eventData)
    // {
    //     //Debug.Log("OnBeginDrag");
    // }

    // public void OnDrag(PointerEventData eventData)
    // {
    //     //Debug.Log("OnDrag");
    // }


    // public void OnEndDrag(PointerEventData eventData)
    // {
    //     //Debug.Log("OnEndDrag");
    // }

    // public void OnDrop(PointerEventData eventData)
    // {
    //     //Debug.Log("OnDrop");
    // }

    public void OnPointerHover(PointerEventData eventData)
    {
        // Debug.Log("OnPointerHover: "+eventData.enterEventCamera.gameObject);
    }

//     public static void requestUsbDoneCallback(bool result)
//     {
// #if !UNITY_EDITOR
//         //Log.d(LOG_TAG, "requestUsbDoneCallback, result= " + result);
// #endif
//     }
}
