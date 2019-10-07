using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;

public class CtrlrSwipeLR : MonoBehaviour
{
	void OnEvent(params object[] args)
	{
		var _event = (WVR_EventType)args[0];
		Log.d("CtrlrSwipeLR", "OnEvent() _event = " + _event);

		switch (_event)
		{
			case WVR_EventType.WVR_EventType_LeftToRightSwipe:
				transform.Rotate(30, 0, 0);
				break;
			case WVR_EventType.WVR_EventType_RightToLeftSwipe:
				transform.Rotate(-30, 0, 0);
				break;
		}
	}

	void OnEnable()
	{
		WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.SWIPE_EVENT, OnEvent);
	}

	void OnDisable()
	{
		WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.SWIPE_EVENT, OnEvent);
	}
}

