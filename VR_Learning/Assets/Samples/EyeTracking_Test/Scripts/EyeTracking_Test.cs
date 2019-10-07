using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;

public class EyeTracking_Test : MonoBehaviour
{
	private const string LOG_TAG = "EyeTracking_Test";
	private void DEBUG(string msg)
	{
		Log.d (LOG_TAG, msg);
	}

	bool hasEyeTracking = false;
	void OnEnable()
	{
		hasEyeTracking = Interop.WVR_StartEyeTracking ();
		DEBUG ("OnEnable() start eye tracking " + (hasEyeTracking ? "successfully." : "failed."));
	}

	WVR_EyePose_t eyePose;
	void Update ()
	{
		bool _ret = Interop.WVR_GetEyePose (ref eyePose);
		if (_ret)
		{
			DEBUG ("leftEyePoseStatus: " + eyePose.leftEyePoseStatus
			+ "\nrightEyePoseStatus: " + eyePose.leftEyePoseStatus
			+ "\ncombinedEyePoseStatus: " + eyePose.combinedEyePoseStatus);

			DEBUG ("leftEyeGazePoint (" + eyePose.leftEyeGazePoint.v0 + ", " + eyePose.leftEyeGazePoint.v1 + ", " + eyePose.leftEyeGazePoint.v2 + ")");
			DEBUG ("rightEyeGazePoint (" + eyePose.rightEyeGazePoint.v0 + ", " + eyePose.rightEyeGazePoint.v1 + ", " + eyePose.rightEyeGazePoint.v2 + ")");
			DEBUG ("combinedEyeGazePoint (" + eyePose.combinedEyeGazePoint.v0 + ", " + eyePose.combinedEyeGazePoint.v1 + ", " + eyePose.combinedEyeGazePoint.v2 + ")");
			DEBUG ("leftEyeGazeVector (" + eyePose.leftEyeGazeVector.v0 + ", " + eyePose.leftEyeGazeVector.v1 + ", " + eyePose.leftEyeGazeVector.v2 + ")");
			DEBUG ("rightEyeGazeVector (" + eyePose.rightEyeGazeVector.v0 + ", " + eyePose.rightEyeGazeVector.v1 + ", " + eyePose.rightEyeGazeVector.v2 + ")");
			DEBUG ("combinedEyeGazeVector (" + eyePose.combinedEyeGazeVector.v0 + ", " + eyePose.combinedEyeGazeVector.v1 + ", " + eyePose.combinedEyeGazeVector.v2 + ")");

			DEBUG ("leftEyeOpenness: " + eyePose.leftEyeOpenness
			+ "\nrightEyeOpenness: " + eyePose.rightEyeOpenness
			+ "\nleftEyePupilDilation: " + eyePose.leftEyePupilDilation
			+ "\nrightEyePupilDilation: " + eyePose.rightEyePupilDilation);

			DEBUG ("leftEyePositionGuide (" + eyePose.leftEyePositionGuide.v0 + ", " + eyePose.leftEyePositionGuide.v1 + ", " + eyePose.leftEyePositionGuide.v2 + ")");
			DEBUG ("rightEyePositionGuide (" + eyePose.rightEyePositionGuide.v0 + ", " + eyePose.rightEyePositionGuide.v1 + ", " + eyePose.rightEyePositionGuide.v2 + ")");

			DEBUG ("timestamp: " + eyePose.timestamp);
		} else
		{
			DEBUG ("WVR_GetEyePose() failed.");
		}
	}

	void OnDisable()
	{
		Interop.WVR_StopEyeTracking ();
	}
}
