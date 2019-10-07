using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using wvr;
using WVR_Log;
using System;
using System.Threading;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;

public class TrackingAccuracyTest : MonoBehaviour {

	private static string LOG_TAG = "TrackingAccuracyTest";

	private string fileName = "trackingAccuracyTest";
	private string fullFileName;

	public GameObject mCtrlPanel;
	public GameObject mConfigPanel;
	public GameObject mPoseInputPanel;
	public GameObject mHelpPanel;

	public GameObject head;
	public GameObject scene;

	public Text mHmdInfo;
	public Text mControllerInfo;

	private Button mStart;
	private Button mStop;
	private Button mQuit;
	private ControlState mCtrlState = ControlState.None;
	private enum ControlState
	{
		None = 0,
		Start,
		Stop,
		Quit
	}

	private enum DeviceType
	{
		DeviceType_HMD = 0,
		DeviceType_Controller,
		DeviceType_Cnt
	}
	private Button[] mDevices = new Button[(int)DeviceType.DeviceType_Cnt];
	private int mSelectDevice = (int)DeviceType.DeviceType_HMD;

	private enum ActionType
	{
		ActionType_1 = 0,
		ActionType_2,
		ActionType_3,
		ActionType_4,
		ActionType_Cnt
	}
	private Button[] mActions = new Button[(int)ActionType.ActionType_Cnt];
	private int mSelectAction = (int)ActionType.ActionType_1;// with select color CFCF27, unselect color D7F3F3
	private Color selColor = new Color(0xCF / 255.0f, 0xCF / 255.0f, 0x27 / 255.0f);
	private Color unselColor = new Color(0xD7 / 255.0f, 0xF3 / 255.0f, 0xF3 / 255.0f);

	private Button[] mEulers = new Button[3];
	private int[] mConfEuler = new int[3] { 0, 0, 0 };
	private Button[] mPos = new Button[3];
	private int[] mConfPos = new int[3] { 0, 0, 0 };
	private int mCurSelInput = 0;

	private Thread recordThread;
	private bool bStop = true;
	private const int RecordSample = 100; //Hz
	private const int Action3SampeDuration = 90; //second
	private StringBuilder records = new StringBuilder();

	private Action autoStopAction;
	private WVR_Arena_t arena;

	private WaveVR_IMEManagerWrapper mIMEWrapper;

	private string aberration;

	private const int mInputBtnCnt = 24; 
	private int mPosStep = 15; //cm.
	private int mEulerStep = 15; //degree.

	private void PrintDebugLog(string msg)
	{
		Log.d(LOG_TAG, msg);
	}

	private void PrintInfoLog(string msg)
	{
		Log.i(LOG_TAG, msg);
	}

	private void PrintWarningLog(string msg)
	{
		Log.w(LOG_TAG, msg);
	}

	// Use this for initialization
	void Start () {
		RegisterConfigCallback();
		RegisterInputPanelCallback();
		RegisterCtrlPanelCallback();
		CreateFile();
		arena = Interop.WVR_GetArena();
		string _content =
			"arena shape: " + arena.shape +
			"\narena length: " + arena.area.rectangle.length +
			"\narena width: " + arena.area.rectangle.width +
			"\narena diameter: " + arena.area.round.diameter;
		PrintInfoLog(_content);

		mIMEWrapper = WaveVR_IMEManagerWrapper.GetInstance();
		if (mIMEWrapper != null)
		{
			mIMEWrapper.SetCallback(InputDoneCallback);
		}
	}

