using System.Collections.Generic;
using UnityEngine;
using WVR_Log;

namespace wvr.render
{
	public class WaveVR_DynamicResolution : MonoBehaviour
	{
		[SerializeField]
		private List<float> dynamicResolutionList = new List<float>();

		[SerializeField]
		private int defaultIndex = 0;

		private int index = 0;

		public float CurrentScale { get { return dynamicResolutionList[index]; } }

		void OnEnable()
		{
			if (dynamicResolutionList.Count < 2)
			{
				Log.e("WVRDynRes", "Not to enable because the list is empty.");
				return;
			}

			WaveVR_Utils.Event.Listen(WVR_EventType.WVR_EventType_RecommendedQuality_Higher.ToString(), HigherHandler);
			WaveVR_Utils.Event.Listen(WVR_EventType.WVR_EventType_RecommendedQuality_Lower.ToString(), LowerHandler);
			index = defaultIndex;

			WaveVR_Render.Instance.SetResolutionScale(dynamicResolutionList[index]);
		}

		void OnDisable()
		{
			WaveVR_Utils.Event.Remove(WVR_EventType.WVR_EventType_RecommendedQuality_Higher.ToString(), HigherHandler);
			WaveVR_Utils.Event.Remove(WVR_EventType.WVR_EventType_RecommendedQuality_Lower.ToString(), LowerHandler);
			index = defaultIndex;

			WaveVR_Render.Instance.SetResolutionScale(1);
		}

		public void Higher() { HigherHandler(); }
		void HigherHandler(params object[] args)
		{
			if (--index < 0)
				index = 0;
			WaveVR_Render.Instance.SetResolutionScale(dynamicResolutionList[index]);
			Log.d("WVRDynRes", "Event Higher: [" + index + "]=" + dynamicResolutionList[index]);
		}

		public void Lower() { LowerHandler(); }
		void LowerHandler(params object[] args)
		{
			if (++index >= dynamicResolutionList.Count)
				index = dynamicResolutionList.Count - 1;
			WaveVR_Render.Instance.SetResolutionScale(dynamicResolutionList[index]);
			Log.d("WVRDynRes", "Event Lower: [" + index + "]=" + dynamicResolutionList[index]);
		}

		public void Reset()
		{
			if (!enabled)
				return;
			index = defaultIndex;
			WaveVR_Render.Instance.SetResolutionScale(dynamicResolutionList[index]);
			Log.d("WVRDynRes", "Event Reset: [" + index + "]=" + dynamicResolutionList[index]);
		}

		void OnValidate()
		{
			while (dynamicResolutionList.Count < 2)
				dynamicResolutionList.Add(1);

			if (defaultIndex < 0 || defaultIndex >= dynamicResolutionList.Count)
				defaultIndex = 0;
		}
	}
}
