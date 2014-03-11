// Last Updated 2014_03_09

using UnityEngine;
using System;
using System.Collections.Generic;

namespace GameData
{
	public static class Constants
	{
		public const int NUM_BUTTONS = 4; // The number of buttons in this game
		public const int MAX_HISTORY = 30; // Amount of commands to hold in input history
		public const float MAX_BUFFER_TIME = 0.2f; // Time to input commands in seconds
		public const float SIMULTANEOUS_TIME = 0.1f; // Time for buttons to be input in simultaneously
	}

	public static class Methods
	{
		public static char KeyToChar(Key k)
		{
			switch(k)
			{
			case Key.A:
				return 'A';
			case Key.B:
				return 'B';
			case Key.C:
				return 'C';
			case Key.D:
				return 'D';
			case Key.DOWNBACK:
				return '1';
			case Key.DOWN:
				return '2';
			case Key.DOWNFORWARD:
				return '3';
			case Key.BACK:
				return '4';
			case Key.NEUTRAL:
				return '5';
			case Key.FORWARD:
				return '6';
			case Key.UPBACK:
				return '7';
			case Key.UP:
				return '8';
			case Key.UPFORWARD:
				return '9';
			}
			return 'X';
		}
	}

	public enum Key
	{
		A,
		B,
		C,
		D,
		DOWNBACK,
		DOWN,
		DOWNFORWARD,
		BACK,
		NEUTRAL,
		FORWARD,
		UPBACK,
		UP,
		UPFORWARD
	};
	
	public enum KeyType
	{
		NORMAL, // Button is checked by buffer
		NB_NORMAL, // Button is checked without buffer
		NORMAL_OR, // At least one button must be present to be pass, MUST be multiple instances consecutively to work
		NB_NORMAL_OR, // NORMAL_OR without checking buffer
		SKIP, // Button can be input, but isn't necessary to pass
		SKIP_AND, // These buttons can be input in any order and it will not affect the pass, MUST be multiple instances consecutively to work
		CHARGE // Button must be held down for the charge threshold to pass, MUST not be the last input to work properly (TODO: Not yet implemented)
	};
	
	public abstract class KeyPress
	{
		protected Key key;
		protected float timePressed;

		public KeyPress()
		{
			timePressed = 0.0f;
		}

		public KeyPress(Key k, float t)
		{
			key = k;
			timePressed = t;
		}	
		
		public Key GetKey()
		{
			return key;
		}

		public float GetTimePressed()
		{
			return timePressed;
		}

		protected void SetTime()
		{
			timePressed = Time.time;
		}
	}
	
	public class DirectionState : KeyPress
	{
		public DirectionState()
		{
			key = Key.NEUTRAL;
		}

		// Set pressed to true and record the time pressed down
		public bool SetDirection(Key d)
		{
			if(key != d)
			{
				key = d;
				SetTime ();
				return true;
			}
			return false;
		}
		
		// Set pressed to false
		public void Release()
		{
			SetDirection(Key.NEUTRAL);
		}
	}
	
	public class ButtonState : KeyPress
	{
		private bool pressed;
		
		public ButtonState(Key k)
		{
			key = k;
			pressed = false;
		}
		
		// Return how long the currently pressed button has been held down
		public float GetHoldTime()
		{
			if(pressed)
				return Time.time - timePressed;
			return 0;
		}
		
		// Set pressed to true and record the time pressed down
		public bool Press()
		{
			if(!pressed)
			{
				SetTime();
				pressed = true;
				return true;
			}
			return false;
		}
		
		// Set pressed to false
		public bool Release()
		{
			if(pressed)
			{
				pressed = false;
				return true;
			}
			return false;
		}
	}
	
	public class CommandNode
	{
		private Key key;
		private KeyType type;
		
		public CommandNode(Key k, KeyType t)
		{
			key = k;
			type = t;
		}
		
		public Key GetKey()
		{
			return key;
		}
		
		public KeyType GetKeyType()
		{
			return type;
		}
	}
	
	public class Command
	{
		// TODO: Do I need command name?
		private string commandName;
		private List<CommandNode> command;
		private int index;
		private float timeActive;
		private List<Key> skipList;
		private Key lastKey;
		
		public Command(string name, List<CommandNode> commandNodes)
		{
			commandName = name;
			command = commandNodes;
			skipList = new List<Key>();
			Clear ();
		}
		
		public int Count
		{
			get { return command.Count; }
		}
		
		public CommandNode this[int i]
		{
			get { return command[i]; }
			set { command[i] = value; }
		}

		public int GetIndex()
		{
			return index;
		}
		
		public void NextIndex()
		{
			index++;
		}
		
		public override string ToString()
		{
			return commandName;
		}

		public bool IsActive()
		{
			if(GetIndex () >= Count)
				return true;
			return false;
		}
		
		public bool Activate()
		{
			if(index >= command.Count)
			{
				timeActive = Time.time;
				return true;
			}
			return false;
		}

		public bool InBuffer(float time)
		{
			if(Time.time - time > Constants.MAX_BUFFER_TIME)
				return false;
			return true;
		}
		
		public void Clear()
		{
			index = 0;
			timeActive = -100.0f;
			skipList.Clear ();
			lastKey = command[0].GetKey ();
		}

		public void SkipClear()
		{
			skipList.Clear ();
		}

		public void SetLastKey(Key k)
		{
			lastKey = k;
		}

		public Key GetLastKey()
		{
			return lastKey;
		}

		public void AddSkipKey(Key k)
		{
			skipList.Add (k);
		}

		public bool SkipKey(Key k)
		{
			// If last saved key doesn't equal current key
			if(lastKey != k)
			{
				// Remove the last saved key from the skip list
				skipList.Remove (lastKey);

				// If the skip list doesn't contain the new key, return false
				if(!skipList.Contains (k))
				{
					return false;
				}
			}

			// Set the new last key as the current key and return true
			lastKey = k;
			return true;
		}
	}
}