	private void InputDoneCallback(WaveVR_IMEManagerWrapper.InputResult results)
	{
		PrintInfoLog("inputDoneCallback:" + results.GetContent());
		string content = results.GetContent();
		if (content == null)
		{
			return;
		}
		int data = int.Parse(content);
		switch (mCurSelInput)
		{
			case 0:// "Yaw":
			case 1:// "Pitch":
			case 2:// "Roll":
				if (data > 360 || data < -360)
				{
					PrintWarningLog("invalid degree.");
					return;
				}
				mConfEuler[mCurSelInput] = data;
				Text txt = mEulers[mCurSelInput].GetComponentInChildren<Text>();
				string name = txt.text;
				string[] strings = name.Split(new char[2] { '(', ')'});
				if (strings.Length == 3)
				{
					txt.text = strings[0] + "(" + mConfEuler[mCurSelInput].ToString() + ")" + strings[2];
				}
				break;
			case 3:// "XP":
			case 4:// "YP":
			case 5:// "ZP":
				mConfPos[mCurSelInput - 3] = data;
				txt = mPos[mCurSelInput - 3].GetComponentInChildren<Text>();
				name = txt.text;
				strings = name.Split(new char[2] { '(', ')' });
				if (strings.Length == 3)
				{
					txt.text = strings[0] + "(" + mConfPos[mCurSelInput - 3].ToString() + ")" + strings[2];
				}
				break;
		}

		//update Action1 & Action2
		int action = (int)ActionType.ActionType_2;
		if (mCurSelInput < 3)
		{
			action = (int)ActionType.ActionType_1;
		}
		Text text = mActions[action].GetComponentInChildren<Text>();
		string str = text.text;
		if ((int)ActionType.ActionType_1 == action)
		{
			string[] strings = str.Split(new char[2] { '(', ')' });
			str = strings[0] + "(" + "Yaw=" + mConfEuler[0] + " Pitch=" + mConfEuler[1] + " Roll=" + mConfEuler[2] + ")";
		}
		if ((int)ActionType.ActionType_2 == action)
		{
			string[] strings = str.Split(new char[2] { '(', ')' });
			str = strings[0] + "(" + "X=" + mConfPos[0] + " Y=" + mConfPos[1] + " Z=" + mConfPos[2] + ")";
		}

		text.text = str;

		// Note: directly update input field text will exception
		// use LastUpdate to update Input field text
	}

	private void CreateFile()
	{
		fullFileName = Application.persistentDataPath + "/" + fileName + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
		PrintInfoLog("CreateFile path:" + fullFileName);

		if (File.Exists(fullFileName))
		{
			PrintInfoLog("the out file will be overwrite.");
		}
		else
		{
			File.Create(fullFileName);
		}
	}

