using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Text))]
public class PermissionText : MonoBehaviour {

	private static string LOG_TAG = "StoragePermission_Test";
	private Text textField;
	private bool permission_granted = false;

	// Use this for initialization
	void Start () {
		Log.d(LOG_TAG, "get instance at start");
		textField = GetComponent<Text>();
		permission_granted = WaveVR_PermissionManager.instance.isPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE");
		if (permission_granted)
		{
			textField.text = "";
		}
		else
		{
			textField.text = "Warning : \n This APP was not granted android.permission.EXTERNAL_STORAGE yet.";
		}
	}

	// Update is called once per frame
	void Update () {

	}

	void OnApplicationPause(bool pauseStatus)
	{
		permission_granted = WaveVR_PermissionManager.instance.isPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE");
		if (permission_granted)
		{
			textField.text = "";
		}
		else
		{
			textField.text = "Warning : \n This APP was not granted android.permission.EXTERNAL_STORAGE yet.";
		}
	}
}
