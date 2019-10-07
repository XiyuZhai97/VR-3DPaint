using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WVR_Log;

public class HMDAnimationEvent : MonoBehaviour
{
	static string TAG = "AniEvent";
	public int rollerCoasterFrameCount;
	public float rollerCoasterTime;
	public int sealedBoxFrameCount;
	public float sealedBoxTime;

	public void AnmationEvent(int e)
	{
		if (e == 0)
		{
			rollerCoasterFrameCount = Time.frameCount;
			rollerCoasterTime = Time.unscaledTime;
		}

		if (e == 1)
		{
			var passFC = Time.frameCount - rollerCoasterFrameCount;
			var passT = Time.unscaledTime - rollerCoasterTime;
			Log.d(TAG, "RollerCoaster: FrameCount=" + passFC + ", AvgFPS=" + passFC / passT);
		}

		if (e == 2)
		{
			sealedBoxFrameCount = Time.frameCount;
			sealedBoxTime = Time.unscaledTime;
		}

		if (e == 3)
		{
			var passFC = Time.frameCount - sealedBoxFrameCount;
			var passT = Time.unscaledTime - sealedBoxTime;
			Log.d(TAG, "SealedBox: FrameCount=" + passFC + ", AvgFPS=" + passFC / passT);
		}
	}
}
