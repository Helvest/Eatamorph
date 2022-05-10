using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
	public string name;

	public AudioClip audioClip;

	[HideInInspector]
	public AudioSource audioSource;

	[Range(0.0f, 1.0f)]
	public float volume = 1.0f;

	[Range(-3.0f, 3.0f)]
	public float pitch = 1.0f;

	public bool loop;
}

public class AudioManager : MonoBehaviour
{
	public static AudioManager instance;

	[SerializeField]
	private Sound[] soundsArray;

	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(gameObject);
			return;
		}

		//DontDestroyOnLoad(gameObject);

		foreach (var s in soundsArray)
		{
			s.audioSource = gameObject.AddComponent<AudioSource>();
			s.audioSource.clip = s.audioClip;

			s.audioSource.volume = s.volume;
			s.audioSource.pitch = s.pitch;
			s.audioSource.loop = s.loop;
		}
	}

	/*void Start()
    {
        PlaySound("ThemeMusic");
    }*/

	public void Play(string name)
	{
		var s = Array.Find(soundsArray, sound => sound.name == name);

		if (s == null)
		{
			Debug.LogWarning("Try to play sound: " + name + " but not found!");
			return;
		}

		s.audioSource.Play();
	}

	public void Pause(string name)
	{
		var s = Array.Find(soundsArray, sound => sound.name == name);

		if (s == null)
		{
			Debug.LogWarning("Try to pause sound: " + name + " but not found!");
			return;
		}

		s.audioSource.Pause();
	}

	public void Stop(string name)
	{
		var s = Array.Find(soundsArray, sound => sound.name == name);

		if (s == null)
		{
			Debug.LogWarning("Try to stop sound: " + name + " but not found!");
			return;
		}

		s.audioSource.Stop();
	}
}
