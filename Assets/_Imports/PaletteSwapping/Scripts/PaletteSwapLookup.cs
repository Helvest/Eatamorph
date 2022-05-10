using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class PaletteSwapLookup : MonoBehaviour
{
	public Texture LookupTexture;
	private Material _mat;

	private void OnEnable()
	{
		var shader = Shader.Find("Hidden/PaletteSwapLookup");
		if (_mat == null)
		{
			_mat = new Material(shader);
		}
	}

	private void OnDisable()
	{
		if (_mat != null)
		{
			DestroyImmediate(_mat);
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		_mat.SetTexture("_PaletteTex", LookupTexture);
		Graphics.Blit(src, dst, _mat);
	}


}
