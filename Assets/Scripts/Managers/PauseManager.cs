using UnityEngine;

/// <summary>
/// Manage the pause system and canvas in pause state
/// </summary>
public class PauseManager : Singleton<PauseManager>
{
	/// <summary>
	/// CanvasGroup who appart in pause
	/// </summary>
	[SerializeField]
	private CanvasGroup canvasGroup;

	private bool isPause = false;
	/// <summary>
	/// On/Off pause state
	/// </summary>
	public bool IsPause
	{
		get => isPause;

		set
		{
			if (isPause == value)
			{
				return;
			}

			if (value)
			{
				Cursor.visible = true;
				Time.timeScale = 0;

				if (canvasGroup)
				{
					canvasGroup.alpha = 1;
					canvasGroup.gameObject.SetActive(true);
				}
			}
			else
			{
				Cursor.visible = false;
				Time.timeScale = 1;

				if (canvasGroup)
				{
					canvasGroup.alpha = 0;
					canvasGroup.gameObject.SetActive(false);
				}
			}

			isPause = value;
		}
	}

	private void Update()
	{
		if (GameManager.Instance.State == GameManager.States.InGame)
		{
			if (Input.GetButtonDown("Pause"))
			{
				IsPause = !IsPause;
			}
		}
		else
		{
			IsPause = false;
		}
	}

}
