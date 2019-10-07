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
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using WVR_Log;

public class ButtonEventHandler: MonoBehaviour,
	IPointerClickHandler
{
	private const string LOG_TAG = "ButtonEventHandler";
	private GameObject eventSystem = null;
	private WaveVR_ControllerInputModule.ERaycastMode raycastMode = WaveVR_ControllerInputModule.ERaycastMode.Beam;
	private float length = 0.8f;

	void Start()
	{
		if (gameObject.name.Equals ("1m"))
			this.length = 1.0f;
		if (gameObject.name.Equals ("2m"))
			this.length = 2.0f;
		if (gameObject.name.Equals ("3m"))
			this.length = 3.0f;
		Log.d (LOG_TAG, "Start() " + this.length + " meter.");
	}

	#region override event handling function
	public void OnPointerClick (PointerEventData eventData)
	{
		if (EventSystem.current == null)
		{
			EventSystem _es = FindObjectOfType<EventSystem> ();
			if (_es != null)
				this.eventSystem = _es.gameObject;
		} else
		{
			this.eventSystem = EventSystem.current.gameObject;
		}

		WaveVR_ControllerInputModule _cim = null;
		if (this.eventSystem != null)
		{
			_cim = this.eventSystem.GetComponent<WaveVR_ControllerInputModule> ();
			if (_cim != null)
				this.raycastMode = _cim.RaycastMode;
		}

		GameObject _go = eventData.enterEventCamera.gameObject;
		if (_go != null)
		{
			Log.d (LOG_TAG, "OnPointerClick() " + _go.name);
			switch (this.raycastMode)
			{
			case WaveVR_ControllerInputModule.ERaycastMode.Mouse:
				WaveVR_PointerCameraTracker _pct = _go.GetComponent<WaveVR_PointerCameraTracker> ();
				if (_pct != null && _cim != null)
				{
					Log.d (LOG_TAG, "OnPointerClick() set beam length of " + _pct.type + " to " + this.length);
					_cim.ChangeBeamLength (_pct.type, this.length);
				}
				break;
			case WaveVR_ControllerInputModule.ERaycastMode.Fixed:
				WaveVR_ControllerPointer _cp = _go.GetComponent<WaveVR_ControllerPointer> ();
				if (_cp != null && _cim != null)
				{
					Log.d (LOG_TAG, "OnPointerClick() set beam length of " + _cp.device + " to " + this.length);
					_cim.ChangeBeamLength (_cp.device, this.length);
				}
				break;
			// Do nothing in beam mode
			case WaveVR_ControllerInputModule.ERaycastMode.Beam:
			default:
				break;
			}
		}
	}
	#endregion
}