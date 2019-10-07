using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using wvr;
using WVR_Log;

public class testeventhandler : MonoBehaviour,
IPointerEnterHandler,
IPointerExitHandler,
IPointerDownHandler,
IBeginDragHandler,
IDragHandler,
IEndDragHandler,
IDropHandler,
IPointerClickHandler,
IPointerUpHandler
{
	private const string LOG_TAG = "testeventhandler";
	private void PrintDebugLog (string msg)
	{
		Log.d (LOG_TAG, gameObject.name + " " + msg);
	}
	public Text text;
	public void OnPointerEnter (PointerEventData eventData)
	{
		text.text = "Enter";
		PrintDebugLog ("OnPointerEnter");
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		text.text = "Exit";
		PrintDebugLog ("OnPointerExit");
	}

	public void OnPointerDown (PointerEventData eventData)
	{
		text.text = "Down";
		PrintDebugLog ("OnPointerDown");
	}

	public void OnBeginDrag (PointerEventData eventData)
	{
		text.text = "Begin Drag";
		PrintDebugLog ("OnBeginDrag");
	}

	public void OnDrag (PointerEventData eventData)
	{
		text.text = "Dragging";
		PrintDebugLog ("OnDrag");
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		text.text = "EndDrag";
		PrintDebugLog ("OnEndDrag");
	}

	public void OnDrop (PointerEventData eventData)
	{
		text.text = "Drop";
		PrintDebugLog ("OnDrop");
	}

	public void OnPointerUp (PointerEventData eventData)
	{
		text.text = "Up";
		PrintDebugLog ("OnPointerUp");
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		text.text = "Click";
		PrintDebugLog ("OnPointerClick");
	}

	private GameObject eventSystem = null;
	// Use this for initialization
	void Start ()
	{
		PrintDebugLog ("Start()");
		setEventSystem ();
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (WaveVR_InputModuleManager.Instance != null && this.eventSystem == null)
		{
			setEventSystem ();
		}

		if (this.eventSystem != null)
		{
			WaveVR_ControllerInputModule _cim = this.eventSystem.GetComponent<WaveVR_ControllerInputModule> ();
			if (_cim != null && _cim.UnityMouseMode == false)
				_cim.UnityMouseMode = true;
		}
	}

	private void setEventSystem()
	{
		if (EventSystem.current == null)
		{
			EventSystem _es = FindObjectOfType<EventSystem> ();
			if (_es != null)
			{
				this.eventSystem = _es.gameObject;
				PrintDebugLog ("setEventSystem() find current EventSystem: " + eventSystem.name);
			}
		} else
		{
			this.eventSystem = EventSystem.current.gameObject;
			PrintDebugLog ("setEventSystem() find current EventSystem: " + eventSystem.name);
		}
	}
}
