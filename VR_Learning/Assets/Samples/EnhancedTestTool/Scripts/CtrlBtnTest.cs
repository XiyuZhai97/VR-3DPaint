using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Runtime.InteropServices;
using WVR_Log;
using wvr;
using UnityEngine.SceneManagement;
using System.Text;
using System.IO;

public class CtrlBtnTest : MonoBehaviour {
	private const string LOG_TAG = "CtrlBtnTest";

	private string fullFileName;

	private static Dictionary<WVR_InputId, string> g_id2name = new Dictionary<WVR_InputId, string>()
	{
		{WVR_InputId.WVR_InputId_Alias1_System, "Button_System"},
		{WVR_InputId.WVR_InputId_Alias1_Menu, "Button_Menu"},
		{WVR_InputId.WVR_InputId_Alias1_Grip, "Button_Grip"},
		{WVR_InputId.WVR_InputId_Alias1_Volume_Up, "Button_Volume_Up"},
		{WVR_InputId.WVR_InputId_Alias1_Volume_Down, "Button_Volume_Down"},
		{WVR_InputId.WVR_InputId_Alias1_Digital_Trigger, "Button_Trigger"},
		{WVR_InputId.WVR_InputId_Alias1_Trigger, "Button_Trigger"},
		{WVR_InputId.WVR_InputId_Alias1_Touchpad, "Button_Touchpad"},
	};
	private Dictionary<WVR_DeviceType, Dictionary<WVR_InputId, GameObject>> m_type2Dict;
	private Dictionary<WVR_DeviceType, GameObject> m_type2Canvas;

	private Dictionary<string, int> g_ButtonClickRecord_R = new Dictionary<string, int>();
	private Dictionary<string, int> g_ButtonClickRecord_L = new Dictionary<string, int>();


	private StringBuilder m_ReportInfo = new StringBuilder();
	private GameObject m_report_Canvas;
	private GameObject m_reportInfoPanel;
	private GameObject m_reportInfoText;
	private GameObject m_titleCanvas;
	private GameObject m_menuCanvas;


	private static void PrintDebugLog(string msg)
	{
		Log.d (LOG_TAG, msg);
	}

