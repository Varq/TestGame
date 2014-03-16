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
	PlayerData data; // The XML file which contains all player data

	PlayerState state;

	bool facingRight = true; // Is the player facing right?

	// Connect to other components
	void Start () 
	{
		inputs = GetComponent<InputManager>();
		data = GetComponent<PlayerData>();
		state = GetComponent<PlayerState>();
	}

	void FixedUpdate ()
	{
		Act ();
	}

	void Update ()
	{
		NextAction ();
	}
	
	// Debug strings
	string s2 = "";
	void NextAction()
	{
		List<Move> moves = inputs.RecentMove ();
		string str = "";
		for(int i = 0; i < moves.Count; i++)
		{
			str += moves[i] + " ";
		}
		
		if(!s2.Equals (str))
		{
			s2 = str;
			Debug.Log (s2);
		}
	}

	void Act()
	{
		List<Move> moves = inputs.RecentMove ();
		// TODO: May be not be flipping properly? Check this out.
		if(inputs.CurrentDirection() == Key.DOWNBACK ||
		   inputs.CurrentDirection() == Key.BACK ||
		   inputs.CurrentDirection() == Key.UPBACK)
			Flip ();

		float moveX = 0;
		float moveY = 0;
		if(inputs.CurrentDirection() == Key.FORWARD)
		{
			moveX = data.WalkSpeed;
		}

		if((inputs.CurrentDirection() == Key.UP ||
		   inputs.CurrentDirection () == Key.UPBACK ||
		   inputs.CurrentDirection () == Key.UPFORWARD) &&
		   state.AirActions > 0)
		{
			Debug.Log ("Jump");
			moveY = data.JumpForce;
			state.AirActions--;
		}
		else
		{
			moveY = rigidbody2D.velocity.y;
		}

		if(!FacingRight())
		{
			moveX *= -1;
		}
		
		rigidbody2D.velocity = new Vector2( moveX, moveY );
	}

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













