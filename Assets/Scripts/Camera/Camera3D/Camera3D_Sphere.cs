using BulletUnity;
using UnityEngine;

/// <summary>
/// Script for set camera 3D comportement on a object
/// Comportement: Limits the displacement of the camera in a sphere around the target
/// </summary>
public class Camera3D_Sphere : Camera3DLogic
{

	[SerializeField, Range(89, 0)]
	private float verticalAxeUpLimit = 89;
	[SerializeField, Range(89, 0)]
	private float verticalAxeDownLimit = 89;

	[SerializeField, Range(0, 10)]
	private float verticalLimitUp = 5;

	[SerializeField, Range(0, -10)]
	private float verticalLimiDown = -5;

	public BSoftBodyWMesh softBodyWMesh { get; protected set; }

	protected override void Awake()
	{
		_transform = transform;
		verticalAxeDownLimit = 360 - verticalAxeDownLimit;

		softBodyWMesh = GetComponent<BSoftBodyWMesh>();

		lastPosition = _transform.position;
	}

	private Vector3 lastPosition = Vector3.zero;

	/// <summary>
	/// Update camera position with the defined comportement
	/// </summary>
	/// <param name="point3D">Object for 3D position and adjustment</param>
	public override void UpdatePoint(ref Point3D point3D)
	{
		Vector3 newPosition;

		if (Application.isPlaying)
		{
			softBodyWMesh.softBody.GetAabbCenter(out var Bposition);
			newPosition = Bposition.ToUnity();
		}
		else
		{
			_transform = transform;
			newPosition = _transform.position;
		}

		float distance = Vector3.Distance(lastPosition, newPosition);
		if (distance > 0.5f)
		{
			lastPosition = Vector3.Lerp(lastPosition, newPosition - (newPosition - lastPosition).normalized * 0.5f, Time.smoothDeltaTime * Mathf.Max(4, distance));
		}

		//check dif
		float difY = lastPosition.y - point3D.centralTrans.position.y;

		//if sup
		if (difY > verticalLimitUp - offset.y)
		{
			tempVector3 = lastPosition;
			tempVector3.y -= verticalLimitUp - offset.y;
			point3D.centralTrans.position = tempVector3;
		}
		//if inf
		else if (difY < verticalLimiDown - offset.y)
		{
			tempVector3 = lastPosition;
			tempVector3.y -= verticalLimiDown - offset.y;
			point3D.centralTrans.position = tempVector3;
		}
		else
		{
			tempVector3 = lastPosition + _transform.TransformDirection(offset);
			tempVector3.y = point3D.centralTrans.position.y;
			point3D.centralTrans.position = tempVector3;
		}

		point3D.horizontalTrans.Rotate(0, Input.GetAxis("Mouse X") * cameraSpeed * Time.deltaTime, 0, Space.Self);

		point3D.verticalTrans.Rotate(Input.GetAxis("Mouse Y") * cameraSpeed * Time.deltaTime, 0, 0, Space.Self);

		tempVector3 = point3D.verticalTrans.localEulerAngles;

		if (tempVector3.y == 180)
		{
			if (tempVector3.x < 180)
			{
				tempVector3.Set(verticalAxeUpLimit, 0, 0);
			}
			else
			{
				tempVector3.Set(verticalAxeDownLimit, 0, 0);
			}

			point3D.verticalTrans.localEulerAngles = tempVector3;
		}
		else
		{
			if (tempVector3.x < 180 && tempVector3.x > verticalAxeUpLimit)
			{
				tempVector3.Set(verticalAxeUpLimit, 0, 0);
				point3D.verticalTrans.localEulerAngles = tempVector3;
			}
			else if (tempVector3.x > 180 && tempVector3.x < verticalAxeDownLimit)
			{
				tempVector3.Set(verticalAxeDownLimit, 0, 0);
				point3D.verticalTrans.localEulerAngles = tempVector3;
			}
		}

		point3D.pointTrans.localPosition = new Vector3(0, 0, -Mathf.LerpUnclamped(distanceMin, distanceMax, distanceActualPourcent));

		point3D.pointTrans.LookAt(lastPosition + _transform.TransformDirection(offsetLook));
	}

}