	public bool IsDeviceConnected (WVR_DeviceType type)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		return Interop.WVR_IsDeviceConnected (type);
		#else
		return true;
		#endif
	}

	private Dictionary<WVR_InputId, GameObject> _InitDict (ref GameObject canvas)
	{
		Dictionary<WVR_InputId, GameObject> dict = new Dictionary<WVR_InputId, GameObject> ();

		Transform panelT = canvas.transform.Find ("ControllerPanel");
		if (panelT) {
			foreach (var item in g_id2name) {
				PrintDebugLog ("key = " + item.Key + ", value = " + item.Value);
				Transform destT = panelT.Find (item.Value);
				if (destT)
				{
					PrintDebugLog("222 key = " + item.Key + ", value = " + item.Value + ", game name =" + destT.gameObject.name);
					dict.Add(item.Key, destT.gameObject);
				}
			}
		}

		return dict;
	}

	private void InitDicts ()
	{
		m_type2Dict = new Dictionary<WVR_DeviceType, Dictionary<WVR_InputId, GameObject>> ();
		m_type2Canvas = new Dictionary<WVR_DeviceType, GameObject> ();

		GameObject leftCanvas = GameObject.Find ("LeftCanvas");
		GameObject rightCanvas = GameObject.Find ("RightCanvas");

		if (leftCanvas) {
			Dictionary<WVR_InputId, GameObject> leftDict = _InitDict (ref leftCanvas);
			m_type2Dict.Add (WVR_DeviceType.WVR_DeviceType_Controller_Left, leftDict);
			m_type2Canvas.Add (WVR_DeviceType.WVR_DeviceType_Controller_Left, leftCanvas);
		}
		if (rightCanvas) {
			Dictionary<WVR_InputId, GameObject> rightDict = _InitDict (ref rightCanvas);
			m_type2Dict.Add (WVR_DeviceType.WVR_DeviceType_Controller_Right, rightDict);
			m_type2Canvas.Add (WVR_DeviceType.WVR_DeviceType_Controller_Right, rightCanvas);
		}
	}

	private GameObject _FindGO (WVR_DeviceType type, WVR_InputId id)
	{
		GameObject go = null;
		if (m_type2Dict.ContainsKey(type)) {
			if (m_type2Dict[type].ContainsKey(id)) {
				go = m_type2Dict [type] [id];
			}
		}

		return go;
	}

	private void _UpdateCanvas(WVR_DeviceType type, bool isDeviceConnected)
	{
		PrintDebugLog ("_UpdateCanvas" + " type=" + type + " isDeviceConnected=" + isDeviceConnected);
#if UNITY_ANDROID && !UNITY_EDITOR
		if (isDeviceConnected && (type == WVR_DeviceType.WVR_DeviceType_Controller_Left || type == WVR_DeviceType.WVR_DeviceType_Controller_Right)) {
			int mask_btn = Interop.WVR_GetInputDeviceCapability (type, WVR_InputType.WVR_InputType_Button);
			string mask_base2 = Convert.ToString (mask_btn, 2);
			PrintDebugLog ("type=" + type + " mask_btn=" + mask_btn + " mask_base2=" + mask_base2);

			if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
				g_ButtonClickRecord_L.Clear();
			else
				g_ButtonClickRecord_R.Clear();

			for (WVR_InputId id = WVR_InputId.WVR_InputId_0; id < WVR_InputId.WVR_InputId_Max; id++) {
				GameObject go = _FindGO (type, id);
				if (go) {
					//set unsupported button gray
					if ((mask_btn & (1 << (int)id)) == 0) {
						go.GetComponent<Image> ().color = Color.gray;
					} else {
						if (type == WVR_DeviceType.WVR_DeviceType_Controller_Left)
							g_ButtonClickRecord_L.Add (go.name, 0);
						else
							g_ButtonClickRecord_R.Add (go.name, 0);

						go.GetComponent<Image> ().color = Color.white;
					}
				}
			}
			//special treatment of digital trigger and trigger
			GameObject go_trigger = _FindGO (type, WVR_InputId.WVR_InputId_Alias1_Trigger);

			if (go_trigger) {
				if ((mask_btn & ((1 << (int)WVR_InputId.WVR_InputId_Alias1_Digital_Trigger) | (1 << (int)WVR_InputId.WVR_InputId_Alias1_Trigger))) == 0) {
					go_trigger.GetComponent<Image> ().color = Color.gray;
				} else {
					go_trigger.GetComponent<Image> ().color = Color.white;
				}
			}
		}
#endif

		//set active state of the left or right canvas according to the connected state
		if (m_type2Canvas.ContainsKey (type)) {
			if (m_type2Canvas [type]) {
				m_type2Canvas [type].SetActive (isDeviceConnected);
			}
		}
	}

	void UpdateCanvass ()
	{
		PrintDebugLog ("UpdateCanvass");
		_UpdateCanvas (WVR_DeviceType.WVR_DeviceType_Controller_Left, IsDeviceConnected (WVR_DeviceType.WVR_DeviceType_Controller_Left));
		_UpdateCanvas (WVR_DeviceType.WVR_DeviceType_Controller_Right, IsDeviceConnected (WVR_DeviceType.WVR_DeviceType_Controller_Right));
	}

	public void BackTo()
	{
		PrintDebugLog("BackTo");
		m_report_Canvas.SetActive(false);
		m_titleCanvas.SetActive(true);
		m_menuCanvas.SetActive(true);
		SceneManager.LoadScene("EnhancedTestTool");
	}

	public void End()
	{
		PrintDebugLog ("End");

		m_titleCanvas = GameObject.Find("TitleCanvas");
		m_menuCanvas = GameObject.Find("MenuCanvas");
		
		/***TODO write all key press report to the file***/
		WriteDict(fullFileName, g_ButtonClickRecord_R, g_ButtonClickRecord_L);

		m_titleCanvas.SetActive(false);
		m_menuCanvas.SetActive(false);

		if (m_report_Canvas != null)
		{
			m_report_Canvas.SetActive(true);

			PrintDebugLog("Jesson 888" + m_ReportInfo.ToString());
			m_reportInfoText = GameObject.Find("ReportInfoText");
			m_reportInfoText.GetComponent<Text> ().text = m_ReportInfo.ToString ();
		}

		//Application.Quit();

		//SceneManager.LoadScene ("EnhancedTestTool");
	}

	void OnEnable()
	{
		// Listen to event
		WaveVR_Utils.Event.Listen (WaveVR_Utils.Event.ALL_VREVENT, OnEvent);
	}
	void OnDisable()
	{
		WaveVR_Utils.Event.Remove (WaveVR_Utils.Event.ALL_VREVENT, OnEvent);
	}

	private void _ActionEvent(WVR_DeviceType deviceType, WVR_InputId id, WVR_EventType eventType)
	{
		GameObject go = _FindGO (deviceType, id);
		if (go == null)
			return;

		switch (eventType) {
		case WVR_EventType.WVR_EventType_ButtonPressed:
			PrintDebugLog ("_ActionEvent pressdown set color red: " + "deviceType=" + deviceType + ", id= " + (int)id);
			go.GetComponent<Image> ().color = Color.red;
			RecordButton(deviceType, id);
			break;
		case WVR_EventType.WVR_EventType_ButtonUnpressed:
			PrintDebugLog ("_ActionEvent pressup set color white: " + "deviceType=" + deviceType + ", id= " + (int)id);
			go.GetComponent<Image> ().color = Color.white;
			break;
		}
	}

	/// Event handling function
	void OnEvent(params object[] args)
	{
		WVR_Event_t _event = (WVR_Event_t)args[0];
		PrintDebugLog ("OnEvent() event type=" + _event.common.type + ", inputId=" + (int)_event.input.inputId + ", device type=" + _event.device.type);
		switch (_event.common.type) {
		case WVR_EventType.WVR_EventType_ButtonPressed:
		case WVR_EventType.WVR_EventType_ButtonUnpressed:
			_ActionEvent(_event.device.type, _event.input.inputId, _event.common.type);
			break;
		case WVR_EventType.WVR_EventType_DeviceConnected:
			_UpdateCanvas (_event.device.type, true);
			break;
		case WVR_EventType.WVR_EventType_DeviceDisconnected:
			_UpdateCanvas (_event.device.type, false);
			break;
		}
	}

	private void CreateFile()
	{
		fullFileName = Application.persistentDataPath + "/" + LOG_TAG + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
		//fullFileName = @"/sdcard/" + DateTime.Now.ToString("yyyy-MM-dd") + ".csv";
		PrintDebugLog("CreateFile path:" + fullFileName);

		if (File.Exists(fullFileName))
		{
			PrintDebugLog("CreateFile 1");
			//PrintDebugLog("the out file will be overwrite.");
		}
		else
		{
			PrintDebugLog("CreateFile 2");
			File.Create(fullFileName);
		}
	}


	public void RecordButton(WVR_DeviceType deviceType, WVR_InputId id)
	{
		PrintDebugLog("Jesson RecordButton start");
		GameObject go = _FindGO(deviceType, id);
		if (go == null)
			return;

		PrintDebugLog("Jesson RecordButton:" + go.name);
		if (deviceType == WVR_DeviceType.WVR_DeviceType_Controller_Right)
		{
			UpdateButtonClickEntry(g_ButtonClickRecord_R, go.name);
		}
		else if (deviceType == WVR_DeviceType.WVR_DeviceType_Controller_Left)
		{
			UpdateButtonClickEntry(g_ButtonClickRecord_L, go.name);
		}
		else
		{
			;
		}

	}

	public void UpdateButtonClickEntry(Dictionary<string, int> dict, string key)
	{
		if (dict.ContainsKey(key))
		{
			dict[key] = dict[key] + 1;
		}
		else
		{
			dict.Add(key, 0);
		}
	}

	/*public Dictionary<key, value> ReadDict(string path) //读txt文件 返回字典
	{
		StreamReader sr = new StreamReader(path, Encoding.Default);
		String line;
		var dic = new Dictionary<string, int>();
		while ((line = sr.ReadLine()) != null)
		{
			var li = line.ToString().Split(","); //将一行用,分开成键值对
			dic.Add(li.key, li.value);
		}
		return dic;
	}*/

	public void WriteDict(string path, Dictionary<string, int> mydicR, Dictionary<string, int> mydicL)  //将字典写入txt
	{
		string str_R_Result = "is OK";
		string str_L_Result = "is OK";
		PrintDebugLog("WriteDict path:" + path);
		FileStream fs = new FileStream(path, FileMode.Create);
		StreamWriter sw = new StreamWriter(fs);
		PrintDebugLog("Jesson WriteDict start1");
		//开始写入
		sw.WriteLine("********[Right Controller]********");
		m_ReportInfo.Remove(0, m_ReportInfo.Length);
		m_ReportInfo.Append("********[Right Controller]********\n");

		if (mydicR.Count == 0)
		{
			str_R_Result = "unconnected";
		}
		else
		{
			foreach (var d in mydicR)
			{
				if (d.Key == "Button_System")
					continue;
				if (d.Value == 0)
					str_R_Result = "is FAILED"; 
				sw.WriteLine(d.Key + "," + d.Value); //键值对写入，用逗号隔开
				m_ReportInfo.Append(d.Key + "," + d.Value + "\n");
			}
		}
		PrintDebugLog("Jesson WriteDict start2");
		sw.WriteLine("********[Right Controller]********" + str_R_Result);
		m_ReportInfo.Append("********[Right Controller]********" + str_R_Result + "\n");
		sw.WriteLine("-----------------------------------------------------------");
		m_ReportInfo.Append("-----------------------------------------------------------" + "\n");
		sw.WriteLine("********[Left Controller]********");
		m_ReportInfo.Append("********[Left Controller]********" + "\n");

		if (mydicL.Count == 0)
		{
			str_L_Result = "unconnected";
		}
		else
		{
			foreach (var d in mydicL)
			{
				if (d.Key == "Button_System")
					continue;
				if (d.Value == 0)
					str_L_Result = "is FAILED";
				sw.WriteLine(d.Key + "," + d.Value); //键值对写入，用逗号隔开
				m_ReportInfo.Append(d.Key + "," + d.Value + "\n");
			}
		}
		PrintDebugLog("Jesson WriteDict start3");
		sw.WriteLine("********[Left Controller]********" + str_L_Result);
		m_ReportInfo.Append("********[Left Controller]********" + str_L_Result + "\n");
		//清空缓冲区
		sw.Flush();
		//关闭流
		sw.Close();
		fs.Close();
	}

	void Awake () {
		PrintDebugLog ("Awake");

		InitDicts ();
	}

	// Use this for initialization
	void Start () {
		PrintDebugLog ("Start: " + gameObject.name);

		m_report_Canvas = GameObject.Find("ReportCanvas");
		if (m_report_Canvas) {
			m_report_Canvas.SetActive (false);
		}

		CreateFile ();

		UpdateCanvass ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
