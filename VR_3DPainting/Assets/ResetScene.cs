using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WVR_Log;
using wvr;
public class ResetScene : MonoBehaviour,
IPointerEnterHandler,
IPointerExitHandler
{
    WaveVR_Controller.EDeviceType NonFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;
    public bool isControllerFocus_R;
    public bool isControllerFocus_L;
    private GameObject m_RightController;
    private GameObject m_LeftController;

    public string SceneName = "VR_3DPainting";
    void Update()
    {
        if (isControllerFocus_R || isControllerFocus_L)
        {
            if (WaveVR_Controller.Input(NonFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Touchpad))
            {
                Reset();

                WaveVR_Controller.Input(NonFocusControllerType).TriggerHapticPulse();

            }
            
        }
    }

    public void Reset()
    {

        SceneManager.LoadScene(SceneName);

    }

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

            // Right-Hand mode
            if (!WaveVR_Controller.IsLeftHanded)
            {
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
            }

            // Left-Hand mode
            else if (WaveVR_Controller.IsLeftHanded)
            {
                GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            }
        }
        NonFocusControllerType = type;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RaycastHit hit;
        
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
}
