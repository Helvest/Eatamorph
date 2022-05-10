using UnityEngine;

/// <summary>
/// Script with functions activate by inputs
/// </summary>
public class PlayerScript : MonoBehaviour
{
	/// <summary>
	/// Optimisation for call transform
	/// </summary>
	public Transform _transform { get; protected set; }

	public bool isControlled = false;

	public Vector3 position = Vector3.zero;

	[Range(1, 4)]
	public int playerID = 1;

	protected virtual void Awake()
	{
		_transform = transform;
	}

	protected virtual void OnDestroy()
	{
		isControlled = false;
	}
}
