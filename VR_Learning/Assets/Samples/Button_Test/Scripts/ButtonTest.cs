using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonTest : MonoBehaviour {
	private const string LOG_TAG = "ButtonTest";
	private void PrintDebugLog(string msg)
	{
		Log.d (LOG_TAG, this.DeviceType + " " + msg);
	}

	private WVR_InputId[] pressIds = new WVR_InputId[] {
		WVR_InputId.WVR_InputId_Alias1_System,
		WVR_InputId.WVR_InputId_Alias1_Menu,
		WVR_InputId.WVR_InputId_Alias1_Grip,
		WVR_InputId.WVR_InputId_Alias1_Volume_Up,
		WVR_InputId.WVR_InputId_Alias1_Volume_Down,
		WVR_InputId.WVR_InputId_Alias1_Digital_Trigger,
		WVR_InputId.WVR_InputId_Alias1_Back,
		WVR_InputId.WVR_InputId_Alias1_Enter,
		WVR_InputId.WVR_InputId_Alias1_Touchpad,
		WVR_InputId.WVR_InputId_Alias1_Trigger,
		WVR_InputId.WVR_InputId_Alias1_Thumbstick
	};

	private WVR_InputId[] touchIds = new WVR_InputId[] {
		WVR_InputId.WVR_InputId_Alias1_Touchpad,
		WVR_InputId.WVR_InputId_Alias1_Trigger,
		WVR_InputId.WVR_InputId_Alias1_Thumbstick
	};

	private WVR_InputId[] dpadIds = new WVR_InputId[] {
		WVR_InputId.WVR_InputId_Alias1_DPad_Left,
		WVR_InputId.WVR_InputId_Alias1_DPad_Up,
		WVR_InputId.WVR_InputId_Alias1_DPad_Right,
		WVR_InputId.WVR_InputId_Alias1_DPad_Down,
	};

	public WaveVR_Controller.EDeviceType DeviceType = WaveVR_Controller.EDeviceType.Dominant;
	public GameObject Button_Touch = null;
	private Text touch_text = null;
	public GameObject Button_Axis = null;
	private Text axis_text = null;
	private Vector2 v2axis = Vector2.zero;
	public GameObject Button_Press = null;
	private Text press_text = null;
	public GameObject DPad_Button_Touch = null;
	private Text dpad_touch_text = null;
	public GameObject DPad_Button_Axis = null;
	private Text dpad_axis_text = null;
	public GameObject DPad_Button_Press = null;
	private Text dpad_press_text = null;

	public void ExitGame()
	{
		Application.Quit();
	}

	public void BackToUpLayer()
	{
		SceneManager.LoadScene (0);
	}

	// Use this for initialization
	void Start ()
	{
		if (this.Button_Touch != null)
		{
			this.touch_text = this.Button_Touch.GetComponent<Text> ();
			PrintDebugLog ("Start() Get Text of " + this.Button_Touch.name);
		}
		if (this.Button_Axis != null)
		{
			this.axis_text = this.Button_Axis.GetComponent<Text> ();
			PrintDebugLog ("Start() Get Text of " + this.Button_Axis.name);
		}
		if (this.Button_Press != null)
		{
			this.press_text = this.Button_Press.GetComponent<Text> ();
			PrintDebugLog ("Start() Get Text of " + this.Button_Press.name);
		}
		if (this.DPad_Button_Touch != null)
		{
			this.dpad_touch_text = this.DPad_Button_Touch.GetComponent<Text> ();
			PrintDebugLog ("Start() Get Text of " + this.DPad_Button_Touch);
		}
		if (this.DPad_Button_Axis != null)
		{
			this.dpad_axis_text = this.DPad_Button_Axis.GetComponent<Text> ();
			PrintDebugLog ("Start() Get Text of " + this.DPad_Button_Axis);
		}
		if (this.DPad_Button_Press != null)
		{
			this.dpad_press_text = this.DPad_Button_Press.GetComponent<Text> ();
			PrintDebugLog ("Start() Get Text of " + this.DPad_Button_Press);
		}
	}
	
	// Update is called once per frame
	private uint printCount = 0;
	private bool printIntervalLog = false;
	void Update ()
	{
		updateTouchText ();
		updateAxisText ();
		updatePressText ();
		updateDPadTouchText ();
		updateDPadAxisText ();
		updateDpadPressText ();

		if (!Application.isEditor)
		{
			printCount++;
			printCount %= 60;  // print log every 60 frames.
			printIntervalLog = (this.printCount == 0) ? true : false;
			if (printIntervalLog)
			{
				foreach (WVR_InputId _id in pressIds)
				{
					bool _available = WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, _id);
					if (_available)
						PrintDebugLog ("Update() " + this.DeviceType + " " + _id + " is available.");
				}
			}
		}
	}

	private void updateTouchText()
	{
		if (this.touch_text == null)
			return;

		WVR_DeviceType _type = WaveVR_Controller.Input (this.DeviceType).DeviceType;
		foreach (WVR_InputId _touchId in this.touchIds)
		{
			if (WaveVR_ButtonList.Instance != null && WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, _touchId))
			{
				if (WaveVR_Controller.Input (this.DeviceType).GetTouchDown (_touchId))
				{
					this.touch_text.text = _touchId.ToString ();
					PrintDebugLog ("updateTouchText() " + _touchId + " is touched down.");

					WVR_InputId _btn = _touchId;
					bool _result = WaveVR_ButtonList.Instance.GetInputMappingPair (this.DeviceType, ref _btn);
					if (_result)
						PrintDebugLog ("updateTouchText() " + _touchId + " is mapping from " + _type + " button " + _btn);
				}
				if (WaveVR_Controller.Input (this.DeviceType).GetTouchUp (_touchId))
				{
					this.touch_text.text = "";
					PrintDebugLog ("updateTouchText() " + _touchId + " is touched up.");
				}
			}
		}
	}

	private void updateAxisText()
	{
		if (this.axis_text == null)
			return;

		bool _touched = false;
		foreach (WVR_InputId _touchId in this.touchIds)
		{
			if (WaveVR_ButtonList.Instance != null && WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, _touchId))
			{
				if (WaveVR_Controller.Input (this.DeviceType).GetTouch (_touchId))
				{
					_touched = true;
					this.v2axis = WaveVR_Controller.Input (this.DeviceType).GetAxis (_touchId);
					this.axis_text.text = this.v2axis.x.ToString () + ", " + this.v2axis.y.ToString ();
					PrintDebugLog ("updateAxisText() " + _touchId + " axis: (" + this.v2axis.x + ", " + this.v2axis.y + ")");
				}
			}
		}
		if (!_touched)
			this.axis_text.text = "";
	}

	private void updatePressText()
	{
		if (this.press_text == null)
			return;

		WVR_DeviceType _type = WaveVR_Controller.Input (this.DeviceType).DeviceType;
		foreach (WVR_InputId _pressId in this.pressIds)
		{
			if (WaveVR_ButtonList.Instance != null && WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, _pressId))
			{
				if (WaveVR_Controller.Input (this.DeviceType).GetPressDown (_pressId))
				{
					this.press_text.text = _pressId.ToString ();
					PrintDebugLog ("updatePressText() " + _pressId + " is pressed down.");

					WVR_InputId _btn = _pressId;
					bool _result = WaveVR_ButtonList.Instance.GetInputMappingPair (this.DeviceType, ref _btn);
					if (_result)
						PrintDebugLog ("updatePressText() " + _pressId + " is mapping from " + _type + " button " + _btn);
				}

				if (WaveVR_Controller.Input (this.DeviceType).GetPressUp (_pressId))
				{
					this.press_text.text = "";
					PrintDebugLog ("updatePressText() " + _pressId + " is pressed up.");
					if (_pressId == WVR_InputId.WVR_InputId_Alias1_Trigger)
						WaveVR_Controller.Input (this.DeviceType).TriggerHapticPulse ();
				}
			}
		}
	}

	private void updateDPadTouchText()
	{
		if (this.dpad_touch_text == null)
			return;

		WVR_DeviceType _type = WaveVR_Controller.Input (this.DeviceType).DeviceType;
		foreach (WVR_InputId _dpadId in this.dpadIds)
		{
			if (WaveVR_ButtonList.Instance != null && WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, _dpadId))
			{
				if (WaveVR_Controller.Input (this.DeviceType).GetTouchDown (_dpadId))
				{
					this.dpad_touch_text.text = _dpadId.ToString ();
					PrintDebugLog ("updateDPadTouchText() " + _dpadId + " is touched down.");

					WVR_InputId _btn = _dpadId;
					bool _result = WaveVR_ButtonList.Instance.GetInputMappingPair (this.DeviceType, ref _btn);
					if (_result)
						PrintDebugLog ("updateDPadTouchText() " + _dpadId + " is mapping from " + _type + " button " + _btn);
				}
				if (WaveVR_Controller.Input (this.DeviceType).GetTouchUp (_dpadId))
				{
					this.dpad_touch_text.text = "";
					PrintDebugLog ("updateDPadTouchText() " + _dpadId + " is touched up.");
				}
			}
		}
	}

	private void updateDPadAxisText()
	{
		if (this.dpad_axis_text == null)
			return;

		bool _touched = false;
		foreach (WVR_InputId _dpadId in this.dpadIds)
		{
			if (WaveVR_ButtonList.Instance != null && WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, _dpadId))
			{
				if (WaveVR_Controller.Input (this.DeviceType).GetTouch (_dpadId))
				{
					_touched = true;
					this.v2axis = WaveVR_Controller.Input (this.DeviceType).GetAxis (_dpadId);
					this.dpad_axis_text.text = this.v2axis.x.ToString () + ", " + this.v2axis.y.ToString ();
					PrintDebugLog ("updateDPadAxisText() " + _dpadId + " axis: (" + this.v2axis.x + ", " + this.v2axis.y + ")");
				}
			}
		}
		if (!_touched)
			this.dpad_axis_text.text = "";
	}

	private void updateDpadPressText()
	{
		if (this.dpad_press_text == null)
			return;

		WVR_DeviceType _type = WaveVR_Controller.Input (this.DeviceType).DeviceType;
		foreach (WVR_InputId _dpadId in this.dpadIds)
		{
			if (WaveVR_ButtonList.Instance != null && WaveVR_ButtonList.Instance.IsButtonAvailable (this.DeviceType, _dpadId))
			{
				if (WaveVR_Controller.Input (this.DeviceType).GetPressDown (_dpadId))
				{
					this.dpad_press_text.text = _dpadId.ToString ();
					PrintDebugLog ("updateDpadPressText() " + _dpadId + " is pressed down.");

					WVR_InputId _btn = _dpadId;
					bool _result = WaveVR_ButtonList.Instance.GetInputMappingPair (this.DeviceType, ref _btn);
					if (_result)
						PrintDebugLog ("updateDpadPressText() " + _dpadId + " is mapping from " + _type + " button " + _btn);
				}

				if (WaveVR_Controller.Input (this.DeviceType).GetPressUp (_dpadId))
				{
					this.dpad_press_text.text = "";
					PrintDebugLog ("updateDpadPressText() " + _dpadId + " is pressed up.");
				}
			}
		}
	}
}
