// Last Updated 2014_03_09

using UnityEngine;
using System.Collections.Generic;
using GameData;

public class InputManager : MonoBehaviour
{
	
	PlayerController control; // The controller
	
	DirectionState direction; // The player's current directions
	ButtonState[] buttons; //  The current state of the buttons

	LinkedList<Key> inputHistory; // The input history
	
	void Start ()
	{
		control = GetComponent<PlayerController>();
		SetUpDirection ();
		SetUpButtons ();
		SetUpInputHistory();
	}

	void Update ()
	{
		UpdateDirection ();
		UpdateButtons ();
		UpdatePossibleCommands();
	}

	void SetUpDirection()
	{
		direction = new DirectionState();
		UpdateDirection ();
	}

	void SetUpButtons()
	{
		// Create an array of buttons
		buttons = new ButtonState[Constants.NUM_BUTTONS];
		buttons[0] = new ButtonState(Key.A);
		buttons[0].Release();
		buttons[1] = new ButtonState(Key.B);
		buttons[1].Release();
		buttons[2] = new ButtonState(Key.C);
		buttons[2].Release();
		buttons[3] = new ButtonState(Key.D);
		buttons[3].Release();
	}
	
	void UpdateButtons()
	{
		for(int i = 0; i < Constants.NUM_BUTTONS; i++)
		{
			if(Input.GetButtonDown (buttons[i].GetKey ().ToString ()))
				if(buttons[i].Press())
					AddToInputHistory (buttons[i].GetKey());
		}
		
		for(int i = 0; i < Constants.NUM_BUTTONS; i++)
		{
			if(Input.GetButtonUp (buttons[i].GetKey ().ToString ()))
				buttons[i].Release();
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

	// Debug string
	string s = "";
	public void UpdatePossibleCommands()
	{
		// Debug string
		string str = "";
		// Go through all commands
		for(int i = 0; i < control.data.Commands.Count; i++)
		{
			// If command is active
			if(control.data.Commands[i].IsActive ())
			{
				// Check key type
				switch (control.data.Commands[i][control.data.Commands[i].Count - 1].GetKeyType ())
				{
					// NB_NORMAL: Check if the key matches with the last command, if not, clear the command
				case KeyType.NB_NORMAL:
					if(direction.GetKey () != control.data.Commands[i][control.data.Commands[i].Count - 1].GetKey ())
					{
						control.data.Commands[i].Clear ();
					}
					break;
					// 
				case KeyType.NORMAL:
					// NORMAL: Check to see if the key matches with the last command and if the last command is within buffer range. If not, clear the command
					if(direction.GetKey () != control.data.Commands[i][control.data.Commands[i].Count - 1].GetKey () ||
					   !control.data.Commands[i].InBuffer (direction.GetTimePressed ()))
					{
						control.data.Commands[i].Clear ();
					}
					break;
				case KeyType.SKIP:
					if(!control.data.Commands[i].SkipKey(direction.GetKey ()) ||
					   !control.data.Commands[i].InBuffer (direction.GetTimePressed()))
					{
						control.data.Commands[i].Clear ();
					}
					break;
				}
			}
			else
			{
				switch(control.data.Commands[i][control.data.Commands[i].GetIndex()].GetKeyType ())
				{
				case KeyType.NB_NORMAL:
					if(direction.GetKey () == control.data.Commands[i][control.data.Commands[i].GetIndex ()].GetKey ())
					{
						control.data.Commands[i].SkipClear();
						control.data.Commands[i].AddSkipKey(direction.GetKey ());
						control.data.Commands[i].SetLastKey(direction.GetKey ());
						control.data.Commands[i].NextIndex ();
					}
					break;
				case KeyType.NORMAL:
					if(Time.time - direction.GetTimePressed () <= Constants.MAX_BUFFER_TIME &&
					   direction.GetKey () == control.data.Commands[i][control.data.Commands[i].GetIndex ()].GetKey ())
					{
						control.data.Commands[i].SkipClear();
						control.data.Commands[i].AddSkipKey(direction.GetKey ());
						control.data.Commands[i].SetLastKey(direction.GetKey ());
						control.data.Commands[i].NextIndex ();
					}
					else if(control.data.Commands[i].GetIndex() == 0 ||
					        !control.data.Commands[i].SkipKey(direction.GetKey ()) ||
					        !control.data.Commands[i].InBuffer (direction.GetTimePressed ()))
					{
						control.data.Commands[i].Clear ();
					}
					break;
				case KeyType.NORMAL_OR:
					// TODO: Make an OrLlist in GameEnums.Command? Last bookmark
					//while(control.data.Commands[i][control.data.Commands[i].GetIndex ()].GetKeyType () )
					break;
				case KeyType.SKIP:
					do
					{
						control.data.Commands[i].AddSkipKey(control.data.Commands[i][control.data.Commands[i].GetIndex()].GetKey ());
						control.data.Commands[i].NextIndex();
					}while(control.data.Commands[i].GetIndex () < control.data.Commands[i].Count &&
					       control.data.Commands[i][control.data.Commands[i].GetIndex()].GetKeyType () == KeyType.SKIP);

					if(!control.data.Commands[i].SkipKey(direction.GetKey ()) ||
					   !control.data.Commands[i].InBuffer (direction.GetTimePressed ()))
					{
						control.data.Commands[i].Clear ();
					}
					break;
				}

				if(control.data.Commands[i].GetIndex () >= control.data.Commands[i].Count)
				{
					control.data.Commands[i].Activate ();
				}
			}

			if(control.data.Commands[i].IsActive ())
				str += control.data.Commands[i] + " ";
		}

		// Debug string
		if(!str.Equals(s))
		{
			s = str;
			Debug.Log (s);
		}
	}

	public Key CurrentDirection()
	{
		return direction.GetKey ();
	}

	public string InputHistory()
	{
		string str = "";
		LinkedListNode<Key> key = inputHistory.First;
		while(key != null)
		{
			str += Methods.KeyToChar(key.Value) + " ";
			key = key.Next;
		}
		return str;
	}
}





















































