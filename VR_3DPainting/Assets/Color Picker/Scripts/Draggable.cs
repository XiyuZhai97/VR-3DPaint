using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WVR_Log;
using wvr;
public class Draggable : MonoBehaviour,
IPointerEnterHandler,
IPointerExitHandler
{
    public bool fixX;
    public bool fixY;
    public Transform thumb;
    bool dragging;

    public bool isControllerFocus_R;
    public bool isControllerFocus_L;
    private GameObject m_RightController;
    private GameObject m_LeftController;
    WaveVR_Controller.EDeviceType curFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;
    void FixedUpdate()
    {

        if (WaveVR_Controller.Input(curFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            dragging = false;
            Vector3 fwd_L = m_LeftController.transform.TransformDirection(Vector3.forward);

            Ray ray = new Ray(WaveVR_Controller.Input(curFocusControllerType).transform.pos, fwd_L);
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                dragging = true;
            }
        }
        if (WaveVR_Controller.Input(curFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger))
            dragging = false;
        if (dragging && WaveVR_Controller.Input(curFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger))
        {
            Vector3 fwd_L = m_LeftController.transform.TransformDirection(Vector3.forward);
            Ray ray = new Ray(WaveVR_Controller.Input(curFocusControllerType).transform.pos, fwd_L);
            //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
            {
                var point = hit.point;
                point = GetComponent<Collider>().ClosestPointOnBounds(point);
                SetThumbPosition(point);
                SendMessage("OnDrag", Vector3.one - (thumb.position - GetComponent<Collider>().bounds.min) / GetComponent<Collider>().bounds.size.x);
            }
        }
    }

    void SetDragPoint(Vector3 point)
    {
        point = (Vector3.one - point) * GetComponent<Collider>().bounds.size.x + GetComponent<Collider>().bounds.min;
        SetThumbPosition(point);
    }

    void SetThumbPosition(Vector3 point)
    {
        thumb.position = new Vector3(fixX ? thumb.position.x : point.x, fixY ? thumb.position.y : point.y, thumb.position.z);
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        WaveVR_Controller.EDeviceType type = WaveVR_Controller.EDeviceType.NonDominant;
        //eventData.enterEventCamera.gameObject.GetComponent<WaveVR_PoseTrackerManager>().Type;
        GameObject target = eventData.enterEventCamera.gameObject;
        if (target.GetComponent<WaveVR_PoseTrackerManager>())
        {
            //if (type == WaveVR_Controller.EDeviceType.Dominant)
            //{
            //    m_RightController = target;
            //    isControllerFocus_R = true;
            //}
            //else
            if (type == WaveVR_Controller.EDeviceType.NonDominant)
            {
                m_LeftController = eventData.enterEventCamera.gameObject;
                isControllerFocus_L = true;
            }

            if (WaveVR_Controller.Input(curFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
            {
                dragging = false;

                Vector3 fwd_L = m_LeftController.transform.TransformDirection(Vector3.forward);
                Ray ray = new Ray(WaveVR_Controller.Input(curFocusControllerType).transform.pos, fwd_L);
                //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
                {
                    dragging = true;
                }
            }
            if (WaveVR_Controller.Input(curFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Trigger))
                dragging = false;
            if (dragging && WaveVR_Controller.Input(curFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Trigger))
            {
                Vector3 fwd_L = m_LeftController.transform.TransformDirection(Vector3.forward);
                Ray ray = new Ray(WaveVR_Controller.Input(curFocusControllerType).transform.pos, fwd_L);
                //var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (GetComponent<Collider>().Raycast(ray, out hit, Mathf.Infinity))
                {
                    var point = hit.point;
                    point = GetComponent<Collider>().ClosestPointOnBounds(point);
                    SetThumbPosition(point);
                    SendMessage("OnDrag", Vector3.one - (thumb.position - GetComponent<Collider>().bounds.min) / GetComponent<Collider>().bounds.size.x);
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
}
