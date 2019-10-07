// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using wvr.render;

namespace wvr.sample.foveated
{
	public class FoveatedTest : MonoBehaviour
	{
		WaveVR_FoveatedRendering foveated;
		float time;

		void Start()
		{
			time = Time.time;
		}

		public float FOVLarge = 57;
		public float FOVMiddle = 38;
		public float FOVSmall = 19;

		public GameObject ObjectFar;
		public GameObject ObjectNear;

		[Tooltip("Click me to change to next case")]
		public bool nextCase = false;

		public enum TestCase
		{
			Static1,
			Static2,
			Static3,  // Keep the period longer

			Disable1,

			QualityHigh_FOVLarge,
			QualityHigh_FOVMiddle,
			QualityHigh_FOVSmall,

			QualityMiddle_FOVLarge,
			QualityMiddle_FOVMiddle,
			QualityMiddle_FOVSmall,

			QualityLow_FOVLarge,
			QualityLow_FOVMiddle,
			QualityLow_FOVSmall,

			Disable2,

			TotalCase  // Keep last
		}

		public TestCase currentCase = 0;

		void NextTestCase()
		{
			currentCase++;
			if (currentCase >= TestCase.TotalCase)
				currentCase = (TestCase)0;

			switch (currentCase)
			{
				default:
				case TestCase.Disable1:
				case TestCase.Disable2:
					foveated.enabled = false;
					break;
				case TestCase.Static1:
				case TestCase.Static2:
				case TestCase.Static3:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVMiddle;
					foveated.RightClearVisionFOV = FOVMiddle;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.Middle;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.Middle;
					foveated.TrackedObject = null;
					break;

				// QualityHigh
				case TestCase.QualityHigh_FOVLarge:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVLarge;
					foveated.RightClearVisionFOV = FOVLarge;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.High;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.High;
					foveated.TrackedObject = ObjectNear;
					break;
				case TestCase.QualityHigh_FOVMiddle:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVMiddle;
					foveated.RightClearVisionFOV = FOVMiddle;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.High;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.High;
					foveated.TrackedObject = ObjectNear;
					break;
				case TestCase.QualityHigh_FOVSmall:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVSmall;
					foveated.RightClearVisionFOV = FOVSmall;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.High;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.High;
					foveated.TrackedObject = ObjectNear;
					break;

				// QualityMiddle
				case TestCase.QualityMiddle_FOVLarge:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVLarge;
					foveated.RightClearVisionFOV = FOVLarge;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.Middle;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.Middle;
					foveated.TrackedObject = ObjectNear;
					break;
				case TestCase.QualityMiddle_FOVMiddle:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVMiddle;
					foveated.RightClearVisionFOV = FOVMiddle;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.Middle;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.Middle;
					foveated.TrackedObject = ObjectNear;
					break;
				case TestCase.QualityMiddle_FOVSmall:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVSmall;
					foveated.RightClearVisionFOV = FOVSmall;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.Middle;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.Middle;
					foveated.TrackedObject = ObjectNear;
					break;

				// QualityLow
				case TestCase.QualityLow_FOVLarge:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVLarge;
					foveated.RightClearVisionFOV = FOVLarge;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.Low;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.Low;
					foveated.TrackedObject = ObjectNear;
					break;
				case TestCase.QualityLow_FOVMiddle:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVMiddle;
					foveated.RightClearVisionFOV = FOVMiddle;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.Low;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.Low;
					foveated.TrackedObject = ObjectNear;
					break;
				case TestCase.QualityLow_FOVSmall:
					foveated.enabled = true;
					foveated.LeftClearVisionFOV = FOVSmall;
					foveated.RightClearVisionFOV = FOVSmall;
					foveated.LeftPeripheralQuality = WVR_PeripheralQuality.Low;
					foveated.RightPeripheralQuality = WVR_PeripheralQuality.Low;
					foveated.TrackedObject = ObjectNear;
					break;
			}
		}

		// Update is called once per frame
		void Update()
		{
			if (foveated == null)
			{
				foveated = WaveVR_FoveatedRendering.Instance;
				if (!foveated)
					return;
			}

			if (Time.time - time > 3 || nextCase)
			{
				NextTestCase();
				time = Time.time;
				nextCase = false;
			}
		}
	}
}