using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using wvr;
using WVR_Log;
using UnityEngine.SceneManagement;

public class CameraTextureSample : MonoBehaviour {
	public bool started = false;
	public Texture2D nativeTexture = null;
	private static string LOG_TAG = "CameraTextureSample";
	System.IntPtr textureid ;
	private MeshRenderer meshrenderer;
	private bool updated = true;
	//private bool tryStop = false;
	private WaveVR_PermissionManager pmInstance = null;
	private bool permission_granted = false;
	int count;

	// Use this for initialization
	void Start () {

	}

	public void startCamera()
	{
		//Log.d(LOG_TAG, "click startCamera");
		pmInstance = WaveVR_PermissionManager.instance;
		permission_granted = pmInstance.isPermissionGranted("android.permission.CAMERA");
		if (started==false && permission_granted)
		{
			SceneManager.sceneUnloaded += OnSceneUnloaded;

			WaveVR_CameraTexture.UpdateCameraCompletedDelegate += updateTextureCompleted;
			WaveVR_CameraTexture.StartCameraCompletedDelegate += onStartCameraCompleted;

			started = WaveVR_CameraTexture.instance.startCamera();


			nativeTexture = new Texture2D(1280, 400);
			textureid = nativeTexture.GetNativeTexturePtr();
			meshrenderer = GetComponent<MeshRenderer>();
			meshrenderer.material.mainTexture = nativeTexture;
			updated = true;
			Log.d(LOG_TAG, "startCamera");
		}
		else
		{
			Log.e(LOG_TAG, "startCamera fail, camera is already started or permissionGranted is failed");
		}
	}


	private void OnSceneUnloaded(Scene current)
	{
		Log.d(LOG_TAG, "OnSceneUnloaded and stopCamera: " + current);
		stopCamera();
	}

	public void stopCamera()
	{
		WaveVR_CameraTexture.instance.stopCamera();
		//tryStop = false;
		started = false;
		Log.d(LOG_TAG, "stopCamera");
		WaveVR_CameraTexture.UpdateCameraCompletedDelegate -= updateTextureCompleted;
		WaveVR_CameraTexture.StartCameraCompletedDelegate -= onStartCameraCompleted;
	}

	void updateTextureCompleted(uint textureId)
	{
		Log.d(LOG_TAG, "updateTextureCompleted, textureid = " + textureId);

		meshrenderer.material.mainTexture = nativeTexture;
		updated = true;
	}

	void onStartCameraCompleted(bool result)
	{
		Log.d(LOG_TAG, "onStartCameraCompleted, result = " + result);
		started = result;
	}

	void OnApplicationPause(bool pauseStatus)
	{
		if (pauseStatus)
		{
			if (started)
			{
				Log.d(LOG_TAG, "Pause(" + pauseStatus + ") and stop camera");
				stopCamera();
			}
		}
	}

	void OnDestroy()
	{
		Log.d(LOG_TAG, "OnDestroy stopCamera");
		stopCamera();
	}

	// Update is called once per frame
	void Update () {
		if (started && updated)
		{
			updated = false;
			WaveVR_CameraTexture.instance.updateTexture((uint)textureid);
			Log.d(LOG_TAG, "Update Camera 2");
		}
	}
}
