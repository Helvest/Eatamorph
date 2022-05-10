using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using BulletUnity;
using UnityEngine;

public class EatElements : MonoBehaviour
{
	private BCollisionCallbacksDefault collisionCallbacks;
	private void Awake()
	{
		collisionCallbacks = GetComponent<BCollisionCallbacksDefault>();
		collisionCallbacks.OnCollisionEnter += BOnCollisionEnter;
		collisionCallbacks.OnCollisionStay += BOnCollisionStay;
		collisionCallbacks.OnCollisionExit += BOnCollisionExit;
	}

	private void BOnCollisionEnter(BCollisionObject other)
	{
		//Debug.Log(gameObject.name + " Has collided with " + other.gameObject.name);

		if (other.gameObject.tag == "Player")
		{
			Destroy(gameObject, 0.01f);
		}
	}

	private void BOnCollisionStay(BCollisionObject other)
	{

	}

	private void BOnCollisionExit(BCollisionObject other)
	{

	}
}
