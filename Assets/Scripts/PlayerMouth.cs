using System.Collections;
using System.Collections.Generic;
using BulletUnity;
using UnityEngine;

public class PlayerMouth : MonoBehaviour
{
	private BGhostObject bGhostObject;

	public Transform targetToFollow;

	public GameObject particleSplashEat;
	public AudioClip[] audioClips;
	private AudioSource audioSource;

	private BSphereShape bSphereShape;

	private const float augmentation = 1.015f;

	private void Awake()
	{
		bGhostObject = GetComponent<BGhostObject>();
		bGhostObject.OnCollisionEnter += BOnCollisionEnter;

		bSphereShape = GetComponent<BSphereShape>();

		audioSource = GetComponent<AudioSource>();
	}

	private void FixedUpdate()
	{
		bGhostObject.SetPosition(targetToFollow.position);
	}

	private void BOnCollisionEnter(BCollisionObject other)
	{
		if (other.CompareTag("Enemy"))
		{
			AudioManager.instance.Play("EatNoise01");

			Instantiate(particleSplashEat, transform.position, Quaternion.identity);

			LevelManager.Instance?.playerScript?.Eat();

			targetToFollow.localScale *= augmentation;

			bSphereShape.LocalScaling *= augmentation;
		}
		else if (other.CompareTag("Home"))
		{
			AudioManager.instance.Play("EatNoise02");

			Instantiate(particleSplashEat, transform.position, Quaternion.identity);

			LevelManager.Instance?.playerScript?.Eat();

			targetToFollow.localScale *= augmentation;

			bSphereShape.LocalScaling *= augmentation;
		}
		else if (other.CompareTag("Monument"))
		{
			AudioManager.instance.Play("EatNoise03");

			Instantiate(particleSplashEat, transform.position, Quaternion.identity);

			LevelManager.Instance?.playerScript?.Eat();

			targetToFollow.localScale *= augmentation;

			bSphereShape.LocalScaling *= augmentation;
		}
		else if (other.CompareTag("Shit"))
		{
			AudioManager.instance.Play("KirbyHurt02");

			Instantiate(particleSplashEat, transform.position, Quaternion.identity);

			LevelManager.Instance?.playerScript?.Eat();

			targetToFollow.localScale *= augmentation;

			bSphereShape.LocalScaling *= augmentation;
		}
		else if (other.CompareTag("Sheep"))
		{
			AudioManager.instance.Play("EatNoise01");
			audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
			audioSource.PlayOneShot(audioSource.clip);

			Instantiate(particleSplashEat, transform.position, Quaternion.identity);

			LevelManager.Instance?.playerScript?.Eat();

			targetToFollow.localScale *= augmentation;

			bSphereShape.LocalScaling *= augmentation;
		}
		else if (other.CompareTag("Chimic"))
		{
			audioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
			audioSource.PlayOneShot(audioSource.clip);

			Instantiate(particleSplashEat, transform.position, Quaternion.identity);

			LevelManager.Instance?.playerScript?.Eat();

			targetToFollow.localScale *= augmentation;

			bSphereShape.LocalScaling *= augmentation;
		}
	}


}
