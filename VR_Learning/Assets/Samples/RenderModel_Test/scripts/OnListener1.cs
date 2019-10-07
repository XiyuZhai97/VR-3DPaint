using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;
using UnityEngine.UI;

public class OnListener1 : MonoBehaviour {
	void OnEnable()
	{
		WaveVR_Utils.Event.Listen(WaveVR_Utils.Event.DS_ASSETS_NOT_FOUND, onAssetNotFound);
	}

	void OnDisable()
	{
		WaveVR_Utils.Event.Remove(WaveVR_Utils.Event.DS_ASSETS_NOT_FOUND, onAssetNotFound);
	}

	// Use this for initialization
	void Start () {
		GameObject.Find("Asset").GetComponent<Text>().text = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void onAssetNotFound(params object[] args)
	{
		GameObject.Find("Asset").GetComponent<Text>().text = "Controller model asset is not found in DS.";
	}
}
