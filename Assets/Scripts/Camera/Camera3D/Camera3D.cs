using UnityEngine;

/// <summary>
/// Script for the camera for 3D comportement
/// </summary>
public class Camera3D : CameraScript
{
	private Point3D pointA;
	private Point3D pointB;

	private Point3D point;
	private Point3D pointPre;
	private Camera3DLogic[] targets;

	protected override void Awake()
	{
		_transform = transform;

#if UNITY_EDITOR
		isPlay = true;
		player = null;

		pointA = GameObject.Find("pointA").GetComponent<Point3D>();
		pointB = GameObject.Find("pointB").GetComponent<Point3D>();

		if (pointA)
		{
			Destroy(pointA.gameObject);
		}
		if (pointB)
		{
			Destroy(pointB.gameObject);
		}
#endif
		var parent = new GameObject("Points").transform;

		pointA = new GameObject("pointA").AddComponent<Point3D>();
		pointA.transform.parent = parent;

		pointPre = pointB = new GameObject("pointB").AddComponent<Point3D>();
		pointB.transform.parent = parent;
	}

	/// <summary>
	/// Give to the camera a new target to follow without transition
	/// </summary>
	/// <param name="targetTransforme">new target</param>
	public override void SetTarget(Transform targetTransforme)
	{
		targets = targetTransforme.GetComponents<Camera3DLogic>();

		if (targets.Length == 0)
		{
			Debug.Log("Aucun script Camera3DLogic dans la cible");
			return;
		}
		else
		{
			IFollowA = !IFollowA;

			if (IFollowA)
			{
				point = pointA;
			}
			else
			{
				point = pointB;
			}

			if (!isTransition)
			{
				//point.Position = targetTransforme.position;
			}
			else
			{
				pointPre.Position = _transform.position;
			}

			point.Reset();
		}
	}

	private void Update()
	{
#if UNITY_EDITOR
		_transform = transform;
		if (point == null || targets == null)
		{
			EditorSetTarget();
			return;
		}
#endif

		foreach (var i in targets)
		{
			i.UpdatePoint(ref point);
		}

		if (isTransition)
		{
			easeTimer += Time.deltaTime;

			if (easeTimer < easeTime)
			{
				float pourcent = easeTimer / easeTime;

				_transform.position = Easing.EaseVector3(pointPre.Position, point.Position, pourcent, easingPart, easingType);
				_transform.rotation = Easing.EaseQuaternion(pointPre.Rotation, point.Rotation, pourcent, easingPart, easingType);
			}
			else
			{
				isTransition = false;
			}
		}
		else
		{
			_transform.position = point.Position;
			_transform.rotation = point.Rotation;
		}
	}

#if UNITY_EDITOR
	private void EditorSetTarget()
	{
		if (!pointA)
		{
			if (GameObject.Find("pointA"))
			{
				pointA = GameObject.Find("pointA").GetComponent<Point3D>();
			}
			if (!pointA)
			{
				pointA = new GameObject("pointA").AddComponent<Point3D>();
			}

			pointA.ForceAwake();

			if (GameObject.Find("pointB"))
			{
				pointB = GameObject.Find("pointB").GetComponent<Point3D>();
			}
			if (!pointB)
			{
				pointB = new GameObject("pointB").AddComponent<Point3D>();
			}

			pointB.ForceAwake();

			pointPre = pointB;
		}

		if (IFollowA)
		{
			point = pointA;
		}
		else
		{
			point = pointB;
		}

		var goPlayer = GameObject.FindGameObjectWithTag("Player");

		if (goPlayer)
		{
			player = goPlayer.transform;
		}

		if (player != null)
		{
			playerPosition = player.position;
			SetTarget(player);
		}
	}

	private void OnDrawGizmos()
	{
		if (!isPlay)
		{
			if (player == null || !playerPosition.Equals(player.position))
			{
				EditorSetTarget();
			}

			Update();
		}
	}
#endif
}
