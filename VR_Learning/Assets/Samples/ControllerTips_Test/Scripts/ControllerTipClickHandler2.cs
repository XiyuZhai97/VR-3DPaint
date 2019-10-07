using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using wvr;
using WVR_Log;

public class ControllerTipClickHandler2 : MonoBehaviour {

	public void LoadScene1()
	{
		Log.d("ControllerTipsTest", "Controller Tips test load scene 1");

		SceneManager.LoadScene("ControllerTips_Test");
	}
}
