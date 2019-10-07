﻿// "WaveVR SDK 
// © 2017 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the WaveVR SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;

public class RangeFinder : MonoBehaviour {
	void Update () {
		var head = WaveVR_Render.Instance;
		if (head == null)
			return;
		RaycastHit hit;
		if (Physics.Raycast(head.transform.position, head.transform.forward, out hit, 40))
		{
			var text = DistanceUI.Instance;
			if (text != null)
				text.text = string.Format("{0,5:.000}m", hit.distance);
		}
	}
}
