using UnityEngine;
using wvr;
using WVR_Log;
using wvr.render;

public class WaveVR_AdaptiveQuality : MonoBehaviour
{
	const string TAG = "WVRAQ";
	private static bool isEnabled = false;

	// Should we use delegate?  Or just let the client receive the AQ Events: WVR_EventType_RecommendedQuality_Higher or Lower.

	void OnEnable()
	{
		Log.i(TAG, "Enable AQ");
		Interop.WVR_EnableAdaptiveQuality(true);
		isEnabled = Interop.WVR_IsAdaptiveQualityEnabled();
		if (isEnabled)
			Log.i(TAG, "Enabled");
	}

	void OnDisable()
	{
		Log.i(TAG, "Disable AQ");
		Interop.WVR_EnableAdaptiveQuality(false);
		isEnabled = Interop.WVR_IsAdaptiveQualityEnabled();
		if (!isEnabled)
		{
			Log.i(TAG, "Disabled");
			GetComponent<WaveVR_DynamicResolution>().Reset();
		}
		Log.i(TAG, "SetPerformaceLevels all max");
		Interop.WVR_SetPerformanceLevels(WVR_PerfLevel.WVR_PerfLevel_Maximum, WVR_PerfLevel.WVR_PerfLevel_Maximum);
	}
}
