using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using wvr;
using WVR_Log;

public class CIMClickHandler : MonoBehaviour {
	public void LoadScene2()
	{
		Log.d("ControllerInstanceMgrTest", "ControllerInstanceMgrTest load scene 2");

		SceneManager.LoadScene("ControllerInstanceSence_test2");
	}
}
