using System.Collections;
using System.Collections.Generic;
using BulletSharp;
using BulletUnity;
using UnityEngine;

public class Player3D : PlayerScript
{
	public BSoftBodyWMesh softBodyWMesh { get; protected set; }

	[SerializeField]
	private float speedForce = 10;

	[SerializeField]
	private float speedMax = 20;

	[SerializeField]
	private float jumpForce = 4000;

	//private bool canJump = false;
	//private bool canDoubleJump = false;

	private bool jumpAction = false;
	private bool eatAction = false;

	//private Animator animator;

	private BCollisionCallbacksDefault collisionCallbacks;

	protected override void Awake()
	{
		base.Awake();
		//animator = GetComponent<Animator>();
		softBodyWMesh = GetComponent<BSoftBodyWMesh>();

		collisionCallbacks = GetComponent<BCollisionCallbacksDefault>();
		collisionCallbacks.OnCollisionEnter += BOnCollisionEnter;
		collisionCallbacks.OnCollisionStay += BOnCollisionStay;
		collisionCallbacks.OnCollisionExit += BOnCollisionExit;
	}

	private void Start()
	{
		Invoke("Lol", 0);
	}

	private int indexFace, indexDos, indexLeft, indexRight, indexLeftFoot, indexRightFoot;

	private void Lol()
	{
		softBodyWMesh.DumpDataFromBullet();

		var _position = softBodyWMesh.softBody.GetAabbCenter().ToUnity();

		indexFace = GetClosetVertex(_position + new Vector3(0, 0, 1));
		indexDos = GetClosetVertex(_position + new Vector3(0, 0, -1));
		indexRight = GetClosetVertex(_position + new Vector3(1, 0, 0));
		indexLeft = GetClosetVertex(_position + new Vector3(-1, 0, 0));

		indexLeftFoot = GetClosetVertex(_position + new Vector3(-0.5f, -1, 0));
		indexRightFoot = GetClosetVertex(_position + new Vector3(0.5f, -1, 0));
	}

	private int GetClosetVertex(Vector3 position)
	{
		float distanceMin = float.PositiveInfinity;
		int index = 0;

		for (int i = 0; i < softBodyWMesh.verts.Length; i++)
		{
			float check = Vector3.Distance(softBodyWMesh.verts[i], position);

			if (check <= distanceMin)
			{
				distanceMin = check;
				index = i;
			}
		}

		return index;
	}

	public Transform transFace;
	public Transform transDos;
	public Transform transLeft;
	public Transform transRight;

	public Transform transLeftFoot;
	public Transform transRightFoot;

	private void Update()
	{
		if (softBodyWMesh.verts.Length != 0)
		{
			var _position = softBodyWMesh.softBody.GetAabbCenter().ToUnity();


			UpdateBodyPart(transFace, indexFace, _position);
			UpdateBodyPart(transDos, indexDos, _position);
			UpdateBodyPartTwo(transRight, indexRight, _position);
			UpdateBodyPartTwo(transLeft, indexLeft, _position);

			UpdateBodyPartTwo(transLeftFoot, indexLeftFoot, _position);
			UpdateBodyPartTwo(transRightFoot, indexRightFoot, _position);
		}

		/*if (Input.GetButtonDown("ActionA_" + playerID) && softBodyWMesh.velocity.y < speedMax)
		{
			jumpAction = true;
		}

		if (Input.GetButtonDown("ActionB_" + playerID))
		{
			eatAction = true;
		}*/
	}

	private const float bodyDecal = 0.2f;

	private void UpdateBodyPart(Transform trans, int index, Vector3 position)
	{
		var bodyPos = softBodyWMesh.verts[index];

		trans.position = (bodyPos - position).normalized * bodyDecal + bodyPos;

		trans.rotation = Quaternion.LookRotation((bodyPos - position).normalized, softBodyWMesh.verts[index + 1] + bodyPos);
	}

	private void UpdateBodyPartTwo(Transform trans, int index, Vector3 position)
	{
		var bodyPos = softBodyWMesh.verts[index];

		trans.position = (bodyPos - position).normalized * bodyDecal + bodyPos;

		trans.rotation = Quaternion.LookRotation(softBodyWMesh.verts[index + 1] + bodyPos, (bodyPos - position).normalized);
	}

	private float speedMove;

	private Vector3 scale = Vector3.one;
	private float pressure = 50;

	[SerializeField]
	private int eatCounter = 100;

	public void Eat()
	{
		eatAction = true;

		eatCounter--;

		if (eatCounter == 0)
		{
			LevelManager.Instance.FinishLevel();
		}
	}

	private void FixedUpdate()
	{
		if (!isControlled || PauseManager.Instance.IsPause)
		{
			return;
		}

		//Game over
		/*if (softbody.softBody.po.position.y <= -10)
		{
			softbody.velocity = Vector3.zero;
			LevelManager.Instance.Respawn();
			return;
		}*/

		position = softBodyWMesh.softBody.GetAabbCenter().ToUnity();

		if (eatAction)
		{
			eatAction = false;

			pressure += 10;

			if (pressure >= 300)
			{
				pressure = 50;
				softBodyWMesh.softBody.Scale(scale.ToBullet());
			}

			softBodyWMesh.softBody.config.Pressure = pressure;

			//Debug.LogWarning("Pressure: " + pressure);

			//scale = Vector3.one * 1.05f;
			//softBodyWMesh.SoftBodySettings.scale = scale;

			//softBodyWMesh.softBody.Scale(scale.ToBullet());

			//transform.position = position;

			//softBodyWMesh.BuildSoftBody();
			//softBodyWMesh._AddObjectToBulletWorld();
		}

		//check ground
		//canJump = Physics.CheckSphere(position. + (Vector3.up * sphereCastGroundOrig), sphereCastGround, layerMaskGround);

		var direction = Input.GetAxis("Horizontal_" + playerID) * LevelManager.Instance.mainCameraTrans.right;
		direction += Input.GetAxis("Vertical_" + playerID) * LevelManager.Instance.mainCameraTrans.forward;


		if (direction.x != 0 || direction.z != 0)
		{
			direction.y = 0;
			direction = direction.normalized * speedForce;

			var velocity = softBodyWMesh.velocity;
			velocity.y = 0;

			if (velocity.sqrMagnitude < speedMax * speedMax)
			{
				softBodyWMesh.softBody.AddVelocity(direction.ToBullet() * Time.fixedDeltaTime);
			}

			//.MoveRotation(Quaternion.RotateTowards(softbody.rotation, Quaternion.LookRotation(direction), speedRot * Time.fixedDeltaTime));

			//softbody.softBody.AddForce(direction.ToBullet());


			//softBodyWMesh.softBody.AddForce(direction.ToBullet() * Time.fixedDeltaTime, GetClosetVertex(position + new Vector3(0, 1, 0)));

		}

		if (jumpAction)
		{
			jumpAction = false;
			Jump();
		}

		//in air the player don't add externe velocity
		/*if (!canJump)
		{
			direction.y = softbody.velocity.y;
			softbody.velocity = direction;
		}*/
	}

	private void Jump()
	{
		softBodyWMesh.softBody.AddForce(_transform.up.ToBullet() * jumpForce * Time.fixedDeltaTime);
	}

	/*public override void UseActionA_Press()
	{
		Jump();
	}*/

	private void BOnCollisionEnter(BCollisionObject other)
	{
		Debug.Log("PLAYER TOUCH");
	}

	private void BOnCollisionStay(BCollisionObject other)
	{

	}

	private void BOnCollisionExit(BCollisionObject other)
	{

	}

}
