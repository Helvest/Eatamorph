using UnityEngine;

/// <summary>
/// Manage menu and boutons functions
/// </summary>
public class MenuManager : Singleton<MenuManager>
{
	protected override void OnAwake()
	{
		GameManager.Instance.State = GameManager.States.InMenu;
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
	/// Quit the application
	/// </summary>
	public void QuitGame()
	{
		Application.Quit();
	}

	/// <summary>
	/// Change to fullscreen
	/// </summary>
	public void FullScreen(bool active)
	{
		Screen.fullScreen = active;
	}
}
