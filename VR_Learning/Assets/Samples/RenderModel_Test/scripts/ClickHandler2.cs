using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using wvr;
using WVR_Log;

public class ClickHandler2 : MonoBehaviour {

	public void LoadScene1()
	{
		Log.d("RenderModelTest", "Render model load scene 1");

		SceneManager.LoadScene("RenderModel_test");
	}
}
