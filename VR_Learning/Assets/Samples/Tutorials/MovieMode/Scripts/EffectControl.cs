using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using WVR_Log;
using wvr;
using wvr.render;

public class EffectControl : MonoBehaviour {
	const string TAG = "EffectControl";

	MovieMode moviemode = null;
	public WaveVR_AdaptiveQuality aq = null;
	public bool TriggerPressDown = false;
	public bool TriggerPressUp = false;
	public bool TouchpadTouchUp = false;
	public bool IsTouchUpper = false;

	public bool touchControlDyn = false;
	public bool touchControlObj = true;
	public int touchControlFunc = 1;

	private List<List<GameObject>> ControlledObjectList = null;
	public List<GameObject> group0;
	public List<GameObject> group1;
	public List<GameObject> group2;
	public List<GameObject> group3;
	public List<GameObject> group4;
	public List<GameObject> group5;
	public int controlledObjectIndex = 0;

	public Animator animator = null;
	public WaveVR_DevicePoseTracker poseTracker = null;

	public MeshRenderer obj1 = null, obj2 = null;

	public WaveVR_DynamicResolution dynRes = null;
	private bool isSystem;
	private bool statusBeforeSystem;

	private WaveVR_Render render;

	void Start()
	{
		moviemode = GetComponent<MovieMode>();
		if (ControlledObjectList == null)
			ControlledObjectList = new List<List<GameObject>>();

		ControlledObjectList.Clear();
		ControlledObjectList.Add(group0);
		ControlledObjectList.Add(group1);
		ControlledObjectList.Add(group2);
		ControlledObjectList.Add(group3);
		ControlledObjectList.Add(group4);
		ControlledObjectList.Add(group5);

		controlledObjectIndex = Mathf.Clamp(controlledObjectIndex, 0, ControlledObjectList.Count - 1);
		for (int i = 0; i < ControlledObjectList.Count; i++)
		{
			SetControlledObjectActive(ControlledObjectList[i], false);
		}
		SetControlledObjectActive(ControlledObjectList[controlledObjectIndex], true);

		render = WaveVR_Render.Instance;
	}

	// Update is called once per frame
	void Update () {
		var device = WaveVR_Controller.Input(WaveVR_Controller.EDeviceType.Dominant);
		//if (device.GetPressUp(WVR_InputId.WVR_InputId_Alias1_Menu))
		//	moviemode.ToggleMovieMode();

		if (animator == null || poseTracker == null)
			return;

		if (device.GetTouchUp(WVR_InputId.WVR_InputId_Alias1_Touchpad) || TouchpadTouchUp)
		{
			if (touchControlDyn && dynRes != null)
			{
				float gap = 0.3f;
				Vector2 axis = device.GetAxis(WVR_InputId.WVR_InputId_Alias1_Touchpad);
				if (Mathf.Abs(axis.x) < gap || TouchpadTouchUp)
				{
					if (axis.y < -gap || (!IsTouchUpper && TouchpadTouchUp))
					{
						dynRes.Lower();
					}
					else if (axis.y > gap || (IsTouchUpper && TouchpadTouchUp))
					{
						dynRes.Higher();
					}
				}
				//if (Mathf.Abs(axis.y) < gap || TouchpadTouchUp)
				//{
				//	if (axis.x < -gap)
				//	{
				//		//moviemode.ToggleMovieMode();
				//	}
				//	else if (axis.x > gap)
				//	{
				//		if (aq != null)
				//			aq.enabled = !aq.enabled;
				//	}
				//}
				if (Mathf.Abs(axis.y) < gap || TouchpadTouchUp)
				{
					if (axis.x > gap)
					{
						if (aq != null)
							aq.enabled = !aq.enabled;
					}
					else if (axis.x < gap)
					{
						//render.SetDiscardContent(!render.NeedDiscardContent);
					}
				}
			}

			if (touchControlObj && ControlledObjectList != null)
			{
				float gap = 0.3f;
				Vector2 axis = device.GetAxis(WVR_InputId.WVR_InputId_Alias1_Touchpad);
				if (Mathf.Abs(axis.x) < gap || TouchpadTouchUp)
				{
					var oldIndex = controlledObjectIndex;
					if (axis.y < -gap || (!IsTouchUpper && TouchpadTouchUp))
					{
						Log.d(TAG, "Obj --");
						controlledObjectIndex = Mathf.Clamp(++controlledObjectIndex, 0, ControlledObjectList.Count - 1);
					}
					else if (axis.y > gap || (IsTouchUpper && TouchpadTouchUp))
					{
						Log.d(TAG, "Obj ++");
						controlledObjectIndex = Mathf.Clamp(--controlledObjectIndex, 0, ControlledObjectList.Count - 1);
					}
					if (oldIndex != controlledObjectIndex)
					{
						SetControlledObjectActive(ControlledObjectList[oldIndex], false);
						SetControlledObjectActive(ControlledObjectList[controlledObjectIndex], true);
					}
				}

				if (Mathf.Abs(axis.y) < gap || TouchpadTouchUp)
				{
					if (axis.x > gap)
					{
						if (aq != null)
							aq.enabled = !aq.enabled;
					}
					else if (axis.x < gap)
					{
						//render.SetDiscardContent(!render.NeedDiscardContent);
					}
				}
			}
			TouchpadTouchUp = false;
		}

		if (device.GetPressDown(WVR_InputId.WVR_InputId_Alias1_Touchpad) || TriggerPressDown)
		{
			if (animator.speed == 0)
			{ }
			else
			{
				animator.speed = 0;
				Time.timeScale = 0;
				poseTracker.enabled = true;
				if (obj1)
					obj1.enabled = false;
				if (obj2)
					obj2.enabled = false;
				moviemode.DisableMovieMode();
			}
		}

		if (device.GetPressUp(WVR_InputId.WVR_InputId_Alias1_Touchpad) || TriggerPressUp)
		{
			if (animator.speed == 0)
			{
				animator.speed = 1;
				Time.timeScale = 1;
				poseTracker.transform.localPosition = Vector3.zero;
				poseTracker.transform.localRotation = Quaternion.identity;
				poseTracker.enabled = false;
				if (obj1)
					obj1.enabled = true;
				if (obj2)
					obj2.enabled = true;
				moviemode.EnableMovieMode();
			}
			else
			{ }
			TriggerPressDown = false;
			TriggerPressUp = false;
		}

		if (Interop.WVR_IsInputFocusCapturedBySystem())
		{
			moviemode.DisableMovieMode();
			isSystem = true;
			statusBeforeSystem = moviemode.GetMovieModeStatus();
		} else if (isSystem)
		{
			if (statusBeforeSystem)
				moviemode.EnableMovieMode();
			isSystem = false;
		}
	}

	void SetControlledObjectActive(List<GameObject> list, bool state)
	{
		if (list == null)
			return;

		for (int o = 0; o < list.Count; o++)
		{
			if (list[o] != null)
			{
				list[o].SetActive(state);
			}
		}
	}

	void OnValidate()
	{
		if (touchControlFunc == 1 && touchControlDyn && touchControlObj)
			touchControlDyn = false;
		if (touchControlFunc == 0 && touchControlDyn && touchControlObj)
			touchControlObj = false;
		touchControlFunc = touchControlDyn ? 1 : 0;
	}
}
