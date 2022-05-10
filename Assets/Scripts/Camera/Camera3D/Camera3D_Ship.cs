using UnityEngine;

/// <summary>
/// Script for set camera 3D comportement on a object
/// Comportement: Limits the displacement of the camera in a sphere around the target
/// </summary>
public class Camera3D_Ship : Camera3DLogic
{
	private const int halfCercle = 180;

	[SerializeField, Range(89, 0)]
	private int verticalAxeUpLimit = 89;
	[SerializeField, Range(89, 0)]
	private int verticalAxeDownLimit = 98;

	[SerializeField]
	private bool horizontalLimit = false;
	[SerializeField, Range(0, 179)]
	private int horizontalAxeLimit = 45;
	private int horizontalAxeLimitInverse;

	[SerializeField]
	private Vector2 defaultRotation = new Vector2(0, 25);
	private Quaternion defaultRotationHorizontal;
	private Quaternion defaultRotationVertical;

	private Quaternion LastRotationHorizontal;
	private Quaternion LastRotationVertical;

	[SerializeField, Range(0, 10)]
	private float resetTime = 3;
	private float resetTimer = 0;

	[SerializeField, Range(0, 10)]
	private float repositionTime = 1;
	private float repositionTimer = 0;

	private float horizontalMove;
	private float verticalMove;

	private float testHorizonLimit;

	protected override void Awake()
	{
		_transform = transform;

		horizontalAxeLimitInverse = 360 - horizontalAxeLimit;

		verticalAxeDownLimit = 360 - verticalAxeDownLimit;

		tempVector3.Set(0, defaultRotation.x, 0);
		defaultRotationHorizontal = Quaternion.Euler(tempVector3);

		tempVector3.Set(defaultRotation.y, 0, 0);
		defaultRotationVertical = Quaternion.Euler(tempVector3);
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
			resetTimer = resetTime;
			horizontalMove *= cameraSpeed * Time.deltaTime;

			if (horizontalLimit)
			{
				tempVector3 = point3D.horizontalTrans.localEulerAngles;
				testHorizonLimit = tempVector3.y + horizontalMove;

				if (halfCercle > tempVector3.y && testHorizonLimit > horizontalAxeLimit)
				{
					tempVector3.y = horizontalAxeLimit;
					point3D.horizontalTrans.localEulerAngles = tempVector3;
				}
				else if (halfCercle < tempVector3.y && testHorizonLimit < horizontalAxeLimitInverse)
				{
					tempVector3.y = horizontalAxeLimitInverse;
					point3D.horizontalTrans.localEulerAngles = tempVector3;
				}
				else
				{
					point3D.horizontalTrans.Rotate(0, horizontalMove, 0, Space.Self);
				}
			}
			else
			{
				point3D.horizontalTrans.Rotate(0, horizontalMove, 0, Space.Self);
			}
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

		if (horizontalMove == 0 || verticalMove == 0)
		{
			if (resetTimer <= 0)
			{
				if (repositionTimer > 0)
				{
					repositionTimer -= Time.deltaTime;
					point3D.horizontalTrans.localRotation = Quaternion.Lerp(defaultRotationHorizontal, LastRotationHorizontal, repositionTimer / repositionTime);
					point3D.verticalTrans.localRotation = Quaternion.Lerp(defaultRotationVertical, LastRotationVertical, repositionTimer / repositionTime);
				}
				else
				{
					point3D.horizontalTrans.localRotation = defaultRotationHorizontal;
					point3D.verticalTrans.localRotation = defaultRotationVertical;
				}
			}
			else
			{
				resetTimer -= Time.deltaTime;
				repositionTimer = repositionTime;
				LastRotationHorizontal = point3D.horizontalTrans.localRotation;
				LastRotationVertical = point3D.verticalTrans.localRotation;
			}
		}
		else
		{
			resetTimer = resetTime;
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
		tempVector3.Set(0, defaultRotation.x, 0);
		defaultRotationHorizontal = Quaternion.Euler(tempVector3);

		tempVector3.Set(defaultRotation.y, 0, 0);
		defaultRotationVertical = Quaternion.Euler(tempVector3);

		if (_point3D && _transform)
		{
			Gizmos.color = Color.cyan;
			Gizmos.DrawLine(_point3D.pointTrans.position, _transform.position + _point3D.pointTrans.TransformDirection(offsetLook));
		}
	}
#endif

}
