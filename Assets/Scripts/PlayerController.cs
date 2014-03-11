// Last Updated 2014_03_09

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameData;

public class PlayerController : MonoBehaviour 
{
	InputManager inputs; // Button inputs

	// TODO: Figure out XML files?
	public PlayerData data; // The XML file which contains all player data

	PlayerState state;

	bool facingRight = true; // Is the player facing right?

	// TODO: Delete this, used to print input history
	string history = "";
	List<CommandNode> commands;
	Command testCommand;

	// Use this for initialization
	void Start () 
	{
		inputs = GetComponent<InputManager>();

		data = new PlayerData("Assets/XMLs/place_holder.xml");
	}

	// TODO: May be not be flipping properly? Check this out.
	void FixedUpdate ()
	{
		if(inputs.CurrentDirection() == Key.DOWNBACK ||
		   inputs.CurrentDirection() == Key.BACK ||
		   inputs.CurrentDirection() == Key.UPBACK)
			Flip ();

		float move = 0;
		if(inputs.CurrentDirection() == Key.FORWARD)
		{
			move = data.WalkSpeed;
		}

		if(!FacingRight())
		{
			move *= -1;
		}
		
		rigidbody2D.velocity = new Vector2( move, rigidbody2D.velocity.y );
	}
	
	// Update is called once per frame
	void Update ()
	{
		string s = inputs.InputHistory();
		if(!s.Equals (history))
		{
			history = s;
			//Debug.Log (history);
		}
	}
	
	// TODO: Get this working properly
	/*string MatchCommand()
	{
		string button = "";
		string command;
		bool cleanInput = false;
		if(inputs.IsLastInputAButton ())
		{
			button = inputs.GetLastButton ().Value.GetButton ().ToString();
			cleanInput = true;
		}
		command = "5" + button;
		for(int i = 0; i < data.Commands.Count; i++)
		{
			if(inputs.MatchDirectionInput(data.Commands[i]))
			{
				string candidate = data.Commands[i].ToString () + button;
				if(data.MoveSet.Contains (candidate))
				{
					command = candidate;
					Debug.Log (candidate);
					break;
				}
			}
		}
		if(cleanInput)
			inputs.cleanDirectionInputs();
		return command;
	}*/

	void Flip()
	{
		facingRight = !facingRight;
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	public bool FacingRight()
	{
		return facingRight;
	}
}

// TODO: Get this to work
public class PlayerState
{
	private bool grounded; // True - Player is on the ground, False - Player is floating
	private bool crouching; // True - Player is crouching, False - Player is standing
	private int stun; // frames until player is able to take action
	private int airActions; // current number of air actions
	private int maxAirActions;

	public PlayerState(bool grounded, int maxAirActions)
	{
		Grounded = grounded;
		Crouching = false;
		Stun = 0;
		AirActions = maxAirActions;
		MaxAirActions =  maxAirActions;
	}

	public bool Grounded
	{
		get { return grounded; }
		set { grounded = value; }
	}

	public bool Crouching
	{
		get { return crouching; }
		set { crouching = value; }
	}

	public int Stun
	{
		get { return stun; }
		set { stun = value; }
	}

	public int AirActions
	{
		get { return airActions; }
		set { airActions = value; }
	}

	public int MaxAirActions
	{
		get { return maxAirActions; }
		set { maxAirActions = value; }
	}

	public void ResetAirActions()
	{
		AirActions = MaxAirActions;
	}
}












