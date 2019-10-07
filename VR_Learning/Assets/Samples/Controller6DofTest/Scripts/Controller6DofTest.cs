#define DETAIL_STATUS_PARSER
//#define ONLY_FOR_TEST

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Runtime.InteropServices;
using WVR_Log;
using wvr;

public class Controller6DofTest : MonoBehaviour,
IPointerUpHandler,
IPointerEnterHandler,
IPointerExitHandler,
IPointerDownHandler,
IBeginDragHandler,
IDragHandler,
IEndDragHandler,
IDropHandler,
IPointerHoverHandler
{
	private const string LOG_TAG = "WaveVR_Controller6DofTest";
	//default set type in editor
	//public WVR_DeviceType type;

	//private GameObject m_leftCanvas;
	//private GameObject m_rightCanvas;
	//private Dictionary<WVR_InputId, GameObject> m_leftDict;
	//private Dictionary<WVR_InputId, GameObject> m_rightDict;
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

	#if UNITY_ANDROID && !UNITY_EDITOR
	private IntPtr ptrParameterName = IntPtr.Zero;
	private IntPtr ptrResult = IntPtr.Zero;
	public const uint MAX_HWSTATUS_SIZE = 256;
	#endif
	private string m_hwStatusResult = "";

	#if DETAIL_STATUS_PARSER
	private GameObject m_leftInfo;
	private GameObject m_rightInfo;
	private GameObject m_facepInfo;
	#else
	private string m_detailErrorInfo = "";
	#endif

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

	private void _UpdateHWStatus (WVR_DeviceType type)
	{
		#if UNITY_ANDROID && !UNITY_EDITOR
		string parameterName = "GetHWStatus";
		ptrParameterName = Marshal.StringToHGlobalAnsi (parameterName);
		ptrResult = Marshal.AllocHGlobal (new IntPtr(MAX_HWSTATUS_SIZE));
		Interop.WVR_GetParameters (type, ptrParameterName, ptrResult, MAX_HWSTATUS_SIZE);
		m_hwStatusResult = Marshal.PtrToStringAnsi (ptrResult);
		#endif
	}


#if DETAIL_STATUS_PARSER
	public enum ControllerDeviceID
	{
		ControllerDeviceID_Ctrl0,//ctrl0->default means right controller
		ControllerDeviceID_Ctrl1,//ctrl1->default means left controller
		ControllerDeviceID_Facep,//facep
	}

	public class ControllerDeviceStatus
	{
		//public ControllerDeviceID id;
		public string status;//true/no_connect/false
		public List<string> brokens;//details of false devices
	}

	private static ControllerDeviceStatus[] deviceStatuss = InitDeviceStatuss();
	private static ControllerDeviceStatus[] InitDeviceStatuss ()
	{
		ControllerDeviceStatus[] deviceStatuss = new ControllerDeviceStatus[Enum.GetValues (typeof(ControllerDeviceID)).Length];
		for (int i = 0; i < deviceStatuss.Length; i++) {
			deviceStatuss[i] = new ControllerDeviceStatus ();
			deviceStatuss[i].brokens = new List<string>();
		}
		return deviceStatuss;
	}

	private string GenBrokenStr (ControllerDeviceID id)
	{
		string broken_str = "";
		foreach (string broken in deviceStatuss[(int)id].brokens) {
			if (broken_str.Length > 0) {
				broken_str += ", ";
			}
			broken_str += broken;
		}
		if (broken_str.Length > 0) {
			broken_str += " broken";
		}

		return broken_str;
	}
#endif

	private void _ParseHWStatus (string hwStatus)
	{
		if (hwStatus.Length <= 0)
			return;

		#if !DETAIL_STATUS_PARSER
		m_detailErrorInfo = "";
		#endif

		string[] keyvalues = hwStatus.Split (',');
		foreach (var keyvalue in keyvalues) {
			string[] pair = keyvalue.Split ('=');
			if (pair.Length == 2) {
				#if DETAIL_STATUS_PARSER
				if (pair [0].Equals ("ctrl0_status")) {
					deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl0].status = pair [1];
				} else if (pair [0].Equals ("ctrl1_status")) {
					deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl1].status = pair [1];
				} else if (pair [0].Equals ("facep_status")) {
					deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Facep].status = pair [1];
				} else {
					if (pair [0].StartsWith ("ctrl0_")) {
						deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl0].brokens.Add (pair [0].Substring(6));
					} else if (pair [0].StartsWith ("ctrl1_")) {
						deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl1].brokens.Add (pair [0].Substring(6));
					} else if (pair [0].StartsWith ("facep_")) {
						deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Facep].brokens.Add (pair [0].Substring(6));
					} else {
						//do nothing
					}
				}
				#endif
				if (!pair [1].Equals ("true")) {
					#if !DETAIL_STATUS_PARSER
					if (m_detailErrorInfo.Length > 0) {
						m_detailErrorInfo += ",";
					}
					m_detailErrorInfo += keyvalue;
					#endif
				}
			}
		}
	}

	private void _ShowHWStatus ()
	{
#if DETAIL_STATUS_PARSER
		if (m_leftInfo) {
			if (deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl1].status.Equals("true") ) {
				m_leftInfo.GetComponent<Text> ().text = "Left controller: ok";
			} else if (deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl1].status.Equals("no_connect") ) {
				m_leftInfo.GetComponent<Text> ().text = "Left controller: no_connect";
				m_leftInfo.GetComponent<Text> ().color = Color.red;
			} else if (deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl1].status.Equals("false") ) {
				string info = "Left controller:\n" + GenBrokenStr(ControllerDeviceID.ControllerDeviceID_Ctrl1);
				m_leftInfo.GetComponent<Text> ().text = info;
				m_leftInfo.GetComponent<Text> ().color = Color.red;
			} else {
				//normally impossible
			}
		}

		if (m_rightInfo) {
			if (deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl0].status.Equals("true") ) {
				m_rightInfo.GetComponent<Text> ().text = "Right controller: ok";
			} else if (deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl0].status.Equals("no_connect") ) {
				m_rightInfo.GetComponent<Text> ().text = "Right controller: no_connect";
				m_rightInfo.GetComponent<Text> ().color = Color.red;
			} else if (deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Ctrl0].status.Equals("false") ) {
				string info = "Right controller:\n" + GenBrokenStr(ControllerDeviceID.ControllerDeviceID_Ctrl0);
				m_rightInfo.GetComponent<Text> ().text = info;
				m_rightInfo.GetComponent<Text> ().color = Color.red;
			} else {
				//normally impossible
			}
		}

		if (m_facepInfo) {
			if (deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Facep].status.Equals("true") ) {
				m_facepInfo.GetComponent<Text> ().text = "Faceplate: ok";
			} else if (deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Facep].status.Equals("no_connect") ) {
				m_facepInfo.GetComponent<Text> ().text = "Faceplate: no_connect";
				m_facepInfo.GetComponent<Text> ().color = Color.red;
			} else if (deviceStatuss [(int)ControllerDeviceID.ControllerDeviceID_Facep].status.Equals("false") ) {
				string info = "Faceplate:\n" + GenBrokenStr(ControllerDeviceID.ControllerDeviceID_Facep);
				m_facepInfo.GetComponent<Text> ().text = info;
				m_facepInfo.GetComponent<Text> ().color = Color.red;
			} else {
				//normally impossible
			}
		}
