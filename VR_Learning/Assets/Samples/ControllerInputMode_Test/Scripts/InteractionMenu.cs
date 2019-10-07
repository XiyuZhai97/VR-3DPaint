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
using UnityEngine.UI;
using UnityEngine.EventSystems;
using WVR_Log;
using wvr;

[RequireComponent(typeof(Toggle))]
public class InteractionMenu : MonoBehaviour
{
	private const string LOG_TAG = "InteractionMenu";
	private Canvas WarningMessage = null;
	private Toggle mToggle;
	private bool isWarningShow = false;
	private float warningStartTime;
	private float warningTime = 10.0f;

	private void PrintDebugLog(string msg)
	{
		Log.d(LOG_TAG, msg);
	}

	void Start()
	{
		mToggle = GetComponent<Toggle>();
		mToggle.onValueChanged.AddListener(delegate {
				ToggleValueChanged(mToggle);
			  });
		GameObject warnObj = GameObject.Find("Warning Canvas");
		if (warnObj != null)
		{
			WarningMessage = warnObj.GetComponent<Canvas>();
			if (WarningMessage != null)
			{
				WarningMessage.enabled = false;
			}
		}
	}

	void Update()
	{
		switch (mToggle.name)
		{
		case "OverrideDefaultToggle":
			if (WaveVR_InputModuleManager.Instance != null && mToggle.isOn != WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
				mToggle.isOn = WaveVR_InputModuleManager.Instance.OverrideSystemSettings;
			break;
		case "EnableControllerToggle":
			if (WaveVR_InputModuleManager.Instance != null)
			{
				mToggle.isOn = WaveVR_InputModuleManager.Instance.CustomInputModule == WaveVR_EInputModule.Controller;
			}
			break;
		case "EnableGazeToggle":
			if (WaveVR_InputModuleManager.Instance != null)
			{
				mToggle.isOn = WaveVR_InputModuleManager.Instance.CustomInputModule == WaveVR_EInputModule.Gaze;
			}
			break;
		}
		CheckWarningStatus();
	}

	void ToggleValueChanged(Toggle change)
	{
		PrintDebugLog ("ToggleValueChanged() " + change);
		switch (change.name)
		{
		case "OverrideDefaultToggle":
			SetOverrideDefault (mToggle.isOn);
			break;
		case "EnableControllerToggle":
			if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
			{
				if (mToggle.isOn)
					SetEnableController ();
				else
					mToggle.isOn = WaveVR_InputModuleManager.Instance.CustomInputModule == WaveVR_EInputModule.Controller;
			}
			break;
		case "EnableGazeToggle":
			if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
			{
				if (mToggle.isOn)
					SetEnableGaze ();
				else
				{
					mToggle.isOn = WaveVR_InputModuleManager.Instance.CustomInputModule == WaveVR_EInputModule.Gaze;
				}
			}
			break;
		}
	}

	void SetOverrideDefault(bool value)
	{
		PrintDebugLog ("SetOverrideDefault: " + value);
		if (WaveVR_InputModuleManager.Instance != null)
		{
			WaveVR_InputModuleManager.Instance.OverrideSystemSettings = value;
			PrintDebugLog("WaveVR_InputModuleManager.Instance.OverrideSystemSettings = " + value);
		}
	}

	void SetEnableGaze()
	{
		PrintDebugLog ("SetEnableGaze()");
		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.Gaze != null)
		{
			WaveVR_InputModuleManager.Instance.CustomInputModule = WaveVR_EInputModule.Gaze;
		}
		//CheckInputStatus();
	}

	void SetEnableController()
	{
		PrintDebugLog ("SetEnableController()");
		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.Controller != null)
		{
			WaveVR_InputModuleManager.Instance.CustomInputModule = WaveVR_EInputModule.Controller;
		}
		//CheckInputStatus();
	}

	void CheckInputStatus()
	{
		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.Controller != null && WaveVR_InputModuleManager.Instance.Gaze != null)
		{
			//if (!WaveVR_InputModuleManager.Instance.Controller.EnableController && !WaveVR_InputModuleManager.Instance.Gaze.EnableGaze)
			{
				if (WarningMessage != null)
				{
					WarningMessage.enabled = true;
				}
				warningStartTime = Time.unscaledTime;
				isWarningShow = true;
			}
		}
	}

	void CheckWarningStatus()
	{
		if (isWarningShow)
		{
			if (Time.unscaledTime - warningStartTime >= warningTime)
			{
				if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.Controller != null && WaveVR_InputModuleManager.Instance.Gaze != null)
				{
					if (WarningMessage != null)
					{
						WarningMessage.enabled = false;
					}
					WaveVR_InputModuleManager.Instance.CustomInputModule = WaveVR_EInputModule.Controller;
					isWarningShow = false;
				}
			}
		}
	}
}