using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using WVR_Log;
using wvr;

public class IMETest : MonoBehaviour {
	public WVR_DeviceType Controller1Index = WVR_DeviceType.WVR_DeviceType_Controller_Right;
	public WVR_DeviceType Controller2Index = WVR_DeviceType.WVR_DeviceType_Controller_Left;
	public TextMesh Target;
	private static string LOG_TAG = "IMETest";
	private bool showKeyboard_ = false;
	private bool initialized = false;
	private WaveVR_IMEManager pmInstance = null;
	private string inputContent = null;
	private WaveVR_IMEManager.IMEParameter mParameter = null;

	public void inputDoneCallback(WaveVR_IMEManager.InputResult results)
	{
		Log.d(LOG_TAG, "inputDoneCallback("+ results.Id + ") :" + results.InputContent);
		inputContent = results.InputContent;
		showKeyboard_ = false;
		if( inputContent!=null && inputContent.Length > 0)
			UpdateTargetText(inputContent);
	}
	public void UpdateTargetText(string str)
	{
		//Log.d(LOG_TAG, "UpdateTargetText:" + str);
		if (Target != null && str != null) {
			
			Target.text = "Result: " + str;
		}
	}
	private void initParameter()
	{
		int MODE_FLAG_FIX_MOTION = 0x02;
		//int MODE_FLAG_AUTO_FIT_CAMERA = 0x04;
		int id = 0;
		int type = MODE_FLAG_FIX_MOTION;
		int mode = 2;

		string exist = "";
		int cursor = 0;
		int selectStart = 0;
		int selectEnd = 0;
		double[] pos = new double[] { 0 ,0 ,-1.05 };
		double[] rot = null; 
		int width = 800;
		int height = 800;
		int shadow = 100;
		string locale = "en_US";
		string title = "IMETest Title";
		int extraInt = 0;
		string extraString = "";
		int buttonId = 16;//System:0 , Menu:1 , Grip:2, Touchpad:16, Trigger:17
		mParameter = new WaveVR_IMEManager.IMEParameter(id, type, mode, exist, cursor, selectStart, selectEnd, pos,
							 rot, width, height, shadow, locale, title, extraInt, extraString, buttonId);
		//IMEParameter(int id, int type, int mode, double[] pos, double[] rot, int width, int height,
		//int shadow, string locale, string title, int buttonId)
	}
	private void InitializeKeyboards()
	{
		pmInstance = WaveVR_IMEManager.instance;
		initialized = pmInstance.isInitialized ();
		showKeyboard_ = false;
		initParameter();
		if (initialized)
			Log.d(LOG_TAG, "InitializeKeyboards: done");
		else
			Log.d(LOG_TAG, "InitializeKeyboards: failed");

	}
	private void hideKeyboard() {
		if (showKeyboard_ && initialized ) {
			Log.i(LOG_TAG, "hideKeyboard: done");
			pmInstance.hideKeyboard();
			showKeyboard_ = false;
		}
	}
	private void showKeyboard(WaveVR_IMEManager.IMEParameter parameter)
	{
		if (!showKeyboard_ && initialized)
		{
			Log.i(LOG_TAG, "showKeyboard: done");
			pmInstance.showKeyboard(parameter, inputDoneCallback);
			showKeyboard_ = true;
		}
	}
	public void showKeyboardEng()
	{
		Log.i(LOG_TAG, "showKeyboardEng()");
		if (!initialized)
			InitializeKeyboards();
		WaveVR_IMEManager.IMEParameter parameter = mParameter;
		showKeyboard(parameter);

	}
	public void showKeyboardPassword()
	{
		if (!initialized)
			InitializeKeyboards();
		WaveVR_IMEManager.IMEParameter parameter = mParameter;
		parameter.id = 1;
		showKeyboard(parameter);

	}
	// Use this for initialization
	void Start () {
		UpdateTargetText("Hello IME");
		InitializeKeyboards ();
	}

	// Update is called once per frame
	void Update () {
		bool rightTriggerDown = false, leftTriggerDown = false;
		//bool rightTriggerUp = false, leftTriggerUp = false;

		//rightTriggerUp |= WaveVR_Controller.Input (Controller1Index).GetPressUp (WVR_InputId.WVR_InputId_Alias1_Touchpad);
		#if UNITY_EDITOR
		/// We don't need to set #if UNITY_EDITOR condition.
		/// In editor mode, XXXTriggerDown value will be overwritten by WaveVR_Controller
		/// In WaveVR_Controller.Update, it checks whether editor mode or not.
		/// In editor mode -> call to emulator provider, otherwise call to SDK.
		rightTriggerDown	= Input.GetMouseButtonDown(1);  // mouse right key
		//rightTriggerUp	  = Input.GetMouseButtonUp(1);
		leftTriggerDown	 = Input.GetMouseButtonDown(0);  // mouse left key
		//leftTriggerUp	   = Input.GetMouseButtonUp(0);

		if (rightTriggerDown) {
			UpdateTargetText ("showKeyboardEng");
		} 
		if (leftTriggerDown) {
			UpdateTargetText ("hideKeyboard");
		}
		#endif

		rightTriggerDown |= WaveVR_Controller.Input (Controller1Index).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Touchpad);
		leftTriggerDown |= WaveVR_Controller.Input (Controller1Index).GetPressDown (WVR_InputId.WVR_InputId_Alias1_Menu);
		if (rightTriggerDown) {
			showKeyboardEng ();
		} 
		if (leftTriggerDown) {
			hideKeyboard ();
		}
	}
	void LateUpdate()
	{
		//UpdateTargetText(inputContent);
		//inputContent = "";
	}
}




