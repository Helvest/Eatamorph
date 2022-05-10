using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class TestChangeColor : MonoBehaviour
{
	[SerializeField, Range(0, 1)]
	private int isAdditive = 1;

	[SerializeField]
	private Color color = Color.white;

	private Material _mat;

	private void OnEnable()
	{
		var shader = Shader.Find("Test/TestChangeColor");
		if (_mat == null)
		{
			_mat = new Material(shader);
		}
	}

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		_mat.SetInt("_IsAdditive", isAdditive);
		//_mat.SetColor("_Color", color);

		_mat.color = color;

		Graphics.Blit(src, dst, _mat);
	}
}
