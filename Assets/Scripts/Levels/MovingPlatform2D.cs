using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform2D : MonoBehaviour
{
	private Transform _transforme;

	//pour calculer le déplacement effectuer à donner aux objets sur la plateforme
	private Vector2 lastPosition;

	/// <summary>
	/// Stop the movement of the platform
	/// </summary>
	public bool stop = false;

	/// <summary>
	/// Inverse the movement of the platform
	/// </summary>
	public bool inverseDirection = false;

	private enum Comportement
	{
		Loop,
		GoAndReturn,
		GoAndTeleport,
		Teleport
	}

	[SerializeField]
	private Comportement comportent;
	[SerializeField]
	private uint startPoint = 0;
	private uint lastPoint, nextPoint, pointsLength;
	[SerializeField]
	private float stopTime = 0;
	private float stopTimer = 0;
	[SerializeField]
	private float decalTime = 0;
	[SerializeField]
	private float speed = 1;
	[SerializeField]
	private Vector2[] points = new Vector2[1];

#if UNITY_EDITOR
	private bool isPlay = false;
#endif

	private void Awake()
	{
		if (points.Length < 1)
		{
			Debug.Log("No destination point");
			Destroy(this);
			return;
		}

		switch (comportent)
		{
			case Comportement.Teleport:
				Destroy(GetComponent<EdgeCollider2D>());
				break;
		}

		_transforme = transform;

		Vector2 originePosition = _transforme.position;

		var newPoints = new Vector2[points.Length + 1];

		newPoints[0] = originePosition;

		pointsLength = (uint)points.Length;

		//save des positions exactes
		for (uint i = 0; i < pointsLength; i++)
		{
			newPoints[i + 1] = points[i] + originePosition;
		}
		pointsLength++;
		points = newPoints;

		startPoint = (uint)Mathf.Min(startPoint, points.Length - 1);
		nextPoint = startPoint;
		GetNextObjectif();

		_transforme.position = points[startPoint];

		stopTimer = decalTime;

#if UNITY_EDITOR
		isPlay = true;
#endif
	}

	private void FixedUpdate()
	{
		if (stop)
		{
			return;
		}

		if (stopTimer <= 0)
		{
			lastPosition = _transforme.position;

			switch (comportent)
			{
				default:
				case Comportement.Loop:

					moveTimer += Time.fixedDeltaTime;
					_transforme.position = Vector2.Lerp(points[lastPoint], points[nextPoint], moveTimer / moveTime);

					if (moveTimer >= moveTime)
					{
						GetNextObjectif();
					}
					MoveObjets();
					break;

				case Comportement.Teleport:
					_transforme.position = points[nextPoint];
					GetNextObjectif();
					break;

				case Comportement.GoAndTeleport:
					if (nextPoint == 0 || nextPoint == pointsLength - 1 && inverseDirection)
					{
						_transforme.position = points[nextPoint];
					}
					else
					{
						moveTimer += Time.fixedDeltaTime;
						_transforme.position = Vector2.Lerp(points[lastPoint], points[nextPoint], moveTimer / moveTime);
						MoveObjets();

						if (moveTimer >= moveTime)
						{
							GetNextObjectif();
						}
					}
					break;
			}
		}
		else
		{
			stopTimer -= Time.fixedDeltaTime;
		}
	}

	public List<Transform> carriedObjects = new List<Transform>();

	private Vector3 tempVector;
	private void MoveObjets()
	{
		tempVector = _transforme.position - (Vector3)lastPosition;
		foreach (var objet in carriedObjects)
		{
			objet.position += tempVector;
		}
	}

	private float moveTimer, moveTime;

	private void GetNextObjectif()
	{
		lastPoint = nextPoint;

		if (inverseDirection)
		{
			if (nextPoint == 0)
			{
				switch (comportent)
				{
					default:
					case Comportement.Loop:
					case Comportement.GoAndTeleport:
					case Comportement.Teleport:
						nextPoint--;
						break;
					case Comportement.GoAndReturn:
						inverseDirection = !inverseDirection;
						nextPoint++;
						break;
				}
			}
			else
			{
				nextPoint--;
			}
		}
		else
		{
			if (nextPoint == pointsLength - 1)
			{
				switch (comportent)
				{
					default:
					case Comportement.Loop:
					case Comportement.GoAndTeleport:
					case Comportement.Teleport:
						nextPoint = 0;
						break;
					case Comportement.GoAndReturn:
						inverseDirection = !inverseDirection;
						nextPoint--;
						break;
				}
			}
			else
			{
				nextPoint++;
			}

		}
		moveTime = Vector2.Distance(points[nextPoint], points[lastPoint]) / speed;
		moveTimer = 0;
		stopTimer = stopTime;
	}

#if UNITY_EDITOR
	[Header("Gizmo")]
	public float drawSize = 0.5f;
	private int L;
	private Vector2 transPosition;
	private void OnDrawGizmos()
	{
		if (points.Length > 0)
		{
			if (inverseDirection)
			{
				Gizmos.color = Color.blue;
			}
			else
			{
				Gizmos.color = Color.cyan;
			}

			L = points.Length - 1;
			if (isPlay)
			{
				for (uint i = 0; i < L; i++)
				{
					Gizmos.DrawWireSphere(points[i], drawSize);
					Gizmos.DrawLine(points[i], points[i + 1]);
				}

				Gizmos.DrawWireSphere(points[L], drawSize);

				if (comportent != Comportement.GoAndReturn)
				{
					Gizmos.DrawLine(points[0], points[L]);
				}
			}
			else
			{
				transPosition = transform.position;

				Gizmos.DrawWireSphere(transPosition, drawSize);
				Gizmos.DrawLine(transPosition, transPosition + points[0]);

				for (uint i = 0; i < L; i++)
				{
					Gizmos.DrawWireSphere(transPosition + points[i], drawSize);
					Gizmos.DrawLine(transPosition + points[i], transPosition + points[i + 1]);
				}

				Gizmos.DrawWireSphere(transPosition + points[L], drawSize);

				if (comportent != Comportement.GoAndReturn && L > 0)
				{
					Gizmos.DrawLine(transPosition, transPosition + points[L]);
				}
			}
		}
	}
#endif

}
