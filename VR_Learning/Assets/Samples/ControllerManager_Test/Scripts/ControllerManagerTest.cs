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
using System.Collections;
using wvr;
using WVR_Log;

public class ControllerManagerTest : MonoBehaviour
{
	private static string LOG_TAG = "ControllerManagerTest";
	private WaveVR_Controller.EDeviceType eDevice = WaveVR_Controller.EDeviceType.Dominant;
	private WVR_PoseState_t pose;

	public void SetDeviceIndex(WaveVR_Controller.EDeviceType device)
	{
		Log.i (LOG_TAG, "SetDeviceIndex, _index = " + device);
		this.eDevice = device;
	}

	void Update()
	{
		#if UNITY_EDITOR
		if (Application.isEditor)
			return;
		else
		#endif
		{
			WVR_DeviceType _type = WaveVR_Controller.Input (this.eDevice).DeviceType;

			Interop.WVR_GetPoseState (
				_type,
				WVR_PoseOriginModel.WVR_PoseOriginModel_OriginOnHead,
				500,
				ref pose);

			//transform.localPosition = new WaveVR_Utils.RigidTransform (pose.PoseMatrix).pos;
			transform.localRotation = new WaveVR_Utils.RigidTransform (pose.PoseMatrix).rot;
		}
	}
}
