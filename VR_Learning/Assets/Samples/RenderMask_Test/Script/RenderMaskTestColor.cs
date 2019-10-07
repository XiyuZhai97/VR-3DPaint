using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(WaveVR_RenderMask))]
public class RenderMaskTestColor : MonoBehaviour {

	public Color32 SinglePassColor = Color.green;
	public Color32 MutiPassColor = Color.red;

	IEnumerator Start()
	{
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

		while (mask.renderMaskMeshBoth == null)
		{
			yield return null;
		}

		GetComponent<MeshFilter>().mesh = mask.renderMaskMeshBoth;
	}
}
