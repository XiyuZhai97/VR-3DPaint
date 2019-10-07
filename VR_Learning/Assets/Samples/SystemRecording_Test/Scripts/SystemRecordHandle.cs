using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using wvr;
using WVR_Log;

public class SystemRecordHandle : MonoBehaviour {
	private static string LOG_TAG = "SystemRecordHandle";
	private WaveVR_PermissionManager pmInstance = null;
	private const string DB_SETTINGS_CLASSNAME = "com.htc.vr.unity.RecordSetting";
	private const string DB_SETTINGS_CALLBACK_CLASSNAME = "com.htc.vr.unity.RecordSettingCallback";
	private AndroidJavaObject dbSetting = null;
	public Text textField;

	// Use this for initialization
	void Start () {
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
        pmInstance = WaveVR_PermissionManager.instance;
		if (pmInstance != null)
		{
			Log.d(LOG_TAG, "isPermissionGranted(com.htc.permission.APP_PLATFORM) = " + pmInstance.isPermissionGranted("com.htc.permission.APP_PLATFORM"));
		}
		using (AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
		{
			using (AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity"))
			{
				AndroidJavaClass ajc_setting = new AndroidJavaClass(DB_SETTINGS_CLASSNAME);
				if (ajc_setting == null)
				{
					Log.e(LOG_TAG, " Start() " + DB_SETTINGS_CLASSNAME + " is null");
					return;
				}
				dbSetting = ajc_setting.CallStatic<AndroidJavaObject>("getInstance", jo, new RequestCompleteHandler());
				if (dbSetting == null)
				{
					Log.e(LOG_TAG, " Start() could NOT get instance of " + DB_SETTINGS_CLASSNAME);
				}
				Log.d(LOG_TAG, " Start() : " + DB_SETTINGS_CLASSNAME);
			}
		}
	}

	private void OnApplicationFocus(bool focus)
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
        if (pmInstance != null)
		{
			Log.d(LOG_TAG, "isPermissionGranted(com.htc.permission.APP_PLATFORM) = " + pmInstance.isPermissionGranted("com.htc.permission.APP_PLATFORM"));
		}
		if (focus)
		{
			AndroidJavaClass ajc_setting = new AndroidJavaClass(DB_SETTINGS_CLASSNAME);
			if (ajc_setting == null)
			{
				Log.e(LOG_TAG, "OnApplicationFocus() " + DB_SETTINGS_CLASSNAME + " is null");
				return;
			}
			if (dbSetting != null)
			{
				Log.d(LOG_TAG, "OnApplicationFocus() got instance of " + DB_SETTINGS_CLASSNAME);
				dbSetting.Call("checkConfigServiceIsConnected");
			}
			else
			{
				Log.e(LOG_TAG, "OnApplicationFocus() could NOT get instance of " + DB_SETTINGS_CLASSNAME);
			}
		}
	}

	// Update is called once per frame
	void Update () {
		
	}

	public void startRecord()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
        AndroidJavaClass ajc_setting = new AndroidJavaClass(DB_SETTINGS_CLASSNAME);
		if (ajc_setting == null)
		{
			Log.e(LOG_TAG, "StartScreenRecord() " + DB_SETTINGS_CLASSNAME + " is null");
			return;
		}
		if (dbSetting != null)
		{
			Log.d(LOG_TAG, "StartScreenRecord() got instance of " + DB_SETTINGS_CLASSNAME);
			dbSetting.Call("startRecording");
			string str = string.Empty;
			str = dbSetting.Call<string>("getRecordStatus");
			if (str != null)
			{
				this.textField.text ="Recording status is : " + str;
			}
			else
				this.textField.text = "status is null";
		}
		else
		{
			Log.e(LOG_TAG, "startRecord() could NOT get instance of " + DB_SETTINGS_CLASSNAME);
		}
	}

	public void stopRecord()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
        AndroidJavaClass ajc_setting = new AndroidJavaClass(DB_SETTINGS_CLASSNAME);
		if (ajc_setting == null)
		{
			Log.e(LOG_TAG, "StopScreenRecord() " + DB_SETTINGS_CLASSNAME + " is null");
			return;
		}
		if (dbSetting != null)
		{
			Log.d(LOG_TAG, "StopScreenRecord() got instance of " + DB_SETTINGS_CLASSNAME);
			dbSetting.Call("stopRecording");
			string str = string.Empty;
			str = dbSetting.Call<string>("getRecordStatus");
			if (str != null)
			{
				this.textField.text = "Recording status is : " + str;
			}
			else
				this.textField.text = "status is null";
		}
		else
		{
			Log.e(LOG_TAG, "stopRecord() could NOT get instance of " + DB_SETTINGS_CLASSNAME);
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
