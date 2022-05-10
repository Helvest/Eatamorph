using UnityEngine;

/// <summary>
/// Script for set camera 3D comportement on a object
/// Comportement: Limits the displacement of the camera in a sphere around the target
/// </summary>
public class Camera3D_FPS : Camera3DLogic
{
	private const int halfCercle = 180;

	[SerializeField, Range(89, 0)]
	private int verticalAxeUpLimit = 89;
	[SerializeField, Range(89, 0)]
	private int verticalAxeDownLimit = 89;

	private float horizontalMove;
	private float verticalMove;

	protected override void Awake()
	{
		_transform = transform;
		verticalAxeDownLimit = 360 - verticalAxeDownLimit;
	}

	/// <summary>
	/// Update camera position with the defined comportement
	/// </summary>
	/// <param name="point3D">Object for 3D position and adjustment</param>
	public override void UpdatePoint(ref Point3D point3D)
	{

#if UNITY_EDITOR
		_point3D = point3D;
		_transform = transform;
#endif

		point3D.centralTrans.position = _transform.position + _transform.TransformDirection(offset);
		point3D.centralTrans.rotation = _transform.rotation;

		horizontalMove = Input.GetAxis("Mouse X");
		if (horizontalMove != 0)
		{
			horizontalMove *= cameraSpeed * Time.deltaTime;

			point3D.horizontalTrans.Rotate(0, horizontalMove, 0, Space.Self);
		}

		verticalMove = Input.GetAxis("Mouse Y");
		if (verticalMove != 0)
		{
			verticalMove *= cameraSpeed * Time.deltaTime;

			tempVector3 = point3D.verticalTrans.localEulerAngles;
			tempVector3.x += verticalMove;

			if (tempVector3.x < halfCercle && tempVector3.x > verticalAxeUpLimit)
			{
				tempVector3.Set(verticalAxeUpLimit, 0, 0);
				point3D.verticalTrans.localEulerAngles = tempVector3;
			}
			else if (tempVector3.x > halfCercle && tempVector3.x < verticalAxeDownLimit)
			{
				tempVector3.Set(verticalAxeDownLimit, 0, 0);
				point3D.verticalTrans.localEulerAngles = tempVector3;
			}
			else
			{
				point3D.verticalTrans.Rotate(verticalMove, 0, 0, Space.Self);
			}
		}

		tempVector3.Set(0, 0, -Mathf.LerpUnclamped(distanceMin, distanceMax, distanceActualPourcent));
		point3D.pointTrans.localPosition = tempVector3;

		tempVector3.z = 0;
		point3D.pointTrans.localEulerAngles = tempVector3;

		point3D.pointTrans.LookAt(_transform.position + point3D.pointTrans.TransformDirection(offsetLook));
		tempVector3 = point3D.pointTrans.localEulerAngles;
		tempVector3.z = 0;
		point3D.pointTrans.localEulerAngles = tempVector3;
	}

#if UNITY_EDITOR
	private Point3D _point3D;

	private void OnDrawGizmos()
	{
		if (_point3D && _transform)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(_point3D.pointTrans.position, _transform.position + _point3D.pointTrans.TransformDirection(offsetLook));
		}
	}
#endif

}
