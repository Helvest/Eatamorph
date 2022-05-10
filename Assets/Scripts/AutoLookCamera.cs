using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoLookCamera : MonoBehaviour
{
	public bool excludeY = true;

	private void Update()
	{
		var target = LevelManager.Instance.mainCameraTrans.position;

		if (excludeY)
		{
			target.y = transform.position.y;
		}

		transform.LookAt(target);

		target = transform.localEulerAngles;
		target.x = 0;
		target.z = 0;

		transform.localEulerAngles = target;
	}
}
