// Last Updated 2014_03_09

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameData;

public class PlayerData
{
	XDocument doc;
	string charName;
	float walkSpeed;
	float dashSpeed;
	float jumpForce;
	float additionalJumpForce;
	float airDashSpeed;
	int maxAirActions;
	float gravity;
	bool flying;
	List<Command> commands;
	List<string> moveSet;

	public PlayerData(string fileName)
	{
		doc = XDocument.Load (fileName);
		SetValues();
	}

	public string DebugString()
	{
		string s = "";

		IEnumerable<XElement> items = doc.Descendants ("Command");
		foreach(var command in items)
		{
			s += "ID: " + command.Attribute ("id").Value + "\n";
			IEnumerable<XElement> keyInputs = command.Descendants ("ButtonInput");
			foreach(var button in keyInputs)
			{
				s += "{ Button: " + button.Attribute("button").Value + ", Type: " + button.Attribute ("type").Value + " }\n";
			}
		}

		return s;
	}

	void SetValues()
	{
		CharName = (string) doc.Element ("GameObject").Element ("Player").Attribute ("name");
		WalkSpeed = (float) doc.Element ("GameObject").Element ("Player").Attribute ("walkSpeed");
		DashSpeed = (float) doc.Element ("GameObject").Element ("Player").Attribute ("dashSpeed");
		JumpForce = (float) doc.Element ("GameObject").Element ("Player").Attribute ("jumpForce");
		AdditionalJumpForce = (float) doc.Element ("GameObject").Element ("Player").Attribute ("additionalJumpForce");
		AirDashSpeed = (float) doc.Element ("GameObject").Element ("Player").Attribute ("airDashSpeed");
		MaxAirActions = (int) doc.Element ("GameObject").Element ("Player").Attribute ("maxAirActions");
		Gravity = (float) doc.Element ("GameObject").Element ("Player").Attribute ("gravity");
		Flying = (bool) doc.Element ("GameObject").Element ("Player").Attribute ("flying");
		Commands = new List<Command>();
		moveSet = new List<string>();
		SetCommands();
		SetMoveSet();
	}

	void SetCommands()
	{
		IEnumerable<XElement> items = doc.Descendants ("Command");
		foreach(var command in items)
		{
			string commandName = command.Attribute ("id").Value;

			List<CommandNode> c = new List<CommandNode>();
			IEnumerable<XElement> buttonInputs = command.Descendants ("ButtonInput");
			foreach(var button in buttonInputs)
			{
				Key key = StringToKey(button.Attribute ("button").Value.ToString ());
				KeyType type = StringToKeyType(button.Attribute ("type").Value.ToString ());
				c.Add (new CommandNode(key, type));
			}
			commands.Add(new Command(commandName, c));
		}
	}

	Key StringToKey(string s)
	{
		Key key;
		switch(s)
		{
		case "A":
			key = Key.A;
			break;
		case "B":
			key = Key.B;
			break;
		case "C":
			key = Key.C;
			break;
		case "D":
			key = Key.D;
			break;
		case "DOWNBACK":
		case "1":
			key = Key.DOWNBACK;
			break;
		case "DOWN":
		case "2":
			key = Key.DOWN;
			break;
		case "DOWNFORWARD":
		case "3":
			key = Key.DOWNFORWARD;
			break;
		case "BACK":
		case "4":
			key = Key.BACK;
			break;
		case "NEUTRAL":
		case "5":
			key = Key.NEUTRAL;
			break;
		case "FORWARD":
		case "6":
			key = Key.FORWARD;
			break;
		case "UPBACK":
		case "7":
			key = Key.UPBACK;
			break;
		case "UP":
		case "8":
			key = Key.UP;
			break;
		case "UPFORWARD":
		case "9":
			key = Key.UPFORWARD;
			break;
		default:
			throw new Exception(s);
		}
		return key;
	}

	KeyType StringToKeyType(string s)
	{
		KeyType type;
		switch(s)
		{
		case "NORMAL":
			type = KeyType.NORMAL;
			break;
		case "NB_NORMAL":
			type = KeyType.NB_NORMAL;
			break;
		case "NORMAL_OR":
			type = KeyType.NORMAL_OR;
			break;
		case "NB_NORMAL_OR":
			type = KeyType.NB_NORMAL_OR;
			break;
		case "SKIP":
			type = KeyType.SKIP;
			break;
		case "SKIP_AND":
			type = KeyType.SKIP_AND;
			break;
		case "CHARGE":
			type = KeyType.CHARGE;
			break;
		default:
			throw new Exception(s);
		}
		return type;
	}

	void SetMoveSet()
	{
		IEnumerable<XElement> items = doc.Descendants ("Move");
		foreach(var move in items)
		{
			string moveName = move.Attribute ("id").Value;
			moveSet.Add(moveName);
		}
	}

	public string CharName
	{
		get { return charName; }
		set { charName = value; }
	}

	public float WalkSpeed
	{
		get { return walkSpeed; }
		set { walkSpeed = value; }
	}

	public float DashSpeed
	{
		get { return dashSpeed; }
		set { dashSpeed = value; }
	}

	public float JumpForce
	{
		get { return jumpForce; }
		set { jumpForce = value; }
	}

	public float AdditionalJumpForce
	{
		get { return additionalJumpForce; }
		set { additionalJumpForce = value; }
	}

	public float AirDashSpeed
	{
		get { return airDashSpeed; }
		set { airDashSpeed = value; }
	}

	public int MaxAirActions
	{
		get { return maxAirActions; }
		set { maxAirActions = value; }
	}

	public float Gravity
	{
		get { return gravity; }
		set { gravity = value; }
	}

	public bool Flying
	{
		get { return flying; }
		set { flying = value; }
	}

	public List<Command> Commands
	{
		get { return commands; }
		set { commands = value; }
	}

	public List<string> MoveSet
	{
		get { return moveSet; }
	}
}




















