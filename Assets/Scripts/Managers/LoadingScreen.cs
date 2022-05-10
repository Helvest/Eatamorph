using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Manage the loading screen
/// </summary>
public class LoadingScreen : Singleton<LoadingScreen>
{
	[SerializeField]
	private string LoadSceneName = "Load";

	[SerializeField]
	private float timeForScreen = 0.75f;

	[SerializeField]
	private float timeForBar = 0.25f;

	/// <summary>
	/// CanvasGroup who appart in the loading screen
	/// </summary>
	[SerializeField]
	private CanvasGroup canvasGroup;

	/// <summary>
	/// Material who maske the screen
	/// </summary>
	[SerializeField]
	private Material cameraMaterial;

	//[SerializeField]
	//private float speed = 0.25f;

	private Slider slider;

	private bool isLoading = false;

	private AsyncOperation asyncOperation;

	private int nextSceneNumber = -1;
	private string nextSceneName = null;

	protected override void OnAwake()
	{
		enabled = false;

		transform.position = new Vector3(0.5f, 0.5f, 0.0f);

		slider = GetComponentInChildren<Slider>();

		cameraMaterial.SetFloat("_Cutoff", 0);

		DontDestroyOnLoad(this);
	}

	/// <summary>
	/// Load Scene with loading screen
	/// </summary>
	public void Load(int sceneNumber)
	{
		if (isLoading)
		{
			return;
		}

		nextSceneNumber = sceneNumber;
		nextSceneName = null;
		StartCoroutine(LoadingOperation());
		StartCoroutine(AnnexeOperation());
	}

	/// <summary>
	/// Load Scene with loading screen
	/// </summary>
	public void Load(string sceneName)
	{
		if (isLoading)
		{
			return;
		}

		nextSceneNumber = -1;
		nextSceneName = sceneName;
		StartCoroutine(LoadingOperation());
		StartCoroutine(AnnexeOperation());
	}

	private IEnumerator AnnexeOperation()
	{
		while (isLoading)
		{
			//cameraMaterial.SetFloat("_Move", cameraMaterial.GetFloat("_Move") + Time.deltaTime * speed);

			yield return null;
		}
	}

	private IEnumerator LoadingOperation()
	{
		GameManager.Instance.State = GameManager.States.InLoading;

		//preparation
		isLoading = true;
		float timer = 0;
		float pourcent = 0;

		//attend fin de l'animation pour masquer écran en cour
		while (isLoading)
		{
			timer += Time.deltaTime;
			pourcent = timer / timeForScreen;

			//animations
			cameraMaterial.SetFloat("_Cutoff", pourcent);

			if (pourcent >= 1)
			{
				break;
			}

			yield return null;
		}

		if (canvasGroup)
		{
			timer = 0;
			pourcent = 0;

			//fais apparaitre l'UI du Load
			while (isLoading)
			{
				timer += Time.deltaTime;
				pourcent = timer / timeForBar;

				//animations

				canvasGroup.alpha = pourcent;

				if (pourcent >= 1)
				{
					break;
				}

				yield return null;
			}
		}

		//chargement de la scene de transition
		asyncOperation = SceneManager.LoadSceneAsync(LoadSceneName);

		//attend le chargement de la scene de transition
		while (isLoading)
		{
			if (asyncOperation.isDone)
			{
				break;
			}

			yield return null;
		}


		//chargement de la vrai scene
		if (nextSceneNumber != -1)
		{
			asyncOperation = SceneManager.LoadSceneAsync(nextSceneNumber);
		}
		else if (nextSceneName != null)
		{
			asyncOperation = SceneManager.LoadSceneAsync(nextSceneName);
		}
		else
		{
			StopAllCoroutines();
			Debug.LogError("No scene name or number valid");
		}

		while (isLoading)
		{
			if (slider)
			{
				slider.value = asyncOperation.progress;
			}

			yield return null;
			if (asyncOperation.isDone)
			{
				break;
			}
		}

		if (canvasGroup)
		{
			timer = timeForBar;
			pourcent = 0;

			//fais disparaitre l'UI du Load
			while (isLoading)
			{
				timer -= Time.deltaTime;
				pourcent = timer / timeForBar;

				//animations
				canvasGroup.alpha = pourcent;

				if (timer <= 0)
				{
					break;
				}

				yield return null;
			}
		}

		timer = timeForScreen;
		pourcent = 0;

		cameraMaterial.SetFloat("_Cutoff", 1);

		//Animation de dispartion du background de loading
		while (isLoading)
		{
			timer -= Time.deltaTime;
			pourcent = timer / timeForScreen;

			//animations
			cameraMaterial.SetFloat("_Cutoff", pourcent);

			if (timer <= 0)
			{
				isLoading = false;
			}

			yield return null;
		}
	}
}
