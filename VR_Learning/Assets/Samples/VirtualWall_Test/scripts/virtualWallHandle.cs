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

public class virtualWallHandle : MonoBehaviour
{

	private static string LOG_TAG = "virtualWallHandle";
	private WaveVR_PermissionManager pmInstance = null;
	private WVR_ArenaVisible _areaVisible;
	//private WVR_ArenaVisible _areaVisibleinit;
	private int _virtualWallinit;
	private int _virtualWallstatus = 0;
	private string readValue = null;
	private string arenavisibleStatus = null;
	private const string CONTENT_PROVIDER_CLASSNAME = "com.htc.vr.unity.ContentProvider";
	private AndroidJavaObject contentProvider = null;

	// Use this for initialization
	void Start()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
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
				Log.e(LOG_TAG, "virtualWallForceOn() " + CONTENT_PROVIDER_CLASSNAME + " is null");
				return;
			}
			contentProvider = ajc.CallStatic<AndroidJavaObject>("getInstance");
			if (contentProvider != null)
			{
				readValue = readVirtualWall_value();
				int value = System.Convert.ToInt32(readValue);
				_virtualWallinit = value;
			}
			else
			{
				Log.e(LOG_TAG, "virtualWallstart() could NOT get instance of " + CONTENT_PROVIDER_CLASSNAME);
			}
			_areaVisible = Interop.WVR_GetArenaVisible();
			//_areaVisibleinit = _areaVisible;
			arenavisibleStatus = _areaVisible.ToString();
		}
		Log.d(LOG_TAG, "virtualWallinit " + CONTENT_PROVIDER_CLASSNAME + _virtualWallinit);
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

	public void virtualWallForceOn()
	{
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			_areaVisible = Interop.WVR_GetArenaVisible();
			Log.d(LOG_TAG, "virtualWall status change to  Force On : " + _areaVisible.ToString());
			if (_areaVisible.ToString() != "WVR_ArenaVisible_ForceOn")
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
					Log.e(LOG_TAG, "virtualWallForceOn() " + CONTENT_PROVIDER_CLASSNAME + " is null");
					return;
				}
				contentProvider = ajc.CallStatic<AndroidJavaObject>("getInstance");
				if (contentProvider != null)
				{
					readValue = readVirtualWall_value();
					int value = System.Convert.ToInt32(readValue);
					Log.d(LOG_TAG, "ForceOn , readVirtualWallValue() " + readValue);
					_virtualWallstatus = value;
					Log.d(LOG_TAG, "ForceOn , _virtualWallstatus " + _virtualWallstatus);
					byte[] virtualWallstatus_byte = System.BitConverter.GetBytes(_virtualWallstatus);
					BitArray virtualWallstatus_bit = new BitArray(virtualWallstatus_byte);

					int mask_and = 0xfb;
					int mask_or = 0x02;
					Log.d(LOG_TAG, "virtualWallForceOn , virtualWallstatus_bit 0 = " + virtualWallstatus_bit.Get(0));
					if (virtualWallstatus_bit.Get(0) == false)
					{
						_virtualWallstatus = 0x71;
						_virtualWallstatus = (_virtualWallstatus & mask_and) | mask_or;
					}
					else
					{
						_virtualWallstatus = (_virtualWallstatus & mask_and) | mask_or;
					}

					Log.d(LOG_TAG, "virtualWallForceOn() got instance of " + CONTENT_PROVIDER_CLASSNAME + ", change virtual wall status to " + _virtualWallstatus);
					contentProvider.Call("writeVirtualWallValue", _virtualWallstatus);
					_virtualWallstatus = 0;
				}
				else
				{
					Log.e(LOG_TAG, "virtualWallForceOn() could NOT get instance of " + CONTENT_PROVIDER_CLASSNAME);
				}
			}
			else
				Log.d(LOG_TAG, "virtualWall status has already Force On");
		}
	}

	public void virtualWallOff()
	{
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			_areaVisible = Interop.WVR_GetArenaVisible();
			Log.d(LOG_TAG, "virtualWall status change to  Force off : " + _areaVisible.ToString());
			if (_areaVisible.ToString() != "WVR_ArenaVisible_ForceOff")
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
					Log.e(LOG_TAG, "virtualWallOff() " + CONTENT_PROVIDER_CLASSNAME + " is null");
					return;
				}
				contentProvider = ajc.CallStatic<AndroidJavaObject>("getInstance");
				if (contentProvider != null)
				{
					readValue = readVirtualWall_value();
					int value = System.Convert.ToInt32(readValue);
					Log.d(LOG_TAG, "readVirtualWallValue() " + CONTENT_PROVIDER_CLASSNAME + readValue);
					_virtualWallstatus = value;
					Log.d(LOG_TAG, "virtualWallOff , _virtualWallstatus " + _virtualWallstatus);
					byte[] virtualWallstatus_byte = System.BitConverter.GetBytes(_virtualWallstatus);
					BitArray virtualWallstatus_bit = new BitArray(virtualWallstatus_byte);

					int mask_and = 0xfd;
					int mask_or = 0x04;
					Log.d(LOG_TAG, "virtualWallOff , virtualWallstatus_bit 0 = " + virtualWallstatus_bit.Get(0));
					if (virtualWallstatus_bit.Get(0) == false)
					{
						_virtualWallstatus = 0x71;
						_virtualWallstatus = (_virtualWallstatus & mask_and) | mask_or;
					}
					else
					{
						_virtualWallstatus = (_virtualWallstatus & mask_and) | mask_or;
					}
					Log.d(LOG_TAG, "virtualWallOff() got instance of " + CONTENT_PROVIDER_CLASSNAME + ", change virtual wall status to " + _virtualWallstatus);
					contentProvider.Call("writeVirtualWallValue", _virtualWallstatus);
					_virtualWallstatus = 0;
				}
				else
				{
					Log.e(LOG_TAG, "virtualWallOff() could NOT get instance of " + CONTENT_PROVIDER_CLASSNAME);
				}
			}
			else
				Log.d(LOG_TAG, "virtualWall status has already Force off");
		}
	}

	public void virtualWallAuto()
	{
#if UNITY_EDITOR
		if (Application.isEditor)
		{
			return;
		}
		else
#endif
		{
			_areaVisible = Interop.WVR_GetArenaVisible();
			Log.d(LOG_TAG, "virtualWall status change to  Auto : " + _areaVisible.ToString());
			if (_areaVisible.ToString() != "WVR_ArenaVisible_Auto")
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
					Log.e(LOG_TAG, "virtualWallAuto() " + CONTENT_PROVIDER_CLASSNAME + " is null");
					return;
				}
				contentProvider = ajc.CallStatic<AndroidJavaObject>("getInstance");
				if (contentProvider != null)
				{
					readValue = readVirtualWall_value();
					int value = System.Convert.ToInt32(readValue);
					Log.d(LOG_TAG, "readVirtualWallValue() " + CONTENT_PROVIDER_CLASSNAME + readValue);
					_virtualWallstatus = value;
					Log.d(LOG_TAG, "virtualWallAuto , _virtualWallstatus " + _virtualWallstatus);
					int mask = 0xf9;
					byte[] virtualWallstatus_byte = System.BitConverter.GetBytes(_virtualWallstatus);
					BitArray virtualWallstatus_bit = new BitArray(virtualWallstatus_byte);
					Log.d(LOG_TAG, "virtualWallAuto , virtualWallstatus_bit 0 = " + virtualWallstatus_bit.Get(0));
					if (virtualWallstatus_bit.Get(0) == false)
					{
						_virtualWallstatus = 0x71;
						_virtualWallstatus = (_virtualWallstatus & mask);
					}
					else
					{
						_virtualWallstatus = (_virtualWallstatus & mask);
					}
					Log.d(LOG_TAG, "virtualWallAuto() got instance of " + CONTENT_PROVIDER_CLASSNAME + ", change virtual wall status to " + _virtualWallstatus);
					contentProvider.Call("writeVirtualWallValue", _virtualWallstatus);

					_virtualWallstatus = 0;
				}
				else
				{
					Log.e(LOG_TAG, "virtualWallAuto() could NOT get instance of " + CONTENT_PROVIDER_CLASSNAME);
				}
			}
			else
				Log.d(LOG_TAG, "virtualWall status has already Auto");
		}
	}

	void OnDisable()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
		switch (arenavisibleStatus)
		{
			case "WVR_ArenaVisible_ForceOn":
				virtualWallForceOn();
				break;
			case "WVR_ArenaVisible_Auto":
				virtualWallAuto();
				break;
			case "WVR_ArenaVisible_ForceOff":
				virtualWallOff();
				break;
		}
	}

	void OnDestroy()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
			return;
