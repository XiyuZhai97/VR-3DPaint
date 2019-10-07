using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using wvr;
using WVR_Log;

public class InteractionModeHandle : MonoBehaviour {
	private static string LOG_TAG = "InteractionModeHandle";
	private WaveVR_PermissionManager pmInstance = null;
	private static bool isDeny = false;
	private static int retryCount = 0;
	private static int RETRY_LIMIT = 0;
	//private static bool requested = false;
	private bool isGazeMode = false;
	private const string DB_SETTINGS_CLASSNAME = "com.htc.vr.unity.InteractionModeSetting";
	private const string DB_SETTINGS_CALLBACK_CLASSNAME = "com.htc.vr.unity.InteractionModeSettingCallback";
	private AndroidJavaObject dbSetting = null;
	public Text textField;
	private bool IsSupport;

	// Use this for initialization
	void Start()
	{
		IsSupport = checkServiceVersion();
		SetOverrideDefault(false);
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
		else
#endif
		{
			pmInstance = WaveVR_PermissionManager.instance;
			if (pmInstance != null)
			{
				Log.d(LOG_TAG, "isPermissionGranted(vive.wave.vr.oem.data.OEMDataWrite) = " + pmInstance.isPermissionGranted("vive.wave.vr.oem.data.OEMDataWrite"));
				Log.d(LOG_TAG, "isPermissionGranted(vive.wave.vr.oem.data.OEMDataRead) = " + pmInstance.isPermissionGranted("vive.wave.vr.oem.data.OEMDataRead"));
			}
			using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
			{
				Log.d(LOG_TAG, " init AndroidJavaClass");
				using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
				{
					AndroidJavaClass ajc_setting = new AndroidJavaClass(DB_SETTINGS_CLASSNAME);
					if (ajc_setting == null)
					{
						Log.e(LOG_TAG, "Interaction mode Start() " + DB_SETTINGS_CLASSNAME + " is null");
						return;
					}
					dbSetting = ajc_setting.CallStatic<AndroidJavaObject>("getInstance", jo, new RequestCompleteHandler());
					if (dbSetting == null)
					{
						Log.e(LOG_TAG, "Interaction mode Start() could NOT get instance of " + DB_SETTINGS_CLASSNAME);
					}
					Log.d(LOG_TAG, "Interaction mode Start() : " + DB_SETTINGS_CLASSNAME );
				}
			}
		}
	}