	// Update is called once per frame
	void Update () {
		if (autoStopAction != null)
		{
			autoStopAction.Invoke();
			autoStopAction = null;
		}

		WaveVR_Utils.RigidTransform transform = WaveVR_Utils.RigidTransform.identity;
		if (0 == mSelectDevice)
		{
			transform = WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_HMD).transform;
			mHmdInfo.text = "Euler:(" + transform.rot.eulerAngles.y.ToString("0.0") + ", " 
									  + transform.rot.eulerAngles.x.ToString("0.0") + ", "
									  + transform.rot.eulerAngles.z.ToString("0.0") + ")\n"
						 + "Pos:(" + transform.pos.x.ToString("0.000") + ", "
										+ transform.pos.y.ToString("0.000") + ", "
										+ transform.pos.z.ToString("0.000") + ")\n"
										+ aberration;
		}
		else
		{
			transform = WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).transform;
			mControllerInfo.text = "Euler:(" + transform.rot.eulerAngles.y.ToString("0.0") + ", "
									  + transform.rot.eulerAngles.x.ToString("0.0") + ", "
									  + transform.rot.eulerAngles.z.ToString("0.0") + ")\n"
						 + "Position:(" + transform.pos.x.ToString("0.000") + ", "
										+ transform.pos.y.ToString("0.000") + ", "
										+ transform.pos.z.ToString("0.000") + ")\n"
										+ aberration;
		}
	}

	private void DeviceChanged(int select)
	{
		PrintInfoLog("DeviceChanged value=" + select);
		if (mSelectDevice == select)
		{
			return;
		}

		if (WaveVR_InputModuleManager.Instance != null && WaveVR_InputModuleManager.Instance.OverrideSystemSettings)
		{
			if (WaveVR_InputModuleManager.Instance.Controller != null && WaveVR_InputModuleManager.Instance.Gaze != null)
			{
				if (WaveVR_InputModuleManager.Instance.CustomInputModule == WaveVR_EInputModule.Gaze)
				{
					WaveVR_InputModuleManager.Instance.CustomInputModule = WaveVR_EInputModule.Controller;
					PrintInfoLog("Set EnableController to true and EnableGaze to false.");
					scene.transform.parent = head.transform;
				}
				else
				{
					WaveVR_InputModuleManager.Instance.CustomInputModule = WaveVR_EInputModule.Gaze;
					PrintInfoLog("Set EnableController to false and EnableGaze to true.");
					scene.transform.parent = null;
				}
				//Interop.WVR_InAppRecenter(WVR_RecenterType.WVR_RecenterType_YawAndPosition);
			}
		}

		selectSelectable(mDevices[mSelectDevice], false);

		selectSelectable(mDevices[select], true);

		mSelectDevice = select;
	}

	private void RegisterConfigCallback()
	{
		foreach (Button child in mConfigPanel.GetComponentsInChildren<Button>())
		{
			PrintInfoLog("get button of configPanel " + child.name);
			child.onClick.AddListener(delegate { ConfigChanged(child); });
			switch (child.name)
			{
				case "HMD":
					mDevices[0] = child;
					break;
				case "Controller":
					mDevices[1] = child;
					break;
				case "Action1":
					mActions[0] = child;
					break;
				case "Action2":
					mActions[1] = child;
					break;
				case "Action3":
					mActions[2] = child;
					break;
				case "Action4":
					mActions[3] = child;
					break;
				case "Yaw":
					mEulers[0] = child;
					break;
				case "Pitch":
					mEulers[1] = child;
					break;
				case "Roll":
					mEulers[2] = child;
					break;
				case "XP":
					mPos[0] = child;
					break;
				case "YP":
					mPos[1] = child;
					break;
				case "ZP":
					mPos[2] = child;
					break;
			}
		}
	}

	private void selectSelectable(Button btn, bool sel)
	{
		ColorBlock cb = new ColorBlock();
		cb.normalColor = sel? selColor : unselColor;
		cb.colorMultiplier = btn.colors.colorMultiplier;
		cb.disabledColor = btn.colors.disabledColor;
		cb.fadeDuration = btn.colors.fadeDuration;
		cb.highlightedColor = btn.colors.highlightedColor;
		cb.pressedColor = btn.colors.pressedColor;

		btn.colors = cb;
	}

	private void setupActionsState(int select)
	{
		if (mSelectAction == select)
		{
			return;
		}

		selectSelectable(mActions[mSelectAction], false);

		selectSelectable(mActions[select], true);

		mSelectAction = select;
	}

	private void setupInputState(int select)
	{
		if (mCurSelInput == select)
		{
			return;
		}

		if (mCurSelInput < 3)
		{
			selectSelectable(mEulers[mCurSelInput], false);
		}
		else
		{
			selectSelectable(mPos[mCurSelInput - 3], false);
		}
		

		if (select < 3)
		{
			selectSelectable(mEulers[select], true);
		}
		else
		{
			selectSelectable(mPos[select - 3], true);
		}

		

		mCurSelInput = select;
	}

	private void ConfigChanged(Button child)
	{
		PrintInfoLog("ConfigChanged " + child.name);
		switch (child.name)
		{
			case "HMD":
				PrintInfoLog("switch to test Controller device.");
				child.interactable = false;
				mDevices[(int)DeviceType.DeviceType_Controller].interactable = true;
				DeviceChanged((int)DeviceType.DeviceType_Controller);
				break;
			case "Controller":
				PrintInfoLog("switch to test HMD device.");
				child.interactable = false;
				mDevices[(int)DeviceType.DeviceType_HMD].interactable = true;
				DeviceChanged((int)DeviceType.DeviceType_HMD);
				break;
			case "Action1":
				PrintInfoLog("Action1 test.");
				setupActionsState((int)ActionType.ActionType_1);
				break;
			case "Action2":
				PrintInfoLog("Action2 test.");
				setupActionsState((int)ActionType.ActionType_2);
				break;
			case "Action3":
				PrintInfoLog("Action3 test.");
				setupActionsState((int)ActionType.ActionType_3);
				break;
			case "Action4":
				PrintInfoLog("Action4 test.");
				setupActionsState((int)ActionType.ActionType_4);
				break;
			case "Yaw":
				setupInputState(0);
				if (mIMEWrapper != null)
				{
					ShowKeyboardButton(child);
				}
				else
				{
					showInputPanel(false);
				}
				break;
			case "Pitch":
				setupInputState(1);
				if (mIMEWrapper != null)
				{
					ShowKeyboardButton(child);
				}
				else
				{
					showInputPanel(false);
				}
					
				break;
			case "Roll":
				setupInputState(2);
				if (mIMEWrapper != null)
				{
					ShowKeyboardButton(child);
				}
				else
				{
					showInputPanel(false);
				}
				break;
			case "XP":
				setupInputState(3);
				if (mIMEWrapper != null)
				{
					ShowKeyboardButton(child);
				}
				else
				{
					showInputPanel(true);
				}
				break;
			case "YP":
				setupInputState(4);
				if (mIMEWrapper != null)
				{
					ShowKeyboardButton(child);
				}
				else
				{
					showInputPanel(true);
				}
				break;
			case "ZP":
				setupInputState(5);
				if (mIMEWrapper != null)
				{
					ShowKeyboardButton(child);
				}
				else
				{
					showInputPanel(true);
				}
				break;
		}
	}

	private void RegisterInputPanelCallback()
	{
		foreach (Button child in mPoseInputPanel.GetComponentsInChildren<Button>())
		{
			PrintInfoLog("get button of mPoseInputPanel " + child.name);
			string name = child.name;
			string[] strings = name.Split(new char[2] { '(', ')' });
			foreach(string str in strings)
			{
				PrintInfoLog("split name " + str);
			}

			int idx = 0;
			if (strings.Length == 3)
			{
				idx = int.Parse(strings[1]);
			}
			child.onClick.AddListener(delegate { InputPanelChanged(idx); });
		}
	}

	private void changeButtonText(Button btn, string text)
	{
		Text txt = btn.GetComponentInChildren<Text>();
		txt.text = text;
	}

	private void ShowKeyboardButton(Button btn)
	{
		//Log.i(LOG_TAG, "ShowKeyboardButton");
		//Re-init all parameters
		mIMEWrapper.InitParameter();
		//if (target != null)
		{
			//Log.i(LOG_TAG, "ShowKeyboardButton text = " + btn.GetComponentInChildren<Text>().text);
			string txt = btn.GetComponentInChildren<Text>().text;
			string[] strings = txt.Split(new char[2] { '(', ')' });
			int data = 0;
			if (strings.Length == 3)
			{
				data = int.Parse(strings[1]);
			}
			mIMEWrapper.SetText(data.ToString());
		}

		if (mCurSelInput < 3)
		{
			mIMEWrapper.SetTitle("Input euler...");
		} else
		{
			mIMEWrapper.SetTitle("Input position...");
		}
		mIMEWrapper.SetLocale(WaveVR_IMEManagerWrapper.Locale.en_US);
		mIMEWrapper.SetAction(WaveVR_IMEManagerWrapper.Action.Send);
		mIMEWrapper.Show();
	}

	private void showInputPanel(bool bEulerInput)
	{
		foreach (Button child in mPoseInputPanel.GetComponentsInChildren<Button>())
		{
			PrintInfoLog("get button of mPoseInputPanel " + child.name);
			string name = child.name;
			string[] strings = name.Split(new char[2] { '(', ')' });
			foreach (string str in strings)
			{
				PrintInfoLog("split name " + str);
			}

			int idx = 0;
			if (strings.Length == 3)
			{
				idx = int.Parse(strings[1]);
			}
			Text txt = child.GetComponentInChildren<Text>();
			if (!bEulerInput)
			{
				txt.text = (idx * mEulerStep).ToString();

			}
			else
			{
				txt.text = (idx * mPosStep - mPosStep*mInputBtnCnt/2).ToString();
			}
		}
		mPoseInputPanel.SetActive(true);
	}

	private void InputPanelChanged(int idx)
	{
		PrintInfoLog("InputPanelChanged " + idx);
		switch (mCurSelInput)
		{
			case 0:// "Yaw":
			case 1:// "Pitch":
			case 2:// "Roll":
				mConfEuler[mCurSelInput] = idx * mEulerStep;
				Text txt = mEulers[mCurSelInput].GetComponentInChildren<Text>();
				string name = txt.text;
				string[] strings = name.Split(new char[2] { '(', ')'});
				if (strings.Length == 3)
				{
					txt.text = strings[0] + "(" + mConfEuler[mCurSelInput].ToString() + ")" + strings[2];
				}
				break;
			case 3:// "XP":
			case 4:// "YP":
			case 5:// "ZP":
				mConfPos[mCurSelInput - 3] = idx * mPosStep - mPosStep * mInputBtnCnt / 2;
				txt = mPos[mCurSelInput - 3].GetComponentInChildren<Text>();
				name = txt.text;
				strings = name.Split(new char[2] { '(', ')' });
				if (strings.Length == 3)
				{
					txt.text = strings[0] + "(" + mConfPos[mCurSelInput - 3].ToString() + ")" + strings[2];
				}
				break;
		}

		//update Action1 & Action2
		int action = (int)ActionType.ActionType_2;
		if (mCurSelInput < 3)
		{
			action = (int)ActionType.ActionType_1;
		}
		Text text = mActions[action].GetComponentInChildren<Text>();
		string str = text.text;
		if ((int)ActionType.ActionType_1 == action)
		{
			string[] strings = str.Split(new char[2] { '(', ')' });
			str = strings[0] + "(" + "Yaw=" + mConfEuler[0] + " Pitch=" + mConfEuler[1] + " Roll=" + mConfEuler[2] + ")";
		}
		if ((int)ActionType.ActionType_2 == action)
		{
			string[] strings = str.Split(new char[2] { '(', ')' });
			str = strings[0] + "(" + "X=" + mConfPos[0] + " Y=" + mConfPos[1] + " Z=" + mConfPos[2] + ")";
		}

		text.text = str;

		mPoseInputPanel.SetActive(false);
	}

	private void setupControlState(ControlState state)
	{
		if (mCtrlState == state)
		{
			return;
		}

		if (ControlState.Start == state)
		{
			mConfigPanel.SetActive(false);
		}
		else
		{
			mConfigPanel.SetActive(true);
		}

		switch (mCtrlState)
		{
			case ControlState.Start:
				selectSelectable(mStart, false);
				break;
			case ControlState.Stop:
				selectSelectable(mStop, false);
				break;
			case ControlState.Quit:
				selectSelectable(mQuit, false);
				break;
		}

		switch (state)
		{
			case ControlState.Start:
				selectSelectable(mStart, true);
				break;
			case ControlState.Stop:
				selectSelectable(mStop, true);
				break;
			case ControlState.Quit:
				selectSelectable(mQuit, true);
				break;
		}

		mCtrlState = state;
	}

	private void getAberration(WaveVR_Utils.RigidTransform start, WaveVR_Utils.RigidTransform end)
	{
		if ((int)ActionType.ActionType_1 == mSelectAction) {
			aberration = "Aberration:\n"
					+ "Euler:" + roundAngle((end.rot.eulerAngles.y - start.rot.eulerAngles.y) - mConfEuler[0]).ToString("0.0") + ","
							   + roundAngle((end.rot.eulerAngles.x - start.rot.eulerAngles.x) - mConfEuler[1]).ToString("0.0") + ","
							   + roundAngle((end.rot.eulerAngles.z - start.rot.eulerAngles.z) - mConfEuler[2]).ToString("0.0") + "\n";
			return;
		}

		if (((int)ActionType.ActionType_2 == mSelectAction))
		{
			aberration = "Aberration:\n"
					+ "Pos:" + ((end.pos.x - start.pos.x) * 100 - mConfPos[0]).ToString("0.00") + ","
								+ ((end.pos.y - start.pos.y) * 100 - mConfPos[1]).ToString("0.00") + ","
								+ ((end.pos.z - start.pos.z) * 100 - mConfPos[2]).ToString("0.00") + "\n";
			return;
		}

		aberration = "Offset:\n"
				+ "Euler:" + roundAngle(end.rot.eulerAngles.y - start.rot.eulerAngles.y).ToString("0.0") + ","
						   + roundAngle(end.rot.eulerAngles.x - start.rot.eulerAngles.x).ToString("0.0") + ","
						   + roundAngle(end.rot.eulerAngles.z - start.rot.eulerAngles.z).ToString("0.0") + "\n"
				+ "Pos:" + ((end.pos.x - start.pos.x) * 100).ToString("0.00") + ","
							+ ((end.pos.y - start.pos.y) * 100).ToString("0.00") + ","
							+ ((end.pos.z - start.pos.z) * 100).ToString("0.00") + "\n";
	}

	private void poseRecordLoop()
	{
		long startTime = DateTime.Now.Ticks;
		long cnt = 0;
		double dur = 0;
		double sampleHz = 0;

		WaveVR_Utils.RigidTransform startTransform = WaveVR_Utils.RigidTransform.identity;
		WaveVR_Utils.RigidTransform endTransform = WaveVR_Utils.RigidTransform.identity;
		//Max
		float yawMax = 0, pitchMax = 0, rollMax = 0;
		float xMax = 0, yMax = 0, zMax = 0;
		//Min
		float yawMin = 0, pitchMin = 0, rollMin = 0;
		float xMin = 0, yMin = 0, zMin = 0;
		//Sum
		float yawSum = 0, pitchSum = 0, rollSum = 0;
		float xSum = 0, ySum = 0, zSum = 0;
		//Avg
		float yawAvg = 0, pitchAvg = 0, rollAvg = 0;
		float xAvg = 0, yAvg = 0, zAvg = 0;

		WaveVR_Utils.RigidTransform transform = WaveVR_Utils.RigidTransform.identity;
		aberration = null;
		while (!bStop)
		{
			if (0 == mSelectDevice)
			{
				transform = WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_HMD).transform;
			}
			else
			{
				transform = WaveVR_Controller.Input(WVR_DeviceType.WVR_DeviceType_Controller_Right).transform;
			}

			/*
			records.Append(transform.rot.eulerAngles.y + ","
				+ transform.rot.eulerAngles.x + ","
				+ transform.rot.eulerAngles.z + ","
				+ transform.pos.x + ","
				+ transform.pos.y + ","
				+ transform.pos.z + "\n");
				*/

			if (0 == cnt)
			{
				startTransform = transform;
			}

			if (cnt % RecordSample == 0)
			{
				//PrintInfoLog("recording...");   
				getAberration(startTransform, transform);
			}

			if ((int)ActionType.ActionType_3 == mSelectAction)
			{
				float yaw = roundAngle(transform.rot.eulerAngles.y);
				float pitch = roundAngle(transform.rot.eulerAngles.x);
				float roll = roundAngle(transform.rot.eulerAngles.z);
				if (0 == yawMax)
				{
					yawMax = yawMin = yawSum = yaw;
					pitchMax = pitchMin = pitchSum = pitch;
					rollMax = rollMin = pitchSum = roll;

					xMax = xMin = xSum = transform.pos.x;
					yMax = yMin = ySum = transform.pos.y;
					zMax = zMin = zSum = transform.pos.z;
				}
				else
				{
					if (yaw > yawMax)
					{
						yawMax = yaw;
					}
					if (pitch > pitchMax)
					{
						pitchMax = pitch;
					}
					if (roll > rollMax)
					{
						rollMax = roll;
					}

					if (yaw < yawMin)
					{
						yawMin = yaw;
					}
					if (pitch < pitchMin)
					{
						pitchMin = pitch;
					}
					if (roll < rollMin)
					{
						rollMin = roll;
					}

					yawSum += yaw;
					pitchSum += pitch;
					rollSum += roll;

					if (transform.pos.x > xMax)
					{
						xMax = transform.pos.x;
					}
					if (transform.pos.y > yMax)
					{
						yMax = transform.pos.y;
					}
					if (transform.pos.z > zMax)
					{
						zMax = transform.pos.z;
					}

					if (transform.pos.x < xMin)
					{
						xMin = transform.pos.x;
					}
					if (transform.pos.y < yMin)
					{
						yMin = transform.pos.y;
					}
					if (transform.pos.z < zMin)
					{
						zMin = transform.pos.z;
					}

					xSum += transform.pos.x;
					ySum += transform.pos.y;
					zSum += transform.pos.z;
				}
			}

			cnt++;
			Thread.Sleep(1000/RecordSample);

			if (mSelectAction == (int)ActionType.ActionType_3)
			{
				TimeSpan elapsed = new TimeSpan(DateTime.Now.Ticks - startTime);
				if (elapsed.TotalSeconds > Action3SampeDuration)
				{
					autoStopAction = () => { setupControlState(ControlState.Stop); };
					bStop = true;
					break;
				}
			}
		}

		endTransform = transform;

		if ((int)ActionType.ActionType_3 == mSelectAction)
		{
			yawAvg = yawSum / cnt;
			pitchAvg = pitchSum / cnt;
			rollAvg = rollSum / cnt;
			xAvg = xSum / cnt;
			yAvg = ySum / cnt;
			zAvg = zSum / cnt;
		}

		TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - startTime);
		sampleHz = cnt/timeSpan.TotalSeconds;
		dur = timeSpan.TotalMilliseconds;

		records.Append("Count:" + cnt + "   sample rate:" + sampleHz + "  duration:" + dur + "us\n");
		records.Append("result:\n");
		records.Append(",Yaw(deg)," + "Pitch(deg)," + "Roll(deg)," + "X(cm)," + "Y(cm)," + "Z(cm)," + "\n");

		if ((int)ActionType.ActionType_3 == mSelectAction)
		{
			records.Append("Max:\n");
			records.Append("," + yawMax + ","
					+ pitchMax + ","
					+ rollMax + ","
					+ xMax * 100 + ","
					+ yMax * 100 + ","
					+ zMax * 100 + "\n");
			records.Append("Min:\n");
			records.Append("," + yawMin + ","
					+ pitchMin + ","
					+ rollMin + ","
					+ xMin * 100 + ","
					+ yMin * 100 + ","
					+ zMin * 100 + "\n");
			records.Append("Avg:\n");
			records.Append("," + yawAvg + ","
					+ pitchAvg + ","
					+ rollAvg + ","
					+ xAvg * 100 + ","
					+ yAvg * 100 + ","
					+ zAvg * 100 + "\n");
		}
		records.Append("Offset(from the end pose to the start pose):\n");
		records.Append("," + roundAngle(endTransform.rot.eulerAngles.y - startTransform.rot.eulerAngles.y) + ","
				+ roundAngle(endTransform.rot.eulerAngles.x - startTransform.rot.eulerAngles.x) + ","
				+ roundAngle(endTransform.rot.eulerAngles.z - startTransform.rot.eulerAngles.z) + ","
				+ (endTransform.pos.x - startTransform.pos.x) * 100 + ","
				+ (endTransform.pos.y - startTransform.pos.y) * 100 + ","
				+ (endTransform.pos.z - startTransform.pos.z) * 100 + "\n");
		if ((int)ActionType.ActionType_1 == mSelectAction) {
			records.Append("Aberration:\n");
			records.Append("," + roundAngle((endTransform.rot.eulerAngles.y - startTransform.rot.eulerAngles.y) - mConfEuler[0]) + ","
					+ roundAngle((endTransform.rot.eulerAngles.x - startTransform.rot.eulerAngles.x) - mConfEuler[1]) + ","
					+ roundAngle((endTransform.rot.eulerAngles.z - startTransform.rot.eulerAngles.z) - mConfEuler[2]) + ","
					+ "N/A,"
					+ "N/A,"
					+ "N/A\n");
		}
		if ((int)ActionType.ActionType_2 == mSelectAction)
		{
			records.Append("Aberration:\n");
			records.Append("," + "N/A,"
					+ "N/A,"
					+ "N/A,"
					+ ((endTransform.pos.x - startTransform.pos.x) * 100 - mConfPos[0]) + ","
					+ ((endTransform.pos.y - startTransform.pos.y) * 100 - mConfPos[1]) + ","
					+ ((endTransform.pos.z - startTransform.pos.z) * 100 - mConfPos[2]) + "\n");
		}
	}

	private float roundAngle(float angle)
	{

		if (angle < -180)
		{
			angle += 360;
		}
		else if (angle > 180)
		{
			angle -= 360;
		}

		return angle;
	}

	private void StartRecord()
	{
		PrintInfoLog("StartRecord");
		if (!bStop)
		{
			PrintDebugLog("There is a record thread existed!");
			return;
		}
		bStop = false;

		records.Append("Device: " + ((mSelectDevice == 1) ? "Controller" : "HMD") + "\n");
		string str = mActions[mSelectAction].GetComponentInChildren<Text>().text;
		if ((int)ActionType.ActionType_1 == mSelectAction)
		{
			string[] strings = str.Split(new char[2] { '(', ')' });
			str = strings[0] + "(" + "Yaw=" + mConfEuler[0] + " Pitch=" + mConfEuler[1] + " Roll=" + mConfEuler[2] + ")";
		}
		if ((int)ActionType.ActionType_2 == mSelectAction)
		{
			string[] strings = str.Split(new char[2] { '(', ')' });
			str = strings[0] + "(" + "X=" + mConfPos[0] + " Y=" + mConfPos[1] + " Z=" + mConfPos[2] + ")";
		}

		records.Append(str.Trim() + "\n");

		recordThread = new Thread(poseRecordLoop);
		//recordThread.IsBackground = true;
		recordThread.Start();
	}

	private void StopRecord()
	{
		PrintInfoLog("StopRecord");
		if (bStop)
		{
			PrintInfoLog("alread stoped.");
			return;
		}
		bStop = true;
		if (recordThread != null)
		{
			recordThread.Join();
		}
	}

	private void SaveRecord()
	{
		FileStream fs = new FileStream(fullFileName, FileMode.Truncate, FileAccess.Write);
		StreamWriter sw = new StreamWriter(fs);
		sw.Write(records.ToString());
		sw.Close();
	}

	private void ControlChanged(Button btn)
	{
		switch (btn.name)
		{
			case "Start":
				setupControlState(ControlState.Start);
				StartRecord();
				break;
			case "Stop":
				setupControlState(ControlState.Stop);
				StopRecord();
				break;
			case "Quit":
				setupControlState(ControlState.Quit);
				StopRecord();
				SaveRecord();
				SceneManager.LoadScene(0);
				break;
			case "ShowHelp":
				mHelpPanel.SetActive(true);
				break;
			case "HideHelp":
				mHelpPanel.SetActive(false);
				break;
		}
	}

	private void RegisterCtrlPanelCallback()
	{
		foreach (Button child in mCtrlPanel.GetComponentsInChildren<Button>())
		{
			PrintInfoLog("get button of mCtrlPanel " + child.name);
			switch (child.name)
			{
				case "Start":
					mStart = child;
					break;
				case "Stop":
					mStop = child;
					break;
				case "Quit":
					mQuit = child;
					break;
			}
			child.onClick.AddListener(delegate { ControlChanged(child); });
		}

		Button close = mHelpPanel.GetComponentInChildren<Button>();
		close.onClick.AddListener(delegate { ControlChanged(close); });
	}
}
