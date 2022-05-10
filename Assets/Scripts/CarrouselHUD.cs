using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarrouselHUD : MonoBehaviour
{
	[SerializeField]
	private CanvasGroup canvasGroup;

	[SerializeField]
	private float moveTime = 0.35f;
	private float moveTimer;

	private Vector2 originePosition;
	private Vector2 targetPosition;

	public void SetTargetX(float _x)
	{
		targetPosition.x = _x;
	}

	public void SetTargetY(float _y)
	{
		targetPosition.y = _y;
	}

	public void MoveCanvas()
	{
		originePosition = canvasGroup.transform.localPosition;

		enabled = true;

		canvasGroup.interactable = false;

		moveTimer = moveTime;
	}

	private void Update()
	{
		moveTimer -= Time.deltaTime;

		if (moveTimer <= 0f)
		{
			enabled = false;
			canvasGroup.interactable = true;
			canvasGroup.transform.localPosition = targetPosition;
		}
		else
		{
			canvasGroup.transform.localPosition = Vector2.Lerp(targetPosition, originePosition, moveTimer / moveTime);
		}
	}

	public void ExternalURL(string externalUrl)
	{
		Application.OpenURL(externalUrl);
	}
}
