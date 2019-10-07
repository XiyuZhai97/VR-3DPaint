// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using wvr;
using WVR_Log;

[RequireComponent(typeof(Text))]
public class ScreenShotText : MonoBehaviour {

	public Text textField;
	string screenShot_info = "";
	private WaveVR_PermissionManager pmInstance = null;
	private bool permission_granted = false;
	// Use this for initialization
	void Start () {
		textField = GetComponent<Text>();
	}

	// Update is called once per frame
	void Update () {
	}

	public void onDefaultClickText()
	{
		pmInstance = WaveVR_PermissionManager.instance;
		permission_granted = pmInstance.isPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE");
		if (permission_granted)
		{
			screenShot_info = "Unity_Default screenshot is finished";
			textField.text = screenShot_info;
		}
		else
		{
			screenShot_info = "Unity_Default screenshot is failed";
			textField.text = screenShot_info;
		}
	}

	public void onDistortedClickText()
	{
		pmInstance = WaveVR_PermissionManager.instance;
		permission_granted = pmInstance.isPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE");
		if (permission_granted)
		{
			screenShot_info = "Unity_Distorted screenshot is finished";
			textField.text = screenShot_info;
		}
		else
		{
			screenShot_info = "Unity_Distorted screenshot is failed";
			textField.text = screenShot_info;
		}
	}

	public void onRawClickText()
	{
		pmInstance = WaveVR_PermissionManager.instance;
		permission_granted = pmInstance.isPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE");
		if (permission_granted)
		{
			screenShot_info = "Unity_Raw screenshot is finished";
			textField.text = screenShot_info;
		}
		else
		{
			screenShot_info = "Unity_Raw screenshot is failed";
			textField.text = screenShot_info;
		}
	}
}
