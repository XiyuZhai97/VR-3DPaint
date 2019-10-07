using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

public class EnhancedTestTool : MonoBehaviour {

	public void LoadSceneCtrlBtnTest()
	{
		SceneManager.LoadScene ("CtrlBtnTest");
	}

	public void LoadSceneCtrlMTRLatencyTest()
	{
		SceneManager.LoadScene ("Controller_MotionToRender");
	}

	public void LoadSceneHmdMTRLatencyTest()
	{
		SceneManager.LoadScene ("Hmd_MotionToRender");
	}

	public void LoadSceneHmdMTPInSvrWrapperLatencyTest()
	{
		SceneManager.LoadScene ("M2P_HMD_InSvrWrapper");
	}

	public void LoadSceneTrackingAccurencyTest()
	{
		SceneManager.LoadScene("TrackingTest");
	}

	public void ExitApp()
	{
		Application.Quit();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
