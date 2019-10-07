using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;

public class TeleportTest : MonoBehaviour {
	private const string LOG_TAG = "TeleportTest";
	private void PrintDebugLog(string msg)
	{
		Log.d (LOG_TAG, msg);
	}

	public GameObject Target = null, Source = null;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (this.toTeleport)
		{
			this.toTeleport = !this.toTeleport;
			TeleportToTarget ();
		}
	}

	private bool toTeleport = false;
	public void Teleport()
	{
		this.toTeleport = true;
	}

	private void TeleportToTarget ()
	{
		Vector3 _target_position =
			new Vector3 (
				this.Target.transform.position.x,
				this.Source.transform.position.y,
				this.Target.transform.position.z
			);

		PrintDebugLog ("TeleportToTarget()");
		Interop.WVR_InAppRecenter (WVR_RecenterType.WVR_RecenterType_YawAndPosition);

		this.Source.transform.position = _target_position;
	}
}