#else
		GameObject go_info = GameObject.Find ("InfoCanvas/Text");
		if (go_info) {
			if (m_detailErrorInfo.Length == 0) {
				if (m_hwStatusResult.Length == 0) {
					go_info.GetComponent<Text> ().text = "Hardware status is unknown!";
				} else {
					go_info.GetComponent<Text> ().text = m_hwStatusResult;//"Hardware status is ok!";
				}
			} else {
				go_info.GetComponent<Text> ().text = m_detailErrorInfo;
			}
		}
#endif
	}

	public void UpdateHWStatus ()
	{
		PrintDebugLog ("UpdateHWStatus");
		if (IsDeviceConnected (WVR_DeviceType.WVR_DeviceType_Controller_Right)) {
			_UpdateHWStatus (WVR_DeviceType.WVR_DeviceType_Controller_Right);
		} else if (IsDeviceConnected (WVR_DeviceType.WVR_DeviceType_Controller_Left)) {
			_UpdateHWStatus (WVR_DeviceType.WVR_DeviceType_Controller_Left);
		} else {
			//do nothing
		}

		_ParseHWStatus (m_hwStatusResult);
		_ShowHWStatus ();
	}

	private Dictionary<WVR_InputId, GameObject> _InitDict (ref GameObject canvas)
	{
		Dictionary<WVR_InputId, GameObject> dict = new Dictionary<WVR_InputId, GameObject> ();

		Transform panelT = canvas.transform.Find ("ControllerPanel");
		if (panelT) {
			foreach (var item in g_id2name) {
				//PrintDebugLog ("key = " + item.Key + ", value = " + item.Value);
				Transform destT = panelT.Find (item.Value);
				if (destT)
					dict.Add (item.Key, destT.gameObject);
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

	private void _HandleInput (WVR_DeviceType type, WVR_InputId id)
	{
		GameObject go = _FindGO (type, id);
		if (go == null)
			return;
		
		if (WaveVR_Controller.Input (type).GetPressDown (id)) {
			go.GetComponent<Image> ().color = Color.red;
		} else if (WaveVR_Controller.Input (type).GetPressUp (id)) {
			go.GetComponent<Image> ().color = Color.white;
		} else {
			//do nothing
		}

	}

	public void HandleInput()
	{
		foreach (var key in g_id2name.Keys) {
			_HandleInput (WVR_DeviceType.WVR_DeviceType_Controller_Left, key);
			_HandleInput (WVR_DeviceType.WVR_DeviceType_Controller_Right, key);
		}
	}

	public void ExitGame()
	{
		PrintDebugLog ("ExitGame");
		Application.Quit();
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

	void Awake () {
		PrintDebugLog ("Awake");

		InitDicts ();

#if DETAIL_STATUS_PARSER
		m_leftInfo = GameObject.Find ("LeftInfo");
		m_rightInfo = GameObject.Find ("RightInfo");
		m_facepInfo = GameObject.Find ("FacepInfo");
		/*deviceStatuss = new ControllerDeviceStatus[Enum.GetValues (typeof(ControllerDeviceID)).Length];
		for (int i = 0; i < deviceStatuss.Length; i++) {
			deviceStatuss[i] = new ControllerDeviceStatus ();
			deviceStatuss[i].brokens = new List<string>();
		}*/
#endif
	}

	// Use this for initialization
	void Start ()
	{
		PrintDebugLog ("Start: " + gameObject.name);

		UpdateCanvass ();

#if ONLY_FOR_TEST
		_ParseHWStatus ("ctrl0_status=true,ctrl1_status=false,ctrl1_led=false,ctrl1_tp=false,ctrl1_ch101=false,facep_status=false,facep_ch101=false");
		_ShowHWStatus ();
#else
		UpdateHWStatus ();
#endif

#if DETAIL_STATUS_PARSER
		for (int i = 0; i < deviceStatuss.Length; i++) {
			PrintDebugLog (Enum.GetName(typeof(ControllerDeviceID), i) + " status = " + deviceStatuss[i].status);
			foreach (string broken in deviceStatuss[i].brokens) {
				PrintDebugLog ("broken device = " + broken);
			}
		}
#endif
	}

	private void _UpdateCanvas(WVR_DeviceType type, bool isDeviceConnected)
	{
		PrintDebugLog ("_UpdateCanvas" + " type=" + type + " isDeviceConnected=" + isDeviceConnected);
#if UNITY_ANDROID && !UNITY_EDITOR
		if (isDeviceConnected && (type == WVR_DeviceType.WVR_DeviceType_Controller_Left || type == WVR_DeviceType.WVR_DeviceType_Controller_Right)) {
			int mask_btn = Interop.WVR_GetInputDeviceCapability (type, WVR_InputType.WVR_InputType_Button);
			string mask_base2 = Convert.ToString (mask_btn, 2);
			PrintDebugLog ("type=" + type + " mask_btn=" + mask_btn + " mask_base2=" + mask_base2);

			for (WVR_InputId id = WVR_InputId.WVR_InputId_0; id < WVR_InputId.WVR_InputId_Max; id++) {
				GameObject go = _FindGO (type, id);
				if (go) {
					//set unsupported button gray
					if ((mask_btn & (1 << (int)id)) == 0) {
						go.GetComponent<Image> ().color = Color.gray;
					} else {
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

	// Update is called once per frame
	void Update ()
	{
		//set active state of the left or right canvas according to the connected state
		/*if (m_leftCanvas)
			m_leftCanvas.SetActive (IsDeviceConnected (WVR_DeviceType.WVR_DeviceType_Controller_Left));
		if (m_rightCanvas)
			m_rightCanvas.SetActive (IsDeviceConnected (WVR_DeviceType.WVR_DeviceType_Controller_Right));*/

		//HandleInput ();

	}

	private Vector3 goPosition;
	private float goPositionZ;
	private void Rotate()
	{
		transform.Rotate (72 * (10 * Time.deltaTime), 0, 0);
		transform.Rotate (0, 72 * (10 * Time.deltaTime), 0);
	}
	#region override event handling function
	public void OnPointerEnter (PointerEventData eventData)
	{
		PrintDebugLog ("OnPointerEnter, camera: " + eventData.enterEventCamera);
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		// Do nothing
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		// Do nothing
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		PrintDebugLog ("OnPointerDown");
	}
	// Called when the pointer enters our GUI component.
	// Start tracking the mouse
	public void OnBeginDrag(PointerEventData eventData)
	{
		goPosition = transform.position;
		goPositionZ = transform.position.z;

		PrintDebugLog ("OnBeginDrag() position: " + goPosition);

		StartCoroutine( "TrackPointer" );
	}

	public void OnDrag(PointerEventData eventData)
	{
		Camera _cam = eventData.enterEventCamera;
		if (_cam != null)
			goPosition = _cam.ScreenToWorldPoint (new Vector3 (eventData.position.x, eventData.position.y, goPositionZ));
	}

	// Called when the pointer exits our GUI component.
	// Stop tracking the mouse
	public void OnEndDrag(PointerEventData eventData)
	{
		PrintDebugLog ("OnEndDrag, position: " + goPosition);

		StopCoroutine( "TrackPointer" );
	}

	public void OnDrop(PointerEventData eventData)
	{
		Camera c = eventData.enterEventCamera;
		goPosition = c.ScreenToWorldPoint (new Vector3 (eventData.position.x, eventData.position.y, goPositionZ));

		PrintDebugLog ("OnDrop, position: " + goPosition);
	}

	public void OnPointerHover (PointerEventData eventData)
	{
		transform.Rotate (0, 12 * (10 * Time.deltaTime), 0);
	}
	#endregion
}
