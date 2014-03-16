using UnityEngine;
using System;

// TODO: Get this to work
public class PlayerState : MonoBehaviour
{
	private bool grounded; // True - Player is on the ground, False - Player is floating
	private bool crouching; // True - Player is crouching, False - Player is standing
	private int stun; // frames until player is able to take action
	private int airActions; // current number of air actions
	private int maxAirActions;

	public LayerMask groundLayer;
	public Transform groundCheck;
	PlayerData data;
	
	void Start()
	{
		data = GetComponent<PlayerData> ();

		MaxAirActions = data.MaxAirActions;
		SetGrounded();
		Crouching = false;
		Stun = 0;
	}

	void FixedUpdate()
	{
		SetGrounded ();
	}

	public void SetGrounded()
	{
		if(Physics2D.OverlapPoint (groundCheck.position, groundLayer))
		{
			grounded = true;
			ResetAirActions();
		}
		else
		{
			grounded = false;
		}
	}

	public bool Grounded
	{
		get { return grounded; }
		set { grounded = value; }
	}
	
	public bool Floating
	{
		get { return !grounded; }
		set { grounded = !value; }
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

