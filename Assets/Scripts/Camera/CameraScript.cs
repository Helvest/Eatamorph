using UnityEngine;

public abstract class CameraScript : MonoBehaviour
{
	protected Transform _transform;

	protected bool IFollowA = false;

	protected bool isTransition = false;
	protected EasingPart easingPart;
	protected EasingType easingType;

	protected float easeTime;
	protected float easeTimer;

	protected virtual void Awake()
	{
		_transform = transform;
	}

	/// <summary>
	/// Move camera with ease to a new target
	/// </summary>
	/// <param name="targetTransforme">new target</param>
	/// <param name="time">time of transition</param>
	/// <param name="part">EasingPart</param>
	/// <param name="type">EasingType</param>
	public virtual void SetTargetWishEase(Transform targetTransforme, float time = 1, EasingPart part = EasingPart.NoEase, EasingType type = EasingType.Linear)
	{
		isTransition = true;

		easingPart = part;
		easingType = type;
		easeTime = time;
		easeTimer = 0;

		SetTarget(targetTransforme);
	}

	/// <summary>
	/// Give to the camera a new target to follow without transition
	/// </summary>
	/// <param name="targetTransforme">new target</param>
	public virtual void SetTarget(Transform targetTransforme) { }

#if UNITY_EDITOR
	protected bool isPlay = false;
	protected Transform player;
	protected Vector3 playerPosition;
#endif
}
