using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoChangeSprite : MonoBehaviour
{
	private SpriteRenderer spriteRenderer;

	[Header("Auto Flip X")]
	public bool useAutoFlipX = true;
	public float durationMinFlip = 0.3f;
	public float durationMaxFlip = 1f;

	[Header("Auto Change Sprite")]
	public bool useChangeFace = true;
	public float durationMin = 2f;
	public float durationMax = 10f;

	public Sprite[] sprites = new Sprite[0];

	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();

		if (useChangeFace)
		{
			ChangeFace();
		}

		if (useAutoFlipX)
		{
			pivoteFace();
		}
	}

	private int lastIndex = -1;
	private void ChangeFace()
	{
		int newIndex = Random.Range(0, sprites.Length - 1);

		if (newIndex == lastIndex)
		{
			newIndex++;
		}

		spriteRenderer.sprite = sprites[newIndex];

		lastIndex = newIndex;

		Invoke(nameof(ChangeFace), Random.Range(durationMin, durationMax));
	}

	private void pivoteFace()
	{
		spriteRenderer.flipX = !spriteRenderer.flipX;

		Invoke(nameof(pivoteFace), Random.Range(durationMinFlip, durationMaxFlip));
	}
}
