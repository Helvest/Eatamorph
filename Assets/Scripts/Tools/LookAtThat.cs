using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtThat : MonoBehaviour
{
	[SerializeField]
	private Transform target;

	private void Awake()
	{
		if (!target)
		{
			Destroy(this);
		}
	}

	private void Update()
	{
		transform.LookAt(target);
	}
}
