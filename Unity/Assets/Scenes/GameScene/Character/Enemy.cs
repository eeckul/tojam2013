using UnityEngine;
using System.Collections;

public class Enemy : Character
{
	public float movementSpeed;
	public int minCombatDistance;
	public int minJumpDistance;
	
	private int targetPlayer;
	
	public enum Direction
	{
		Left,
		Right
	}
	
	public Direction direction;
	
	protected override void Start()
	{
		base.Start();
		
		GameRoot.current.enemies.Add(this);
		
		targetPlayer = -1;

	}
	
	protected override void FixedUpdate()
	{
		UpdateAI();
		
		base.FixedUpdate();
	}
	
	#region Input
	
	private void UpdateAI()
	{
		bool combatRange = false;
		
		if ( targetPlayer == -1 )
		{
			targetPlayer = FindClosestPlayer();
		}
		
		if ( targetPlayer != -1 )
		{
			if ( GameRoot.current.players[targetPlayer].transform.localPosition.x < transform.localPosition.x )
			{
				combatRange = (transform.localPosition.x - GameRoot.current.players[targetPlayer].transform.localPosition.x) < minCombatDistance;
				
				direction = Direction.Left;
			}
			else
			{
				combatRange = (GameRoot.current.players[targetPlayer].transform.localPosition.x - transform.localPosition.x) < minCombatDistance;
				
				direction = Direction.Right;
			}
			
			if ( isGrounded && jumpSpeed > 0 && (GameRoot.current.players[targetPlayer].transform.localPosition.y - transform.localPosition.y) > minJumpDistance )
			{
				TriggerJump();
			}
		}
		
		int directionMult = 1;
		
		if ( direction == Direction.Left )
		{
			directionMult = -1;
		}		
		
		Vector2 leftStickInput = new Vector2(movementSpeed * directionMult * 10, 0);
		
		if (leftStickInput.x == 0 || combatRange )
		{
			movementMagnitude = 0;
		}
		else
		{
			transform.localScale = new Vector3(leftStickInput.x > 0 ? 1 : -1, 1, 1);
			movementMagnitude = leftStickInput.x;
		}
	}
	
	private int FindClosestPlayer()
	{
		float minDistance = -1;
		int minPlayer = -1;
		for (int i = 0; i < GameRoot.current.players.size; i++)
		{
			float currentDistance = (transform.localPosition - GameRoot.current.players[i].transform.localPosition).sqrMagnitude;
			if ( minDistance == -1 || minDistance > currentDistance )
			{
				minPlayer = i;
				minDistance = currentDistance;
			}
		}
		
		return minPlayer;
	}
	
	#endregion
	
}
