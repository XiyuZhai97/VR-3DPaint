using System.Collections;
using UnityEngine;

// A debug tool for test render mask
[RequireComponent(typeof(WaveVR_RenderMask))]
public class RenderMaskColor : MonoBehaviour
{
	public Color32 SinglePassColor = Color.green;
	public Color32 MutiPassColor = Color.red;

	IEnumerator Start() {
		while (WaveVR_Render.Instance == null)
		{
			yield return null;
		}

		var mask = GetComponent<WaveVR_RenderMask>();
		if (mask != null)
		{
			if (WaveVR_Render.Instance.IsSinglePass)
				mask.SetMaskColor(SinglePassColor);
			else
				mask.SetMaskColor(MutiPassColor);
		}
	}
}
