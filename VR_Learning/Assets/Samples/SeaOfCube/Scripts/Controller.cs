using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;

public class Controller : MonoBehaviour {
	private static string LOG_TAG = "Controller";
	public WVR_DeviceType device = WVR_DeviceType.WVR_DeviceType_HMD;
	public GameObject ControlledObject;

	// Update is called once per frame
	void Update () {
		if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Trigger))
		{
			Log.d(LOG_TAG, "button " + WVR_InputId.WVR_InputId_Alias1_Trigger + " press down");

			ControlledObject.SetActive(true);
		}

		if (WaveVR_Controller.Input(device).GetPressDown(WVR_InputId.WVR_InputId_Alias1_Digital_Trigger))
		{
			Log.d(LOG_TAG, "button " + WVR_InputId.WVR_InputId_Alias1_Digital_Trigger + " press down");

			ControlledObject.SetActive(true);
		}
	}
}