#endif
		//virtualWallinit();
		switch (arenavisibleStatus)
		{
			case "WVR_ArenaVisible_ForceOn":
				virtualWallForceOn();
				break;
			case "WVR_ArenaVisible_Auto":
				virtualWallAuto();
				break;
			case "WVR_ArenaVisible_ForceOff":
				virtualWallOff();
				break;
		}
	}

	private void OnSceneUnloaded(Scene current)
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
			return;
#endif
		//Log.e(LOG_TAG, "OnSceneUnloaded() : " + arenavisibleStatus.ToString());
		switch (arenavisibleStatus)
		{
			case "WVR_ArenaVisible_ForceOn":
				virtualWallForceOn();
				break;
			case "WVR_ArenaVisible_Auto":
				virtualWallAuto();
				break;
			case "WVR_ArenaVisible_ForceOff":
				virtualWallOff();
				break;
		}
	}

	private void OnApplicationPause(bool pause)
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
			return;
#endif
		//Log.e(LOG_TAG, "OnApplicationPause() : " + arenavisibleStatus.ToString());
		switch (arenavisibleStatus)
		{
			case "WVR_ArenaVisible_ForceOn":
				virtualWallForceOn();
				break;
			case "WVR_ArenaVisible_Auto":
				virtualWallAuto();
				break;
			case "WVR_ArenaVisible_ForceOff":
				virtualWallOff();
				break;
		}
	}

	public string readVirtualWall_value()
	{
		return contentProvider.Call<string>("readVirtualWallValue");
	}

}