	void OnDisable()
	{
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			Log.d(LOG_TAG, " init AndroidJavaClass");
			AndroidJavaClass ajc_setting = new AndroidJavaClass(DB_SETTINGS_CLASSNAME);
			if (ajc_setting == null)
			{
				Log.e(LOG_TAG, "writeinteractionvalue() " + DB_SETTINGS_CLASSNAME + " is null");
				return;
			}
			if (dbSetting != null)
			{
				dbSetting.Call("onDisable");
			}
			else
			{
				Log.e(LOG_TAG, "writeinteractionvalue() could NOT get instance of " + DB_SETTINGS_CLASSNAME);
			}
		}
	}

	private bool checkServiceVersion()
	{
		string str = string.Empty;
		uint version = Interop.WVR_GetWaveRuntimeVersion();
		if (version < 2) //2.0.92 support interaction mode function
		{
			str = "Service version is : " + version.ToString() + ",so not support this test";
			this.textField.text = str;
			return false;
		}
		else
		{
			str = "Service version is : " + version.ToString();
			this.textField.text = str;
			return true;
		}
	}

	public void changetoControllerMode() {
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			if (IsSupport == true)
			{
				writeinteractionvalue(3);
				isGazeMode = false;
			}
		}
	}

	public void changetoGazeMode() {
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			if (IsSupport == true) { 
				writeinteractionvalue(2);
				isGazeMode = true;
			}
		}
	}

	public void gaze_timeoutMode()
	{
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			if (IsSupport == true)
				writeGazeTriggermodevalue(1);
		}
	}

	public void gaze_buttonMode() {
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			if (IsSupport == true)
				writeGazeTriggermodevalue(2);
		}
	}

	public void gaze_TimeoutandButtonMode() {
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			if (IsSupport == true)
				writeGazeTriggermodevalue(3);
		}
	}

	// Update is called once per frame
	void Update () {
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
#endif
	}

	private string[] getDBsetting()
	{
		Log.d(LOG_TAG, "getDBsetting ");
		return dbSetting.Call<string[]>("getDBSetting");
	}

	public void writeinteractionvalue(int value)
	{
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			Log.d(LOG_TAG, " init AndroidJavaClass");
			AndroidJavaClass ajc_setting = new AndroidJavaClass(DB_SETTINGS_CLASSNAME);
			if (ajc_setting == null)
			{
				Log.e(LOG_TAG, "writeinteractionvalue() " + DB_SETTINGS_CLASSNAME + " is null");
				return;
			}
			if (dbSetting != null)
			{
				string valueStr = value.ToString();
				Log.d(LOG_TAG, "writeinteractionvalue() got instance of " + DB_SETTINGS_CLASSNAME + ", change Interaction mode to " + valueStr);
				dbSetting.Call("setInteractionModeValue", valueStr);
			}
			else
			{
				Log.e(LOG_TAG, "writeinteractionvalue() could NOT get instance of " + DB_SETTINGS_CLASSNAME);
			}
		}
	}

	public void writeGazeTriggermodevalue(int value)
	{
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			if (isGazeMode == true)
			{
				Log.d(LOG_TAG, " init AndroidJavaClass");

				AndroidJavaClass ajc_setting = new AndroidJavaClass(DB_SETTINGS_CLASSNAME);
				if (ajc_setting == null)
				{
					Log.e(LOG_TAG, "write GazeTrigger mode value() " + DB_SETTINGS_CLASSNAME + " is null");
					return;
				}
				if (dbSetting != null)
				{
					string valueStr = value.ToString();
					Log.d(LOG_TAG, "write GazeTrigger mode value()  got instance of " + DB_SETTINGS_CLASSNAME + ", change Gaze Trigger mode to " + valueStr);
					dbSetting.Call("setGazemodeTriggerType", valueStr);
				}
				else
				{
					Log.e(LOG_TAG, "write Gaze Trigger Mode Value() could NOT get instance of " + DB_SETTINGS_CLASSNAME);
				}
			}
		}
	}

	public static void requestDoneCallback(List<WaveVR_PermissionManager.RequestResult> results)
	{
		Log.d(LOG_TAG, "requestDoneCallback, count = " + results.Count);
		isDeny = false;

		foreach (WaveVR_PermissionManager.RequestResult p in results)
		{
			Log.d(LOG_TAG, "requestDoneCallback " + p.PermissionName + ": " + (p.Granted ? "Granted" : "Denied"));
			if (!p.Granted)
			{
				isDeny = true;
			}
		}

		if (isDeny)
		{
			if (retryCount++ < RETRY_LIMIT)
			{
				Log.d(LOG_TAG, "Permission denied, retry count = " + retryCount);
				//requested = false;
			}
			else
			{
				Log.w(LOG_TAG, "Permission denied, exceed RETRY_LIMIT and skip request");
			}
		}
	}

	void SetOverrideDefault(bool value)
	{
		Log.d(LOG_TAG, "SetOverrideDefault: " + value);
		if (WaveVR_InputModuleManager.Instance != null)
		{
			WaveVR_InputModuleManager.Instance.OverrideSystemSettings = value;
			Log.d(LOG_TAG, "WaveVR_InputModuleManager.Instance.OverrideSystemSettings = " + value);
		}
	}

	class RequestCompleteHandler : AndroidJavaProxy
	{
		internal RequestCompleteHandler() : base(new AndroidJavaClass(DB_SETTINGS_CALLBACK_CLASSNAME))
		{
		}
		public void onRequestCompletedwithObject(string s)
		{
			Log.d("onRequestCompletedwithObject", " callback from init = " + s);
		}
	}
}
