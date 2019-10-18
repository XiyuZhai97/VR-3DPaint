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

    void Update()
    {
        // if (teleportOn)
        // {
        //     //grip release, try to teleport
        //     if (WaveVR_Controller.Input(DomFocusControllerType).GetPressUp(WVR_InputId.WVR_InputId_Alias1_Grip))
        //     {
        //         teleportOn = false;
        //         hasPosition = false;
        //     }
        //     else //otherwise update sprite
        //     {
                hasPosition = UpdateSprite();
                teleportSprite.SetActive(true);
        //         teleportSprite.SetActive(hasPosition); //only visible if we can teleport there
        //     }
        // }
        //to turn on teleport, check for just right grip down
        // else if (WaveVR_Controller.Input(DomFocusControllerType).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Grip))
        //     //&& !WaveVR_Controller.Input(NonFocusControllerType).GetPress(WVR_InputId.WVR_InputId_Alias1_Grip))
        // {
        //     teleportOn = true;
        // }
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
}
