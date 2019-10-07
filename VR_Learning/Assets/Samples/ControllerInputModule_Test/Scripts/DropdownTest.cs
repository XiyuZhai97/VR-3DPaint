using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using wvr;
using WVR_Log;

public class DropdownTest : MonoBehaviour {
	private static string LOG_TAG = "DropdownTest";
	public Dropdown Maindropdown;
	public Text text;
	private string[] text_strings = new string[]{ "aaa", "bbb", "ccc" };
	private Color pointerColor = new Color (26, 7, 253, 255);

	void Start () {
		// clear all option item
		Maindropdown.options.Clear ();

		// fill the dropdown menu OptionData
		foreach (string c in text_strings)
		{
			Maindropdown.options.Add (new Dropdown.OptionData () { text = c });
		}
		// this swith from 1 to 0 is only to refresh the visual menu
		Maindropdown.value = 1;
		Maindropdown.value = 0;
	}

	void Update () {
		text.text = text_strings [Maindropdown.value];

		Canvas _dropdownList_canvas = Maindropdown.gameObject.GetComponentInChildren<Canvas> ();
		Button[] _buttons = Maindropdown.gameObject.GetComponentsInChildren<Button> ();
		if (_dropdownList_canvas != null)
		{
			_dropdownList_canvas.gameObject.tag = "EventCanvas";
			foreach (Button _btn in _buttons)
			{
				Log.d (LOG_TAG, "set button " + _btn.name + " color.");
				ColorBlock _cb = _btn.colors;
				_cb.normalColor = this.pointerColor;
				_btn.colors = _cb;
			}
		}
	}
}
