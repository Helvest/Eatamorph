using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manage states of the game
/// </summary>
public class GameManager : Singleton<GameManager>
{
	/// <summary>
	/// Possible states of the game
	/// </summary>
	public enum States
	{
		Other,
		InMenu,
		InGame,
		InLoading
	}

	private States state = States.InMenu;
	/// <summary>
	/// Actual state of the game
	/// Make the transition from one state to another
	/// </summary>
	public States State
	{
		get => state;

		set
		{
			if (state == value)
			{
				return;
			}

			switch (value)
			{
				case States.Other:
					break;
				case States.InMenu:
					Cursor.visible = true;
					break;
				case States.InGame:
					Cursor.visible = false;
					break;
				default:
					break;
			}

			state = value;
		}
	}

	/// <summary>
	/// Load Scene with the loader
	/// </summary>
	public void LoadScene(string sceneName)
	{
		LoadingScreen.Instance.Load(sceneName);
	}

	/// <summary>
	/// Load Scene with the loader
	/// </summary>
	public void LoadScene(int sceneNumber)
	{
		LoadingScreen.Instance.Load(sceneNumber);
	}

	/// <summary>
	/// Load Scene with the loader
	/// </summary>
	public void LoadActiveScene()
	{
		LoadingScreen.Instance.Load(SceneManager.GetActiveScene().buildIndex);
	}

	/// <summary>
	/// Quit the application
	/// </summary>
	public void QuitGame()
	{
		Application.Quit();
	}
}