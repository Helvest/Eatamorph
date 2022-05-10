using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ButtonData
{
	public float pressTime = 0f;
	public float doublepressTime = 0f;
}

[System.Serializable]
public class PlayerController
{
	public bool isActivate = true;
	public bool hasJoystick = false;

	public PlayerController(int _playerID)
	{
		playerID = _playerID;
	}

	public int playerID;

	private const string nameSeparator = "_";

	public Dictionary<string, ButtonData> buttonDatas = new Dictionary<string, ButtonData>();

	public bool GetButtonDown(string buttonName)
	{
		if (!isActivate)
		{
			return false;
		}

		if (!buttonDatas.ContainsKey(buttonName))
		{
			buttonDatas[buttonName] = new ButtonData();
		}

		bool result = Input.GetButtonDown(buttonName + nameSeparator + playerID);

		if (result && buttonDatas[buttonName].pressTime != Time.time)
		{
			buttonDatas[buttonName].doublepressTime = buttonDatas[buttonName].pressTime;
			buttonDatas[buttonName].pressTime = Time.time;
		}

		return result;
	}

	public bool GetDoubleButtonDown(string buttonName, float intervale = 0.2f)
	{
		if (GetButtonDown(buttonName))
		{
			return buttonDatas[buttonName].pressTime - buttonDatas[buttonName].doublepressTime <= intervale;
		}
		else
		{
			return false;
		}
	}
}

/// <summary>
/// Manage controller
/// </summary>
public class ControlManager : Singleton<ControlManager>
{
	//public delegate void Event();
	public delegate void EventInt(int number);

	public EventInt JoystickConnectedEvent;
	public EventInt JoystickDisconnectedEvent;

	private PlayerController playerController = new PlayerController(0);

	private List<PlayerController> playerControllerList = new List<PlayerController>();

	public PlayerController GetPlayerController(int ID = 0)
	{
		if (playerControllerList.Count > ID)
		{
			return playerControllerList[0];
		}

		return null;
	}

	public List<PlayerController> GetAllPlayerControllers => playerControllerList;

	protected override void OnAwake()
	{
		playerControllerList.Add(playerController);
		playerController.isActivate = true;
	}

	private float oneCheckSeconde = 0f;
	private int joystickLength = 0;

	private string[] joystickNames;

	private void Update()
	{
		if (oneCheckSeconde != (int)Time.time)
		{
			oneCheckSeconde = (int)Time.time;

			joystickNames = Input.GetJoystickNames();
			joystickLength = joystickNames.Length;

			while (joystickLength > playerControllerList.Count)
			{
				int ID = playerControllerList.Count;

				playerControllerList.Add(new PlayerController(ID));

				if (JoystickConnectedEvent != null)
				{
					JoystickConnectedEvent(ID);
				}
			}

			for (int i = 0; i < joystickLength; i++)
			{
				if (playerControllerList[i].hasJoystick == string.IsNullOrEmpty(joystickNames[i]))
				{
					playerControllerList[i].hasJoystick = !playerControllerList[i].hasJoystick;

					if (i != 0)
					{
						playerControllerList[i].isActivate = playerControllerList[i].hasJoystick;
					}

					if (playerControllerList[i].hasJoystick)
					{
						if (JoystickConnectedEvent != null)
						{
							JoystickConnectedEvent(playerControllerList[i].playerID);
						}

						Debug.Log("Joystick Connected: (" + i + ") " + joystickNames[i]);
					}
					else
					{
						if (JoystickDisconnectedEvent != null)
						{
							JoystickDisconnectedEvent(playerControllerList[i].playerID);
						}

						Debug.Log("Joystick Disconnected: (" + i + ") " + joystickNames[i]);
					}
				}
			}
		}
	}

}
