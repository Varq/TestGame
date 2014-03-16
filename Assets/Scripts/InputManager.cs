// Last Updated 2014_03_09

using UnityEngine;
using System.Collections.Generic;
using GameData;

public class InputManager : MonoBehaviour
{
	
	PlayerController control; // The controller
	PlayerData data; // The data for the player
	
	DirectionState direction; // The player's current directions
	List<ButtonState> buttons; //  The current state of the buttons

	List<Command> recentCommands;
	List<Key> recentButtons; // The list of recently simultaneously pressed buttons

	LinkedList<Key> inputHistory; // The input history
	
	void Start ()
	{
		control = GetComponent<PlayerController>();
		data = GetComponent<PlayerData>();
		SetUpDirection ();
		SetUpButtons ();
		SetUpInputHistory();
		SetUpRecentCommands ();
		SetUpRecentButtons();
	}

	void Update ()
	{
		UpdateDirection();
		UpdateButtons();
		UpdateRecentCommands();
		UpdateRecentButtons();
	}

	// Set up the direction key, and update it to the currently pressed key
	void SetUpDirection()
	{
		direction = new DirectionState();
		UpdateDirection ();
	}

	// Set up an array of buttons
	void SetUpButtons()
	{
		// Create an array of buttons
		buttons = new List<ButtonState>();
		buttons.Add (new ButtonState(Key.A));
		buttons.Add (new ButtonState(Key.B));
		buttons.Add (new ButtonState(Key.C));
		buttons.Add (new ButtonState(Key.D));

		for(int i = 0; i < buttons.Count; i++)
		{
			buttons[i].Release();
		}
	}


	void UpdateButtons()
	{
		for(int i = 0; i < buttons.Count; i++)
		{
			if(Input.GetButtonDown (buttons[i].GetKey ().ToString ()))
				if(buttons[i].Press())
					AddToInputHistory (buttons[i].GetKey());
		}
		
		for(int i = 0; i < buttons.Count; i++)
		{
			if(Input.GetButtonUp (buttons[i].GetKey ().ToString ()))
				if(buttons[i].Release ())
					AddToInputHistory (buttons[i].GetKey());
		}
	}

	// TODO: I want it to snap to a new directions when I press left/rigth or up/down simultaneously
	void UpdateDirection()
	{
		Key d = direction.GetKey ();
		float horizontal = Input.GetAxisRaw ("Horizontal");
		float vertical = Input.GetAxisRaw ("Vertical");
		
		if(!control.FacingRight())
			horizontal *= -1;
		
		if(horizontal < 0 && vertical < 0)
			d = Key.DOWNBACK;
		else if(horizontal == 0 && vertical < 0)
			d = Key.DOWN;
		else if(horizontal > 0 && vertical < 0)
			d = Key.DOWNFORWARD;
		else if(horizontal < 0 && vertical == 0)
			d = Key.BACK;
		else if(horizontal > 0 && vertical == 0)
			d = Key.FORWARD;
		else if(horizontal < 0 && vertical > 0)
			d = Key.UPBACK;
		else if(horizontal == 0 && vertical > 0)
			d = Key.UP;
		else if(horizontal > 0 && vertical > 0)
			d = Key.UPFORWARD;
		else
			d = Key.NEUTRAL;

		if(direction.SetDirection (d))
			AddToInputHistory (direction.GetKey());
	}
	
	void SetUpInputHistory()
	{
		inputHistory = new LinkedList<Key>();
	}

	// TODO: Should I be including release keys to the history?
	void AddToInputHistory(Key key)
	{
		inputHistory.AddLast (key);
		if(inputHistory.Count > Constants.MAX_HISTORY)
			inputHistory.RemoveFirst ();
	}

	void SetUpRecentCommands()
	{
		recentCommands = new List<Command>();
	}

