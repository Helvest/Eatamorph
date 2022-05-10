using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSound : MonoBehaviour
{
	public string nameTestMusic = "ThemeMusic";

	private void Start()
	{

	}

	public void StartMusic()
	{
		AudioManager.instance.Play(nameTestMusic);
	}

	public void PauseMusic()
	{
		AudioManager.instance.Pause(nameTestMusic);
	}

	public void StopMusic()
	{
		AudioManager.instance.Stop(nameTestMusic);
	}

	public void TestSounds(string soundName)
	{
		AudioManager.instance.Play(soundName);
	}
}
