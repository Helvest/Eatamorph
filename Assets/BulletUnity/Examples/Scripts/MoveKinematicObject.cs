using UnityEngine;

public class MoveKinematicObject : MonoBehaviour
{
	private void FixedUpdate()
	{
		transform.position = new Vector3(Mathf.Sin(Time.time), 0f, Mathf.Cos(Time.time)) * 5f;
		//test switching between dynamic and kinematic
	}
}
