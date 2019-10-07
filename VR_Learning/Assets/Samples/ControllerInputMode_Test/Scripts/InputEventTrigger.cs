// "WaveVR SDK
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;
using wvr;
using WVR_Log;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class InputEventTrigger : MonoBehaviour
{
	private static string LOG_TAG = "InputEventTrigger";
	private void PrintDebugLog(string msg)
	{
		Log.d (LOG_TAG, msg);
	}
	private Vector3 startPosition;
	private Color defaultColor = Color.gray;
	private Color changedColor = Color.red;

	private WaveVR_PermissionManager pmInstance = null;

	void Start ()
	{
		startPosition = transform.localPosition;

		if (!Application.isEditor)
		{
			Log.d (LOG_TAG, "Start() get instance of WaveVR_PermissionManager");
			pmInstance = WaveVR_PermissionManager.instance;
		}
	}

	public void OnEnter()
	{
		Log.d (LOG_TAG, "OnEnter");
		ChangeColor (true);
	}

	public void OnTrigger()
	{
		Log.d (LOG_TAG, "OnTrigger");
		TeleportRandomly ();
	}

	public void OnExit()
	{
		Log.d (LOG_TAG, "OnExit");
		ChangeColor (false);
	}

	public void BackToUpLayer()
	{
		SceneManager.LoadScene (0);
	}

	public void OnQuitGame()
	{
		Log.d(LOG_TAG, "Quit Game");
		Application.Quit();
	}

	public void OnGazeReset ()
	{
		transform.localPosition = startPosition;
		ChangeColor (false);
	}

	public void OnShowButton()
	{
		PrintDebugLog ("OnShowButton");
		transform.gameObject.SetActive (true);
	}

	public void OnHideButton()
	{
		PrintDebugLog ("OnHideButton");
		transform.gameObject.SetActive (false);
	}

	public void OnBeamButton()
	{
		PrintDebugLog ("OnBeamButton");
		if (WaveVR_InputModuleManager.Instance != null)
			WaveVR_InputModuleManager.Instance.Controller.RaycastMode = WaveVR_ControllerInputModule.ERaycastMode.Beam;
	}

	public void OnFixedButton()
	{
		PrintDebugLog ("OnFixedButton");
		if (WaveVR_InputModuleManager.Instance != null)
			WaveVR_InputModuleManager.Instance.Controller.RaycastMode = WaveVR_ControllerInputModule.ERaycastMode.Fixed;
	}

	public void OnMouseButton()
	{
		PrintDebugLog ("OnMouseButton");
		if (WaveVR_InputModuleManager.Instance != null)
			WaveVR_InputModuleManager.Instance.Controller.RaycastMode = WaveVR_ControllerInputModule.ERaycastMode.Mouse;
	}

	public void OnSelectCtrlrButton()
	{
		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
		{
			if (WaveVR_InputModuleManager.Instance.Controller != null && WaveVR_InputModuleManager.Instance.Gaze != null)
			{
				WaveVR_InputModuleManager.Instance.CustomInputModule = WaveVR_EInputModule.Controller;
				PrintDebugLog ("Set EnableController to true and EnableGaze to false.");
			}
		}
	}

	public void OnSelectGazeButton()
	{
		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
		{
			if (WaveVR_InputModuleManager.Instance.Controller != null && WaveVR_InputModuleManager.Instance.Gaze != null)
			{
				WaveVR_InputModuleManager.Instance.CustomInputModule = WaveVR_EInputModule.Gaze;
				PrintDebugLog ("Set EnableController to false and EnableGaze to true.");
			}
		}
	}

	public void OnSelectGazeCtrlrButton()
	{
		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
		{
			if (WaveVR_InputModuleManager.Instance.Controller != null && WaveVR_InputModuleManager.Instance.Gaze != null)
			{
				WaveVR_InputModuleManager.Instance.CustomInputModule = WaveVR_EInputModule.Controller;
				PrintDebugLog ("Set EnableController to true and EnableGaze to true.");
			}
		}
	}

	public void OnSelectGazeTimeoutTriggerButton()
	{
		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.Gaze != null && WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
		{
			WaveVR_InputModuleManager.Instance.Gaze.ButtonControl = false;
			WaveVR_InputModuleManager.Instance.Gaze.TimerControl = false;
			PrintDebugLog ("Set timer control.");
		}
	}

	public void OnSelectGazeButtonTriggerButton()
	{
		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.Gaze != null && WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
		{
			WaveVR_InputModuleManager.Instance.Gaze.ButtonControl = true;
			WaveVR_InputModuleManager.Instance.Gaze.TimerControl = false;
			PrintDebugLog ("Set button control without timer.");
		}
	}

	public void OnSelectGazeButtonTimeoutTriggerButton()
	{
		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.Gaze != null && WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
		{
			WaveVR_InputModuleManager.Instance.Gaze.ButtonControl = true;
			WaveVR_InputModuleManager.Instance.Gaze.TimerControl = true;
			PrintDebugLog ("Set button control with timer.");
		}
	}

	private const string CONTENT_PROVIDER_CLASSNAME = "com.htc.vr.unity.ContentProvider";
	private AndroidJavaObject contentProvider = null;
	public void OnChangeHand()
	{
		#if UNITY_EDITOR
		if (Application.isEditor)
		{
			WaveVR_Controller.SetLeftHandedMode(WaveVR_Controller.IsLeftHanded ? false : true);
		} else
		#endif
		{
			if (pmInstance != null)
			{
				Log.d (LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataWrite) = " + pmInstance.isPermissionGranted ("com.htc.vr.core.server.VRDataWrite"));
				Log.d (LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataRead) = " + pmInstance.isPermissionGranted ("com.htc.vr.core.server.VRDataRead"));
				Log.d (LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataProvider) = " + pmInstance.isPermissionGranted ("com.htc.vr.core.server.VRDataProvider"));
			}

			AndroidJavaClass ajc = new AndroidJavaClass(CONTENT_PROVIDER_CLASSNAME);
			if (ajc == null)
			{
				Log.e(LOG_TAG, "OnChangeHand() " + CONTENT_PROVIDER_CLASSNAME + " is null");
				return;
			}
			// Get the PermissionManager object
			contentProvider = ajc.CallStatic<AndroidJavaObject>("getInstance");
			if (contentProvider != null)
			{
				string _role = Interop.WVR_GetDefaultControllerRole () == WVR_DeviceType.WVR_DeviceType_Controller_Left ? "2" : "1";
				Log.d (LOG_TAG, "OnChangeHand() got instance of " + CONTENT_PROVIDER_CLASSNAME + ", change role to " + _role);
				contentProvider.Call ("writeControllerRoleValue", _role);
			} else
			{
				Log.e (LOG_TAG, "OnChangeHand() could NOT get instance of " + CONTENT_PROVIDER_CLASSNAME);
			}
		}
	}

	public void ChangeColor(string color)
	{
		if (color.Equals("blue"))
			GetComponent<Renderer>().material.color = Color.blue;
		else if (color.Equals("cyan"))
			GetComponent<Renderer>().material.color = Color.cyan;
	}

	private void ChangeColor(bool change)
	{
		GetComponent<Renderer>().material.color = change ? changedColor : defaultColor;
	}

	private void TeleportRandomly () {
		Vector3 direction = UnityEngine.Random.onUnitSphere;
		direction.y = Mathf.Clamp (direction.y, 0.5f, 1f);
		direction.z = Mathf.Clamp (direction.z, 3f, 10f);
		float distance = 2 * UnityEngine.Random.value + 1.5f;
		transform.localPosition = direction * distance;
	}
}
