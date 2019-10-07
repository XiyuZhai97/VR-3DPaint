using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ScreenShotHandle : MonoBehaviour {

	private static string LOG_TAG = "ScreenShotHandle";
	private WaveVR_PermissionManager pmInstance = null;
	private bool permission_granted = false;
	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {

	}

	public void onDefaultClick()
	{
		pmInstance = WaveVR_PermissionManager.instance;
		permission_granted = pmInstance.isPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE");
		if (permission_granted) {
			WaveVR_Screenshot.requestScreenshot(WVR_ScreenshotMode.WVR_ScreenshotMode_Default, "Unity_Default");
		}
		else
		{
			Log.e(LOG_TAG, "permissionGranted is failed");
		}
	}

	public void onDistortedClick()
	{
		pmInstance = WaveVR_PermissionManager.instance;
		permission_granted = pmInstance.isPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE");
		if (permission_granted)
		{
			WaveVR_Screenshot.requestScreenshot(WVR_ScreenshotMode.WVR_ScreenshotMode_Distorted, "Unity_Distorted");
		}
		else
		{
			Log.e(LOG_TAG, "permissionGranted is failed");
		}
	}

	public void onRawClick()
	{
		pmInstance = WaveVR_PermissionManager.instance;
		permission_granted = pmInstance.isPermissionGranted("android.permission.WRITE_EXTERNAL_STORAGE");
		if (permission_granted)
		{
			WaveVR_Screenshot.requestScreenshot(WVR_ScreenshotMode.WVR_ScreenshotMode_Raw, "Unity_Raw");
		}
		else
		{
			Log.e(LOG_TAG, "permissionGranted is failed");
		}
	}
}