	void UpdateRecentCommands()
	{
		recentCommands.Clear ();

		// Go through all commands
		for(int i = 0; i < data.Commands.Count; i++)
		{
			// If command is active
			if(data.Commands[i].IsActive ())
			{
				// Check key type
				switch (data.Commands[i][data.Commands[i].Count - 1].GetKeyType ())
				{
				// NO_BUFFER: Check if the key matches with the last command, if not, clear the command
				case KeyType.NO_BUFFER:
					if(direction.GetKey () != data.Commands[i][data.Commands[i].Count - 1].GetKey ())
					{
						data.Commands[i].Clear ();
					}
					break;
				// NORMAL: Check to see if the key matches with the last command and if the last command is within buffer range. If not, clear the command
				case KeyType.NORMAL:
					if(direction.GetKey () != data.Commands[i][data.Commands[i].Count - 1].GetKey () ||
					   !data.Commands[i].InBuffer (direction.GetTimePressed ()))
					{
						data.Commands[i].Clear ();
					}
					break;
				// SKIP: Match the input with inputs in the skip list, if not match fails
				case KeyType.SKIP:
					if(!data.Commands[i].SkipKey(direction.GetKey ()) ||
					   !data.Commands[i].InBuffer (direction.GetTimePressed()))
					{
						data.Commands[i].Clear ();
					}
					break;
				// REPEAT: As long as the last two commands are being repeated, this stays active
				case KeyType.REPEAT:
					if(!data.Commands[i].InBuffer (direction.GetTimePressed ()) ||
					   (direction.GetKey () != data.Commands[i][data.Commands[i].Count - 1].GetKey () &&
					   direction.GetKey () != data.Commands[i][data.Commands[i].Count - 2].GetKey ()))
					{
						data.Commands[i].Clear ();
					}
					break;
				}
			}
			else
			{
				switch(data.Commands[i][data.Commands[i].GetIndex()].GetKeyType ())
				{
				case KeyType.NO_BUFFER:
					if(direction.GetKey () == data.Commands[i][data.Commands[i].GetIndex ()].GetKey ())
					{
						data.Commands[i].SkipClear();
						data.Commands[i].AddSkipKey(direction.GetKey ());
						data.Commands[i].SetLastKey(direction.GetKey ());
						data.Commands[i].NextIndex ();
					}
					break;
				case KeyType.NORMAL:
				case KeyType.REPEAT:
					if(data.Commands[i].InBuffer (direction.GetTimePressed ()) &&
					   direction.GetKey () == data.Commands[i][data.Commands[i].GetIndex ()].GetKey ())
					{
						data.Commands[i].SkipClear();
						data.Commands[i].AddSkipKey(direction.GetKey ());
						data.Commands[i].SetLastKey(direction.GetKey ());
						data.Commands[i].NextIndex ();
					}
					else if(data.Commands[i].GetIndex() == 0 ||
					        !data.Commands[i].SkipKey(direction.GetKey ()) ||
					        !data.Commands[i].InBuffer (direction.GetTimePressed ()))
					{
						data.Commands[i].Clear ();
					}
					break;
				case KeyType.NORMAL_OR:
					if(data.Commands[i][data.Commands[i].GetIndex () + 1].GetKeyType() == KeyType.NORMAL_OR)
					{
						do
						{
							data.Commands[i].AddOrKey(data.Commands[i][data.Commands[i].GetIndex()].GetKey ());
							data.Commands[i].NextIndex();
						} while(data.Commands[i][data.Commands[i].GetIndex () + 1].GetKeyType() == KeyType.NORMAL_OR);
					}

					if(data.Commands[i].InBuffer (direction.GetTimePressed ()) &&
					   data.Commands[i].ContainsOrKey(direction.GetKey ()))
					{
						data.Commands[i].SkipClear();
						data.Commands[i].AddSkipKey(direction.GetKey ());
						data.Commands[i].SetLastKey(direction.GetKey ());
						data.Commands[i].NextIndex ();
					}
					else if(data.Commands[i].GetIndex() == 0 ||
					        !data.Commands[i].SkipKey(direction.GetKey ()) ||
					        !data.Commands[i].InBuffer (direction.GetTimePressed ()))
					{
						data.Commands[i].Clear ();
					}
					break;
				case KeyType.SKIP:
					do
					{
						data.Commands[i].AddSkipKey(data.Commands[i][data.Commands[i].GetIndex()].GetKey ());
						data.Commands[i].NextIndex();
					}while(data.Commands[i].GetIndex () < data.Commands[i].Count &&
					       data.Commands[i][data.Commands[i].GetIndex()].GetKeyType () == KeyType.SKIP);

					if(!data.Commands[i].SkipKey(direction.GetKey ()) ||
					   !data.Commands[i].InBuffer (direction.GetTimePressed ()))
					{
						data.Commands[i].Clear ();
					}
					break;
				}

				if(data.Commands[i].GetIndex () >= data.Commands[i].Count)
				{
					data.Commands[i].Activate ();
				}
			}

			if(data.Commands[i].IsActive ())
			{
				recentCommands.Add(data.Commands[i]);
			}
		}
		
		recentCommands.Add (data.Commands[data.Commands.Count - 1]);
	}

	public Key CurrentDirection()
	{
		return direction.GetKey ();
	}

	void SetUpRecentButtons()
	{
		recentButtons = new List<Key>();
	}

	void UpdateRecentButtons()
	{
		recentButtons.Clear ();

		for(int i = 0; i < buttons.Count; i++)
		{
			if(buttons[i].GetHoldTime () > 0 && buttons[i].GetHoldTime () < Constants.SIMULTANEOUS_TIME)
			{
				recentButtons.Add (buttons[i].GetKey ());
			}
		}
	}


	// TODO: Get this to work
	public List<Move> RecentMove()
	{
		List<Move> recentMoves = new List<Move>();

		for(int i = 0; i < data.MoveSet.Count; i++)
		{
			if(recentCommands.Contains (data.MoveSet[i].Command))
			{
				bool addMove = true;
				for(int j = 0; j < data.MoveSet[i].Buttons.Count; j++)
				{
					if(!recentButtons.Contains(data.MoveSet[i].Buttons[j]))
					{
						addMove = false;
					}
				}

				if(addMove)
					recentMoves.Add (data.MoveSet[i]);
			}
		}
		return recentMoves;
	}

	public string InputHistory()
	{
		string str = "";
		LinkedListNode<Key> key = inputHistory.First;
		while(key != null)
		{
			str += key.Value + " ";
			key = key.Next;
		}
		return str;
	}
}





















































