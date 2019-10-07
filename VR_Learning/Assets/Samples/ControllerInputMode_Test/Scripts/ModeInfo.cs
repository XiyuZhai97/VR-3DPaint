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

[RequireComponent(typeof(Text))]
public class ModeInfo : MonoBehaviour
{
	private const string LOG_TAG = "ModeInfo";
	private Text textField;

	private void PrintDebugLog(string msg)
	{
		Log.d(LOG_TAG, msg);
	}

	void Awake()
	{
		textField = GetComponent<Text>();
	}

	void LateUpdate()
	{
		string text = "";

		string gaze_type_text = "";
		string ctrlr_mode_text = "";

		if (WaveVR_InputModuleManager.Instance != null)
		{
			WVR_InteractionMode mode = WaveVR_InputModuleManager.Instance.GetInteractionMode();
			switch (mode)
			{
			case WVR_InteractionMode.WVR_InteractionMode_Controller:
				switch (WaveVR_InputModuleManager.Instance.GetRaycastMode ())
				{
				case WaveVR_ControllerInputModule.ERaycastMode.Beam:
					ctrlr_mode_text = "Controller(FlexibleBeam mode)";
					break;
				case WaveVR_ControllerInputModule.ERaycastMode.Fixed:
					ctrlr_mode_text = "Controller(FixedBeam mode)";
					break;
				case WaveVR_ControllerInputModule.ERaycastMode.Mouse:
					ctrlr_mode_text = "Controller(FixedMouse mode)";
					break;
				default:
					ctrlr_mode_text = "Controller(--)";
					break;
				}

				text += ctrlr_mode_text;
				break;
			case WVR_InteractionMode.WVR_InteractionMode_Gaze:
				switch (WaveVR_InputModuleManager.Instance.GetGazeTriggerType ())
				{
				case WVR_GazeTriggerType.WVR_GazeTriggerType_Button:
					gaze_type_text = "Gaze(Button Trigger)";
					break;
				case WVR_GazeTriggerType.WVR_GazeTriggerType_Timeout:
					gaze_type_text = "Gaze(Timeout Trigger)";
					break;
				case WVR_GazeTriggerType.WVR_GazeTriggerType_TimeoutButton:
					gaze_type_text = "Gaze(Button and Timeout Trigger)";
					break;
				default:
					gaze_type_text = "Gaze(--)";
					break;
				}
				text += gaze_type_text;
				break;
			default:
				break;
			}
		}
		textField.text = text;
	}
}