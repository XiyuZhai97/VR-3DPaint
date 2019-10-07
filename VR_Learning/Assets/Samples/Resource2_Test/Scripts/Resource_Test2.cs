using UnityEngine;
using WVR_Log;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Resource_Test2 : MonoBehaviour
{
	public Text resource2_AppKeyEN;
	public Text resource2_HomeKeyEN;
	public Text resource2_TriggerKeyEN;
	public Text resource2_TouchPadEN;
	public Text resource2_VolumeKeyEN;
	public Text resource2_DigitalTriggerEN;
	public Text resource2_AppKeyCN;
	public Text resource2_HomeKeyCN;
	public Text resource2_TriggerKeyCN;
	public Text resource2_TouchPadCN;
	public Text resource2_VolumeKeyCN;
	public Text resource2_DigitalTriggerCN;
	public Text resource2_AppKeyTW;
	public Text resource2_HomeKeyTW;
	public Text resource2_TriggerKeyTW;
	public Text resource2_TouchPadTW;
	public Text resource2_VolumeKeyTW;
	public Text resource2_DigitalTriggerTW;
	private static string LOG_TAG = "Resource_Test2";
	private WaveVR_Resource rw = null;
	// Use this for initialization
	void Start()
	{
#if UNITY_EDITOR
		if (Application.isPlaying)
		{
			return;
		}
#endif
		rw = WaveVR_Resource.instance;

		resource2_AppKeyEN.text = rw.getStringByLanguage("AppKey", "en", "US");
		resource2_HomeKeyEN.text = rw.getStringByLanguage("HomeKey", "en", "US");
		resource2_TriggerKeyEN.text = rw.getStringByLanguage("TriggerKey", "en", "US");
		resource2_TouchPadEN.text = rw.getStringByLanguage("TouchPad", "en", "US");
		resource2_VolumeKeyEN.text = rw.getStringByLanguage("VolumeKey", "en", "US");
		resource2_DigitalTriggerEN.text = rw.getStringByLanguage("DigitalTriggerKey", "en", "US");

		resource2_AppKeyCN.text = rw.getStringByLanguage("AppKey", "zh", "CN");
		resource2_HomeKeyCN.text = rw.getStringByLanguage("HomeKey", "zh", "CN");
		resource2_TriggerKeyCN.text = rw.getStringByLanguage("TriggerKey", "zh", "CN");
		resource2_TouchPadCN.text = rw.getStringByLanguage("TouchPad", "zh", "CN");
		resource2_VolumeKeyCN.text = rw.getStringByLanguage("VolumeKey", "zh", "CN");
		resource2_DigitalTriggerCN.text = rw.getStringByLanguage("DigitalTriggerKey", "zh", "CN");

		resource2_AppKeyTW.text = rw.getStringByLanguage("AppKey", "zh", "TW");
		resource2_HomeKeyTW.text = rw.getStringByLanguage("HomeKey", "zh", "TW");
		resource2_TriggerKeyTW.text = rw.getStringByLanguage("TriggerKey", "zh", "TW");
		resource2_TouchPadTW.text = rw.getStringByLanguage("TouchPad", "zh", "TW");
		resource2_VolumeKeyTW.text = rw.getStringByLanguage("VolumeKey", "zh", "TW");
		resource2_DigitalTriggerTW.text = rw.getStringByLanguage("DigitalTriggerKey", "zh", "TW");
    }
	// Update is called once per frame
	void Update()
	{
	}

}
