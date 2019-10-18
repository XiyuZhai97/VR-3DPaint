using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WVR_Log;
using wvr;
public class Navigator : MonoBehaviour
{
    WaveVR_Controller.EDeviceType DomFocusControllerType = WaveVR_Controller.EDeviceType.Dominant;
    WaveVR_Controller.EDeviceType NonFocusControllerType = WaveVR_Controller.EDeviceType.NonDominant;

    public GameObject teleportSprite; //indicator of where you'll teleport to
    public bool teleportOn = false;
    private bool hasPosition = false;
    private bool firstClick;
    private float firstClickTime;
    private float doubleClickTimeLimit = 0.5f;

    void Update()
    {
        if (teleportOn)
        {
            hasPosition = UpdateSprite();
            teleportSprite.SetActive(hasPosition); //only visible if we can teleport there
            
            //trigger down, teleport if possible
            if (hasPosition && WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
            {
                Vector3 teleportPos = teleportSprite.transform.position;
                Vector3 oldPos = this.gameObject.transform.position;
                this.gameObject.transform.position = new Vector3(teleportPos.x, oldPos.y, teleportPos.z);

                ToggleTeleportMode();
            }
        }
        
        //double click to toggle teleport mode
        if (WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger)) { // The trigger is pressed.
            if (!firstClick) { // The first click is detected.
                firstClick = true;
                firstClickTime = Time.unscaledTime;
            } else { // The second click detected, so toggle teleport mode.
                firstClick = false;
                ToggleTeleportMode();
            }
        }

        if (Time.unscaledTime - firstClickTime > doubleClickTimeLimit) { // Time for the double click has run out.
            firstClick = false;
        }
    }

    /* moves the teleport sprite to the teleport position, if possible
     * returns whether a teleport position was found
     */
    private bool UpdateSprite()
    {
        WaveVR_Utils.RigidTransform _rpose = WaveVR_Controller.Input(DomFocusControllerType).transform;
        Vector3 pos = _rpose.pos;
        Vector3 euler = _rpose.rot.eulerAngles;
        float elevation = - Mathf.Deg2Rad * (euler[0] + 35.0f);
        float heading = Mathf.Deg2Rad * euler[1];
        Vector3 forward = new Vector3(Mathf.Cos(elevation) * Mathf.Sin(heading), Mathf.Sin(elevation), Mathf.Cos(elevation) * Mathf.Cos(heading));
        Ray ray = new Ray(pos, forward);
        RaycastHit hit;

        //if ray from controller hits a surface, move sprite there
        if (Physics.Raycast(ray, out hit) && hit.transform.tag == "floor")
        {
            teleportSprite.transform.position = hit.point;
            return true;
        }
        
        //no hit
        return false;
    }

    void ToggleTeleportMode() {
        teleportOn = !teleportOn;
        if (!teleportOn) {
            hasPosition = false;
            teleportSprite.SetActive(false);
        }
    }
}
