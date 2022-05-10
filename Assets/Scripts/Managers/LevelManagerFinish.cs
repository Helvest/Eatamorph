using UnityEngine;

/// <summary>
/// Manage the objects of the scene
/// </summary>
public class LevelManagerFinish : LevelManager
{
	public override void FinishLevel()
	{
		GameManager.Instance.LoadScene(nextLevel);
	}
}
