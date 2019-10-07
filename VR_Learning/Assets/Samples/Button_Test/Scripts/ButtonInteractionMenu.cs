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
using System.Collections.Generic;

[RequireComponent(typeof(Toggle))]
public class ButtonInteractionMenu : MonoBehaviour, IPointerDownHandler
{
	private const string LOG_TAG = "ButtonInteractionMenu";
	private void PrintDebugLog(string msg)
	{
		Log.d (LOG_TAG, this.DeviceType + " " + msg);
	}

	public WaveVR_Controller.EDeviceType DeviceType = WaveVR_Controller.EDeviceType.Dominant;
	private Toggle mToggle;
	private static List<WaveVR_ButtonList.EHmdButtons> headButtonList = new List<WaveVR_ButtonList.EHmdButtons>();
	private static List<WaveVR_ButtonList.EControllerButtons> dominantButtonList = new List<WaveVR_ButtonList.EControllerButtons>();
	private static List<WaveVR_ButtonList.EControllerButtons> noDomintButtonList = new List<WaveVR_ButtonList.EControllerButtons>();

	void Start()
	{
		if (!headButtonList.Contains (WaveVR_ButtonList.EHmdButtons.Enter))
			headButtonList.Add (WaveVR_ButtonList.EHmdButtons.Enter);
		// Always enable Touchpad
		if (!dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Touchpad))
			dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.Touchpad);
		if (!noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Touchpad))
			noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.Touchpad);

		mToggle = GetComponent<Toggle> ();
	}

	void Update()
	{
		if (WaveVR_ButtonList.Instance == null)
			return;

		switch (mToggle.name)
		{
		case "Toggle_Menu":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Menu);
			break;
		case "Toggle_Grip":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Grip);
			break;
		case "Toggle_DPadUp":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_DPad_Up);
			break;
		case "Toggle_DPadRight":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_DPad_Right);
			break;
		case "Toggle_DPadDown":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_DPad_Down);
			break;
		case "Toggle_DPadLeft":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_DPad_Left);
			break;
		case "Toggle_VolumeUp":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Volume_Up);
			break;
		case "Toggle_VolumeDown":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Volume_Down);
			break;
		case "Toggle_HmdBack":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Back);
			break;
		case "Toggle_HmdEnter":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Enter);
			break;
		case "Toggle_Trigger":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Trigger);
			break;
		case "Toggle_Thumbstick":
			mToggle.isOn = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, WVR_InputId.WVR_InputId_Alias1_Thumbstick);
			break;
		default:
			break;
		}
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		if (WaveVR_ButtonList.Instance == null)
			return;

		switch (this.DeviceType)
		{
		case WaveVR_Controller.EDeviceType.Head:
			switch (gameObject.name)
			{
			case "Toggle_HmdEnter":
				if (headButtonList.Contains (WaveVR_ButtonList.EHmdButtons.Enter))
					headButtonList.Remove (WaveVR_ButtonList.EHmdButtons.Enter);
				else
					headButtonList.Add (WaveVR_ButtonList.EHmdButtons.Enter);
				break;
			case "Toggle_VolumeUp":
				if (headButtonList.Contains (WaveVR_ButtonList.EHmdButtons.VolumeUp))
					headButtonList.Remove (WaveVR_ButtonList.EHmdButtons.VolumeUp);
				else
					headButtonList.Add (WaveVR_ButtonList.EHmdButtons.VolumeUp);
				break;
			case "Toggle_VolumeDown":
				if (headButtonList.Contains (WaveVR_ButtonList.EHmdButtons.VolumeDown))
					headButtonList.Remove (WaveVR_ButtonList.EHmdButtons.VolumeDown);
				else
					headButtonList.Add (WaveVR_ButtonList.EHmdButtons.VolumeDown);
				break;
			default:
				break;
			}

			foreach (WaveVR_ButtonList.EHmdButtons _btn in headButtonList)
				PrintDebugLog ("OnPointerDown() set up button list: " + _btn);

			WaveVR_ButtonList.Instance.SetupHmdButtonList (headButtonList);
			break;
		case WaveVR_Controller.EDeviceType.Dominant:
			switch (gameObject.name)
			{
			case "Toggle_Menu":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Menu))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.Menu);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.Menu);
				break;
			case "Toggle_Grip":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Grip))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.Grip);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.Grip);
				break;
			case "Toggle_DPadUp":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.DPadUp))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.DPadUp);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.DPadUp);
				break;
			case "Toggle_DPadRight":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.DPadRight))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.DPadRight);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.DPadRight);
				break;
			case "Toggle_DPadDown":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.DPadDown))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.DPadDown);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.DPadDown);
				break;
			case "Toggle_DPadLeft":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.DPadLeft))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.DPadLeft);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.DPadLeft);
				break;
			case "Toggle_VolumeUp":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.VolumeUp))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.VolumeUp);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.VolumeUp);
				break;
			case "Toggle_VolumeDown":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.VolumeDown))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.VolumeDown);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.VolumeDown);
				break;
			case "Toggle_Trigger":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Trigger))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.Trigger);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.Trigger);
				break;
			case "Toggle_Thumbstick":
				if (dominantButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Thumbstick))
					dominantButtonList.Remove (WaveVR_ButtonList.EControllerButtons.Thumbstick);
				else
					dominantButtonList.Add (WaveVR_ButtonList.EControllerButtons.Thumbstick);
				break;
			default:
				break;
			}

			foreach (WaveVR_ButtonList.EControllerButtons _btn in dominantButtonList)
				PrintDebugLog ("OnPointerDown() set up button list: " + _btn);

			WaveVR_ButtonList.Instance.SetupControllerButtonList (this.DeviceType, dominantButtonList);
			break;
		case WaveVR_Controller.EDeviceType.NonDominant:
			switch (gameObject.name)
			{
			case "Toggle_Menu":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Menu))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.Menu);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.Menu);
				break;
			case "Toggle_Grip":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Grip))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.Grip);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.Grip);
				break;
			case "Toggle_DPadUp":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.DPadUp))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.DPadUp);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.DPadUp);
				break;
			case "Toggle_DPadRight":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.DPadRight))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.DPadRight);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.DPadRight);
				break;
			case "Toggle_DPadDown":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.DPadDown))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.DPadDown);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.DPadDown);
				break;
			case "Toggle_DPadLeft":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.DPadLeft))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.DPadLeft);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.DPadLeft);
				break;
			case "Toggle_VolumeUp":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.VolumeUp))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.VolumeUp);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.VolumeUp);
				break;
			case "Toggle_VolumeDown":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.VolumeDown))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.VolumeDown);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.VolumeDown);
				break;
			case "Toggle_Trigger":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Trigger))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.Trigger);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.Trigger);
				break;
			case "Toggle_Thumbstick":
				if (noDomintButtonList.Contains (WaveVR_ButtonList.EControllerButtons.Thumbstick))
					noDomintButtonList.Remove (WaveVR_ButtonList.EControllerButtons.Thumbstick);
				else
					noDomintButtonList.Add (WaveVR_ButtonList.EControllerButtons.Thumbstick);
				break;
			default:
				break;
			}

			foreach (WaveVR_ButtonList.EControllerButtons _btn in noDomintButtonList)
				PrintDebugLog ("OnPointerDown() set up button list: " + _btn);

			WaveVR_ButtonList.Instance.SetupControllerButtonList (this.DeviceType, noDomintButtonList);
			break;
		}
	}
}