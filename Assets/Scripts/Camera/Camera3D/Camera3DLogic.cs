using UnityEngine;

/// <summary>
/// Script for set camera 3D comportement on a object
/// Comportement: camera is fixed on a position relative to a object
/// </summary>
public class Camera3DLogic : MonoBehaviour
{
	/// <summary>
	/// Optimisation for call transform
	/// </summary>
	protected Transform _transform;

	/// <summary>
	/// Offset position of camera
	/// </summary>
	[SerializeField]
	protected Vector3 offset = new Vector3(0, 3, 0);

	/// <summary>
	/// Offset position of target
	/// </summary>
	[SerializeField]
	protected Vector3 offsetLook = new Vector3(0, 3, 0);

	protected Vector3 tempVector3;

	[SerializeField, Range(100, 10000)]
	protected float cameraSpeed = 1000;

	[SerializeField]
	protected float distanceMin = 4;
	[SerializeField]
	protected float distanceMax = 15;
	[SerializeField, Range(0, 1)]
	protected float distanceActualPourcent = 1;

	protected virtual void Awake()
	{
		_transform = transform;
	}

	/// <summary>
	/// Update camera position with the defined comportement
	/// </summary>
	/// <param name="point3D">Object for 3D position and adjustment</param>
	public virtual void UpdatePoint(ref Point3D point3D)
	{
#if UNITY_EDITOR
		_transform = transform;
#endif

		point3D.centralTrans.position = _transform.position + _transform.TransformDirection(offset);

		point3D.pointTrans.localPosition = new Vector3(0, 0, -Mathf.LerpUnclamped(distanceMin, distanceMax, distanceActualPourcent));

		point3D.centralTrans.rotation = Quaternion.RotateTowards(point3D.centralTrans.rotation, _transform.rotation, 2);

		point3D.pointTrans.LookAt(_transform.position + _transform.TransformDirection(offsetLook));
	}
}
