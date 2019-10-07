using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;

public class OverlayHandle : MonoBehaviour {
	private static string LOG_TAG = "OverlayHandle";
	private int overlayTextureId = 0;
	private uint width = 0;
	private uint height = 0;
	private bool isShowOverlay = false;
	// Use this for initialization
	void Start () {
		Interop.WVR_GetRenderTargetSize(ref width, ref height);
		Log.d(LOG_TAG, "Overlayid width = " + width + ", Overlayid height = " + height);
	}

	// Update is called once per frame
	void Update () {

	}

	private void OnApplicationPause(bool pause)
	{
		if (WaveVR_Overlay.instance != null)
		{
			WaveVR_Overlay.instance.resetOverlayId();
			overlayTextureId = 0;

			WaveVR_Overlay.instance.resetWrapperTexture();
			overlayTextureId = WaveVR_Overlay.instance.getOverlayTextureId();
		}
		Log.d(LOG_TAG, "OnApplicationPause Go to hide overlay: " + WaveVR_Overlay.instance.getOverlayTextureId().ToString());
		WaveVR_Overlay.instance.HideOverlay();
	}

	void OnDisable()
	{
		Log.d(LOG_TAG, "onDisable Go to hide overlay: " + WaveVR_Overlay.instance.getOverlayTextureId().ToString());
		WaveVR_Overlay.instance.HideOverlay();
	}

	private void OnApplicationQuit()
	{
		overlayTextureId = WaveVR_Overlay.instance.getOverlayTextureId();
		if (overlayTextureId != 0)
		{
			WaveVR_Overlay.instance.DelOverlay();
			overlayTextureId = WaveVR_Overlay.instance.getOverlayTextureId();
			Log.d(LOG_TAG, "DelOverlay OnApplicationQuit: " + overlayTextureId);
		}
	}

	public void ExitGame()
	{
		Application.Quit();
	}

	public void ShowOverlay()
	{
		overlayTextureId = overlayTextureId = WaveVR_Overlay.instance.getOverlayTextureId();
		WaveVR_Overlay.instance.ShowOverlayTexture(width / 2, height / 2);
		Log.d(LOG_TAG, "Show Overlay isShowing: " + WaveVR_Overlay.instance.getOverlayTextureId().ToString());

	}

	public void HideOverlay()
	{
		Log.d(LOG_TAG, "Go to hide overlay: " + WaveVR_Overlay.instance.getOverlayTextureId().ToString());
		WaveVR_Overlay.instance.HideOverlay();
	}
}
