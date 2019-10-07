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
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using wvr;
using WVR_Log;

public class InAppRecenterHandle : MonoBehaviour
{


	private static string LOG_TAG = "InAppRecenterHandle";
	private bool bulletGeneratorState = false;
	private WaveVR_PermissionManager pmInstance = null;
	private WaveVR_Resource wvrRes = null;
	//bool inited = false;
	private static int systemCheckFailCount = 0;
	public static Stack previouslevel;
	private WVR_ArenaVisible _areaVisible;

	void OnEnable()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
		//WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.SWIPE_EVENT, onSwipeEvent);
		_areaVisible = Interop.WVR_GetArenaVisible();
		Log.w(LOG_TAG, "OnEnable, _areaVisible = " + _areaVisible);
	}

	void OnDisable()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
		//WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.SWIPE_EVENT, onSwipeEvent);
		Disabled();
		Log.w(LOG_TAG, "OnDisable, _areaVisible = " + _areaVisible);
	}

	private void onSwipeEvent(params object[] args)
	{
		var _event = (WVR_EventType)args[0];
		Log.d("CtrlrSwipeLR", "OnEvent() _event = " + _event);

		switch (_event)
		{
			case WVR_EventType.WVR_EventType_LeftToRightSwipe:
				//transform.Rotate(30, 0, 0);
				Interop.WVR_InAppRecenter(WVR_RecenterType.WVR_RecenterType_YawAndPosition);
				break;
			case WVR_EventType.WVR_EventType_RightToLeftSwipe:
				//transform.Rotate(-30, 0, 0);
				Interop.WVR_InAppRecenter(WVR_RecenterType.WVR_RecenterType_RotationAndPosition);
				break;
			case WVR_EventType.WVR_EventType_DownToUpSwipe:
				//transform.Rotate(30, 0, 0);
				Interop.WVR_InAppRecenter(WVR_RecenterType.WVR_RecenterType_Disabled);
				break;
			case WVR_EventType.WVR_EventType_UpToDownSwipe:
				//transform.Rotate(-30, 0, 0);
				Interop.WVR_InAppRecenter(WVR_RecenterType.WVR_RecenterType_YawOnly);
				break;
		}
	}

	public void YawAndPosition()
	{
		Interop.WVR_InAppRecenter(WVR_RecenterType.WVR_RecenterType_YawAndPosition);
	}

	public void RotationAndPosition()
	{
		Interop.WVR_InAppRecenter(WVR_RecenterType.WVR_RecenterType_RotationAndPosition);
	}

	public void Disabled()
	{
		Interop.WVR_InAppRecenter(WVR_RecenterType.WVR_RecenterType_Disabled);
	}

	public void YawOnly()
	{
		Interop.WVR_InAppRecenter(WVR_RecenterType.WVR_RecenterType_YawOnly);
	}

	// Use this for initialization
	void Start()
	{

#if UNITY_EDITOR
		if (Application.isEditor) return;
#endif
		if (!WaveVR.Instance.Initialized)
		{
			Log.w(LOG_TAG, "Fail to initialize!");
			return;
		}
		wvrRes = WaveVR_Resource.instance;

		if (wvrRes == null)
		{
			Log.w(LOG_TAG, "Failed to initial WaveVR Resource instance!");
		}
		else
		{
			string lang = wvrRes.getSystemLanguage();
			string country = wvrRes.getSystemCountry();
			Log.d(LOG_TAG, "system default language is " + lang);
			Log.d(LOG_TAG, "system default country is " + country);

		}

		pmInstance = WaveVR_PermissionManager.instance;
		if (pmInstance != null)
		{
			StartCoroutine(checkPackageManagerStatus());
		}
		//Interop.WVR_SetArenaVisible(WVR_ArenaVisible.WVR_ArenaVisible_Auto);
	}

	IEnumerator checkPackageManagerStatus()
	{
		if (systemCheckFailCount < 10)
		{
			if (!pmInstance.isInitialized())
			{
				systemCheckFailCount++;
				yield return new WaitForSeconds(0.1f);
			}
			else
			{
				//inited = true;
				yield break;
			}
		}

		//inited = false;
		yield break;
	}

	public static void requestDoneCallback(List<WaveVR_PermissionManager.RequestResult> results)
	{
		Log.d(LOG_TAG, "requestDoneCallback, count = " + results.Count);
	}

	public void toggleBulletGenerator()
	{
		var roots = SceneManager.GetActiveScene().GetRootGameObjects();
		foreach (var obj in roots)
		{
			if (obj.name == "BodyByDoF")
			{
				bulletGeneratorState = !bulletGeneratorState;
				obj.GetComponentInChildren<BulletGenerator>().enabled = bulletGeneratorState;
				break;
			}
		}
	}

	private void disableClicking()
	{
		GameObject btn = GameObject.Find("BtnUtra");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = false;
		}
		btn = GameObject.Find("BtnHigh");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = false;
		}
		btn = GameObject.Find("BtnMedium");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = false;
		}
		btn = GameObject.Find("BtnLow");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = false;
		}
	}

	private void enableClicking()
	{
		GameObject btn = GameObject.Find("BtnUtra");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = true;
		}
		btn = GameObject.Find("BtnHigh");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = true;
		}
		btn = GameObject.Find("BtnMedium");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = true;
		}
		btn = GameObject.Find("BtnLow");
		if (btn != null)
		{
			btn.GetComponent<Button>().interactable = true;
		}
	}

	private void hidePanel2()
	{
		GameObject obj = GameObject.Find("Panel2");
		obj.SetActive(false);
	}

	public void setQualityLevel(int level)
	{
		disableClicking();
		WaveVR.Instance.SetQualityLevel(level);
		enableClicking();
		hidePanel2();
	}

	public void quitGame()
	{
		Application.Quit();
	}

}
