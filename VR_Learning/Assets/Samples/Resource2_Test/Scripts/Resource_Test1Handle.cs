using UnityEngine;
using WVR_Log;
using UnityEngine.UI;

//[RequireComponent(typeof(Text))]
public class Resource_Test1Handle : MonoBehaviour
{
	private string displayText = "";
	private bool setPreferredLanguageSuccess;
	//-------System----
	public Text displaytext;
	public Text resource1_AppKey;
	public Text resource1_HomeKey;
	public Text resource1_TriggerKey;
	public Text resource1_TouchPad;
	public Text resource1_VolumeKey;
	public Text resource1_DigitalTrigger;
	//-------EN----
	public Text resource1_AppKeyEN;
	public Text resource1_HomeKeyEN;
	public Text resource1_TriggerKeyEN;
	public Text resource1_TouchPadEN;
	public Text resource1_VolumeKeyEN;
	public Text resource1_DigitalTriggerEN;
	//-------CN----
	public Text resource1_AppKeyCN;
	public Text resource1_HomeKeyCN;
	public Text resource1_TriggerKeyCN;
	public Text resource1_TouchPadCN;
	public Text resource1_VolumeKeyCN;
	public Text resource1_DigitalTriggerCN;
	//-------TW----
	public Text resource1_AppKeyTW;
	public Text resource1_HomeKeyTW;
	public Text resource1_TriggerKeyTW;
	public Text resource1_TouchPadTW;
	public Text resource1_VolumeKeyTW;
	public Text resource1_DigitalTriggerTW;

	private static string LOG_TAG = "Resource_Test1Handle";
	private WaveVR_Resource rw = null;
	private string _country = "TW";
	private string _lang = "zh";

	// Use this for initialization
	void Start()
	{
		rw = WaveVR_Resource.instance;
		this.resetTextString();
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void getCountryandLanguage()
	{
		displayText = "location country : " + rw.getSystemCountry() + " \nLanguage : " + rw.getSystemLanguage();
		displaytext.text = displayText;
	}

	public void selectTW()
	{
		this.resetTextString();
		_country = "TW";
		_lang = "zh";
		setPreferredLanguageSuccess = rw.setPreferredLanguage(_lang, _country);
		if (setPreferredLanguageSuccess == true)
		{
			displayText = "zh_TWsetPreferredLanguageSuccess";
			displaytext.text = displayText;
			resource1_AppKeyTW.text = rw.getString("AppKey");
			resource1_HomeKeyTW.text = rw.getString("HomeKey");
			resource1_TriggerKeyTW.text = rw.getString("TriggerKey");
			resource1_TouchPadTW.text = rw.getString("TouchPad");
			resource1_VolumeKeyTW.text = rw.getString("VolumeKey");
			resource1_DigitalTriggerTW.text = rw.getString("DigitalTriggerKey");
		}
		else
		{
			displayText = "zh_TWsetPreferredLanguage fail";
			displaytext.text = displayText;
		}
	}

	public void selectCN()
	{
		this.resetTextString();
		_country = "CN";
		_lang = "zh";
		setPreferredLanguageSuccess = rw.setPreferredLanguage(_lang, _country);
		if (setPreferredLanguageSuccess == true)
		{
			displayText = "zh_CNsetPreferredLanguageSuccess";
			displaytext.text = displayText;
			resource1_AppKeyCN.text = rw.getString("AppKey");
			resource1_HomeKeyCN.text = rw.getString("HomeKey");
			resource1_TriggerKeyCN.text = rw.getString("TriggerKey");
			resource1_TouchPadCN.text = rw.getString("TouchPad");
			resource1_VolumeKeyCN.text = rw.getString("VolumeKey");
			resource1_DigitalTriggerCN.text = rw.getString("DigitalTriggerKey");
		}
		else
		{
			displayText = "zh_CNsetPreferredLanguage fail";
			displaytext.text = displayText;
		}
	}

	public void selectEN()
	{
		this.resetTextString();
		_country = "US";
		_lang = "en";
		setPreferredLanguageSuccess = rw.setPreferredLanguage(_lang, _country);
		if (setPreferredLanguageSuccess == true)
		{
			displayText = "eng_USsetPreferredLanguageSuccess";
			displaytext.text = displayText;
			resource1_AppKeyEN.text = rw.getString("AppKey");
			resource1_HomeKeyEN.text = rw.getString("HomeKey");
			resource1_TriggerKeyEN.text = rw.getString("TriggerKey");
			resource1_TouchPadEN.text = rw.getString("TouchPad");
			resource1_VolumeKeyEN.text = rw.getString("VolumeKey");
			resource1_DigitalTriggerEN.text = rw.getString("DigitalTriggerKey");
		}
		else
		{
			displayText = "eng_USsetPreferredLanguage fail";
			Log.d(LOG_TAG, "eng_USsetPreferredLanguage fail, location country : " + rw.getSystemCountry() + " \nLanguage : " + rw.getSystemLanguage());
			displaytext.text = displayText;
		}
	}

	public void useSystemLanguage()
	{
		this.resetTextString();
		_country = rw.getSystemCountry();
		_lang = rw.getSystemLanguage();
		setPreferredLanguageSuccess = rw.setPreferredLanguage(_lang, _country);
		if (setPreferredLanguageSuccess == true)
		{
			displayText = "Set Current Language Success";
			displaytext.text = displayText;
			resource1_AppKey.text = rw.getString("AppKey");
			resource1_HomeKey.text = rw.getString("HomeKey");
			resource1_TriggerKey.text = rw.getString("TriggerKey");
			resource1_TouchPad.text = rw.getString("TouchPad");
			resource1_VolumeKey.text = rw.getString("VolumeKey");
			resource1_DigitalTrigger.text = rw.getString("DigitalTriggerKey");
		}
		else
		{
			displayText = " set System Language fail";
			displaytext.text = displayText;
		}
	}

	private void resetTextString()
	{
		resource1_AppKey.text = "";
		resource1_HomeKey.text = "";
		resource1_TriggerKey.text = "";
		resource1_TouchPad.text = "";
		resource1_VolumeKey.text = "";
		resource1_DigitalTrigger.text = "";
		resource1_AppKeyTW.text = "";
		resource1_HomeKeyTW.text = "";
		resource1_TriggerKeyTW.text = "";
		resource1_TouchPadTW.text = "";
		resource1_VolumeKeyTW.text = "";
		resource1_DigitalTriggerTW.text = "";
		resource1_AppKeyCN.text = "";
		resource1_HomeKeyCN.text = "";
		resource1_TriggerKeyCN.text = "";
		resource1_TouchPadCN.text = "";
		resource1_VolumeKeyCN.text = "";
		resource1_DigitalTriggerCN.text = "";
		resource1_AppKeyEN.text = "";
		resource1_HomeKeyEN.text = "";
		resource1_TriggerKeyEN.text = "";
		resource1_TouchPadEN.text = "";
		resource1_VolumeKeyEN.text = "";
		resource1_DigitalTriggerEN.text = "";
	}

}
