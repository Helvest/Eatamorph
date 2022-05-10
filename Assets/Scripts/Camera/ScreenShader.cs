using UnityEngine;

/// <summary>
/// Script for put a shader on the camera screen
/// </summary>
[ExecuteInEditMode]
public class ScreenShader : MonoBehaviour
{
	/// <summary>
	/// Material with shader for camera screen
	/// </summary>
	public Material TransitionMaterial;

	private void OnRenderImage(RenderTexture src, RenderTexture dst)
	{
		if (TransitionMaterial != null)
		{
			Graphics.Blit(src, dst, TransitionMaterial);
		}
	}
}
