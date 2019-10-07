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
using wvr;
using WVR_Log;
using UnityEngine.SceneManagement;

public class PassengerModeHandle : MonoBehaviour
{

	private static string LOG_TAG = "PassengerModeHandle";
	private WaveVR_PermissionManager pmInstance = null;
	private WVR_NumDoF _PassengerModeinit ;
	private int _passengerModestatus;
	private string readValue;

	// Use this for initialization
	void Start()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
	}

	// Update is called once per frame
	void Update()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
	}

	private void OnEnable()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
		_PassengerModeinit = Interop.WVR_GetDegreeOfFreedom(WVR_DeviceType.WVR_DeviceType_HMD);
		Log.w(LOG_TAG, "OnEnable, _PassengerModeinit = " + _PassengerModeinit);
	}

	void OnDisable()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
	}

	private const string CONTENT_PROVIDER_CLASSNAME = "com.htc.vr.unity.ContentProvider";
	private AndroidJavaObject contentProvider = null;

	public void setPassengerMode3dof()
	{
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			if (pmInstance != null)
			{
				Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataWrite) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataWrite"));
				Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataRead) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataRead"));
				Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataProvider) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataProvider"));
			}

			AndroidJavaClass ajc = new AndroidJavaClass(CONTENT_PROVIDER_CLASSNAME);
			if (ajc == null)
			{
				Log.e(LOG_TAG, "setPassengerMode3dof() " + CONTENT_PROVIDER_CLASSNAME + " is null");
				return;
			}
			// Get the PermissionManager object
			contentProvider = ajc.CallStatic<AndroidJavaObject>("getInstance");
			if (contentProvider != null)
			{
				readValue = readPassengerMode_value();
				int value = System.Convert.ToInt32(readValue);
				Log.d(LOG_TAG, "readPassengerModevalue() " + CONTENT_PROVIDER_CLASSNAME + readValue);
				_passengerModestatus = value;
				Log.d(LOG_TAG, "setPassengerMode3dof , _passengerModestatus " + _passengerModestatus);
				int mask = 0x02;
				//int _setPassengerModestatus = 115;
				_passengerModestatus = _passengerModestatus | mask;
				Log.d(LOG_TAG, "setPassengerMode3dof() got instance of " + CONTENT_PROVIDER_CLASSNAME + ", change virtual wall status to " + _passengerModestatus);
				contentProvider.Call("writePassengerModeValue", _passengerModestatus);
			}
			else
			{
				Log.e(LOG_TAG, "setPassengerMode3dof() could NOT get instance of " + CONTENT_PROVIDER_CLASSNAME);
			}
		}
	}

	public void setPassengerModeOff()
	{
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			if (pmInstance != null)
			{
				Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataWrite) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataWrite"));
				Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataRead) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataRead"));
				Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataProvider) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataProvider"));
			}

			AndroidJavaClass ajc = new AndroidJavaClass(CONTENT_PROVIDER_CLASSNAME);
			if (ajc == null)
			{
				Log.e(LOG_TAG, "setPassengerModeOff() " + CONTENT_PROVIDER_CLASSNAME + " is null");
				return;
			}
			// Get the PermissionManager object
			contentProvider = ajc.CallStatic<AndroidJavaObject>("getInstance");
			if (contentProvider != null)
			{
				readValue = readPassengerMode_value();
				int value = System.Convert.ToInt32(readValue);
				Log.d(LOG_TAG, "readPassengerModevalue() " + CONTENT_PROVIDER_CLASSNAME + readValue);
				_passengerModestatus = value;
				Log.d(LOG_TAG, "setPassengerModeOff , _passengerModestatus " + _passengerModestatus);
				int mask = 0xfd;
				_passengerModestatus = _passengerModestatus & mask;
				//int _setPassengerModestatus = 113;
				Log.d(LOG_TAG, "setPassengerModeOff() got instance of " + CONTENT_PROVIDER_CLASSNAME + ", change virtual wall status to " + _passengerModestatus);
				contentProvider.Call("writePassengerModeValue", _passengerModestatus);
			}
			else
			{
				Log.e(LOG_TAG, "setPassengerModeOff() could NOT get instance of " + CONTENT_PROVIDER_CLASSNAME);
			}
		}
	}
	void OnDestroy()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
			return;
#endif
		if (_PassengerModeinit.ToString() == "WVR_NumDoF_3DoF")
			setPassengerMode3dof();
		else
			setPassengerModeOff();
	}

	private void OnSceneUnloaded(Scene current)
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
			return;
#endif
		if (_PassengerModeinit.ToString() == "WVR_NumDoF_3DoF")
			setPassengerMode3dof();
		else
			setPassengerModeOff();
	}
	public string readPassengerMode_value()
	{
		return contentProvider.Call<string>("readPassengerModeValue");
	}
}


