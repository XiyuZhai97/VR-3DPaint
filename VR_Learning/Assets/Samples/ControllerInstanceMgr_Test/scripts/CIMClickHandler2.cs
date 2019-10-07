using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using wvr;
using WVR_Log;

public class CIMClickHandler2 : MonoBehaviour {

	public void LoadScene1()
	{
		Log.d("ControllerInstanceMgrTest", "ControllerInstanceMgrTest load scene 1");

		SceneManager.LoadScene("ControllerInstanceSence_test1");
	}
}
