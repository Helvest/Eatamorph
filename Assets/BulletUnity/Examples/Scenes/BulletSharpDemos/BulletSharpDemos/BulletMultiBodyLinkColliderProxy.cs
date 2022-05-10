using System.Collections;
using BulletSharp;
using BulletSharp.Math;
using BulletUnity;
using UnityEngine;

public class BulletMultiBodyLinkColliderProxy : MonoBehaviour
{
	public MultiBodyLinkCollider target;

	// Update is called once per frame
	private void Update()
	{
		var m = target.WorldTransform.ToUnity();
		transform.position = BSExtensionMethods2.ExtractTranslationFromMatrix(ref m);
		transform.rotation = BSExtensionMethods2.ExtractRotationFromMatrix(ref m);
		transform.localScale = BSExtensionMethods2.ExtractScaleFromMatrix(ref m);
	}
}
