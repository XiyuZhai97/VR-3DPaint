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

public class SoftwareIpdHandle : MonoBehaviour
{

	private static string LOG_TAG = "SoftwareIpdHandle";
	private WaveVR_PermissionManager pmInstance = null;
	private int softwareIpdValueInit;
	private string readValue = null;
	private string SoftwareIpdStatus = null;
	private const string CONTENT_PROVIDER_CLASSNAME = "com.htc.vr.unity.ContentProvider";
	private AndroidJavaObject contentProvider = null;
	private bool SoftwareIpdIsEnable = false;
	public Text textField;

	// Use this for initialization
	void Start()
	{
		checkServiceVersion();
		if (pmInstance != null)
		{
			Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataWrite) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataWrite"));
			Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataRead) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataRead"));
		}

		AndroidJavaClass ajc = new AndroidJavaClass(CONTENT_PROVIDER_CLASSNAME);
		if (ajc == null)
		{
			Log.e(LOG_TAG, "Start() " + CONTENT_PROVIDER_CLASSNAME + " is null");
			return;
		}
		contentProvider = ajc.CallStatic<AndroidJavaObject>("getInstance");
		if (contentProvider != null)
		{
			readValue = readSoftwareIpdValue();
			int value = System.Convert.ToInt32(readValue);
			softwareIpdValueInit = value;
		}
		else
		{
			Log.e(LOG_TAG, "start() could NOT get instance of " + CONTENT_PROVIDER_CLASSNAME);
		}
		Log.d(LOG_TAG, "softwareIpdValueInit " + CONTENT_PROVIDER_CLASSNAME + softwareIpdValueInit);
	}

	private void checkServiceVersion()
	{
		string str = string.Empty;
		uint version = Interop.WVR_GetWaveRuntimeVersion();
		if (version < 3)
		{
			str = "API Level is : " + version.ToString() + ",so not support this test";
			this.textField.text = str;
		}
		else
		{
			str = "API Level is : " + version.ToString();
			this.textField.text = str;
		}
	}

	// Update is called once per frame
	void Update()
	{
	}

	private void OnApplicationQuit()
	{
		SoftwareIpdonDisable();
	}

	public void quitGame()
	{
		Application.Quit();
	}

	void OnDisable()
	{
		SoftwareIpdonDisable();
	}

	public string readSoftwareIpdValue()
	{
		//string _readValue = null;
		return contentProvider.Call<string>("readSoftwareIpdValue");
	}

	public void SoftwareIpdonEnable()
	{
		if (SoftwareIpdIsEnable != true)
		{
			double value = 0.056;
			string SWIpd_value = value.ToString();
			writesoftwarevalue(true.ToString(), SWIpd_value);
			SoftwareIpdIsEnable = true;
		}
	}

	public void SoftwareIpdonDisable()
	{
		if (SoftwareIpdIsEnable == true)
		{
			double value = 0.056;
			string SWIpd_value = value.ToString();
			writesoftwarevalue(false.ToString(), SWIpd_value);
			SoftwareIpdIsEnable = false;
		}
	}

	public void setValueLow()//0.056
	{
		setValueInternal(0.056f);
	}

	public void setValueMedieum()//0.063
	{
		setValueInternal(0.063f);
	}

	public void setValueHigh()//0.07
	{
		setValueInternal(0.07f);
	}

	private void setValueInternal(double value)
	{
		string SWIpd_value = value.ToString();
		writesoftwarevalue(true.ToString(), SWIpd_value);
		SoftwareIpdIsEnable = true;
	}

	public void writesoftwarevalue(string enable, string value)
	{
		string inputvalue = value.ToString();
		string input = enable + "/" + inputvalue;
		if (pmInstance != null)
		{
			Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataWrite) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataWrite"));
			Log.d(LOG_TAG, "isPermissionGranted(com.htc.vr.core.server.VRDataRead) = " + pmInstance.isPermissionGranted("com.htc.vr.core.server.VRDataRead"));
		}

		AndroidJavaClass ajc = new AndroidJavaClass(CONTENT_PROVIDER_CLASSNAME);
		if (ajc == null)
		{
			Log.e(LOG_TAG, "SoftwareIpdHandle() " + CONTENT_PROVIDER_CLASSNAME + " is null");
			return;
		}
		contentProvider = ajc.CallStatic<AndroidJavaObject>("getInstance");
		if (contentProvider != null)
		{
			Log.d(LOG_TAG, "SoftwareIpdHandle() got instance of " + CONTENT_PROVIDER_CLASSNAME + ", change SoftwareIpd value to " + value);
			contentProvider.Call("writeSoftwareIpdValue", input);
		}
		else
		{
			Log.e(LOG_TAG, "SoftwareIpd could NOT get instance of " + CONTENT_PROVIDER_CLASSNAME);
		}
	}
}
