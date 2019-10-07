using UnityEngine;
using UnityEngine.UI;
using wvr.render;

public class ShowQualitySettings : MonoBehaviour {

	private Text textField;
	public WaveVR_DynamicResolution dynRes;
	public WaveVR_AdaptiveQuality aq;

	void Awake()
	{
		textField = GetComponent<Text>();
	}

	void Start()
	{
		UpdateText();
	}

	void OnApplicationPause(bool pause)
	{
		UpdateText();
	}

	float time = 0;
	void Update()
	{
		time += Time.unscaledDeltaTime;
		if (time > 0.5f)
		{
			time = 0;
			UpdateText();
		}
	}

	void UpdateText()
	{
		if (dynRes != null && aq != null)
			textField.text = "aq=" + (aq.enabled ? "Enable" : "Disable") + " ResScale=" + dynRes.CurrentScale;
	}
}
