using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class PaletteSwapMatrix : MonoBehaviour
{
	public Color Color0;
	public Color Color1;
	public Color Color2;
	public Color Color3;
	private Material _mat;

	private void OnEnable()
	{
		var shader = Shader.Find("Hidden/PaletteSwapMatrix");
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
		_mat.SetMatrix("_ColorMatrix", ColorMatrix);
		Graphics.Blit(src, dst, _mat);
	}

	private Matrix4x4 ColorMatrix
	{
		get
		{
			var mat = new Matrix4x4();
			mat.SetRow(0, ColorToVec(Color0));
			mat.SetRow(1, ColorToVec(Color1));
			mat.SetRow(2, ColorToVec(Color2));
			mat.SetRow(3, ColorToVec(Color3));

			return mat;
		}
	}

	private Vector4 ColorToVec(Color color)
	{
		return new Vector4(color.r, color.g, color.b, color.a);
	}
}
