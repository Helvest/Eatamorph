using UnityEngine;

/// <summary>
/// Point for 3D position and adjustment
/// </summary>
public class Point3D : MonoBehaviour
{
	public Transform centralTrans;
	public Transform horizontalTrans;
	public Transform verticalTrans;
	public Transform pointTrans;

	private void Awake()
	{
		centralTrans = transform;

		horizontalTrans = new GameObject("horizontalTrans").transform;
		horizontalTrans.parent = centralTrans;

		verticalTrans = new GameObject("verticalTrans").transform;
		verticalTrans.parent = horizontalTrans;

		pointTrans = new GameObject("pointTrans").transform;
		pointTrans.parent = verticalTrans;
	}

#if UNITY_EDITOR
	public void ForceAwake()
	{
		horizontalTrans = transform.Find("horizontalTrans");

		if (horizontalTrans)
		{
			DestroyImmediate(horizontalTrans.gameObject);
		}

		Awake();
	}
#endif

	public void Reset()
	{
		horizontalTrans.localPosition = Vector3.zero;
		verticalTrans.localPosition = Vector3.zero;
		pointTrans.localPosition = Vector3.zero;

		horizontalTrans.localRotation = Quaternion.identity;
		verticalTrans.localRotation = Quaternion.identity;
		pointTrans.localRotation = Quaternion.identity;
	}

	/// <summary>
	/// Position of the point
	/// </summary>
	public Vector3 Position
	{
		get => pointTrans.position;

		set => pointTrans.position = value;
	}

	/// <summary>
	/// Rotation of the point
	/// </summary>
	public Quaternion Rotation
	{
		get => pointTrans.rotation;

		set => pointTrans.rotation = value;
	}
}
