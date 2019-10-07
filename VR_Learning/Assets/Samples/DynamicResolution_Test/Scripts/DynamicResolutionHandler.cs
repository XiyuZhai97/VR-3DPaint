using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using wvr;
using WVR_Log;
using UnityEngine.UI;

public class DynamicResolutionHandler : MonoBehaviour
{

	public Text textField;
	// Use this for initialization
	void Start() { }

	// Update is called once per frame
	void Update() { }

	public void LoadMultiPassScene()
	{
		Log.d("DynamicResolutionTest", "DynamicResolution test load multipass scene");
		SceneManager.LoadScene("DynamicResolutionScene2_Test");
	}

	public void LoadSinglePassScene()
	{
		Log.d("DynamicResolutionTest", "DynamicResolution test load multipass scene");
		SceneManager.LoadScene("DynamicResolutionScene1_Test");
	}

	public void SetDefaultValue()//1
	{
		SetDynamicResolutionValue(1);
		printScaleInfo(1);
	}

	public void SetMediumValue()
	{
		SetDynamicResolutionValue(0.5f);
		printScaleInfo(0.5f);
	}

	public void SetLowValue()
	{
		SetDynamicResolutionValue(0.3f);
		printScaleInfo(0.3f);
	}

	private void SetDynamicResolutionValue(float value)
	{
		WaveVR_Render.Instance.SetResolutionScale(value);
	}

	public void printScaleInfo(float value)
	{
		string str = string.Empty;
		str = "Scale : " + value.ToString();
		this.textField.text = str;
	}
}
