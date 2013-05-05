using UnityEngine;
using System.Collections;

public class Enemy : Character
{
	public bool isEnabled;
		
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
		if (isEnabled)
		{
			UpdateAI();
		}
		
		base.FixedUpdate();
	}
	
	#region Input
	
	private void UpdateAI()
	{
		bool combatRange = false;
		
		if ( targetPlayer == -1 || GameRoot.current.players[targetPlayer].IsDead() )
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
			
			if ( isGrounded && jumpSpeed > 0 && (GameRoot.current.players[targetPlayer].transform.localPosition.y - transform.localPosition.y) > minJumpDistance && nextAttackState == AttackState.None)
			{
				TriggerJump();
			}
		}
		
		int directionMult = 1;
		
		if ( direction == Direction.Left )
		{
			directionMult = -1;
		}		
		
		if ( combatRange && isGrounded )
		{
			TriggerLightAttack();
		}
		
		Vector2 leftStickInput = new Vector2(movementSpeed * directionMult * 10, 0);
		
		if (leftStickInput.x == 0 || combatRange || IsDead () || nextAttackState != AttackState.None )
		{
			movementMagnitude = 0;
		}
		else if (combatRange && targetPlayer != -1)
		{
			transform.localScale = new Vector3(directionMult > 0 ? 1 : -1, 1, 1);
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
			if ( (minDistance == -1 || minDistance > currentDistance) && !GameRoot.current.players[i].IsDead() )
			{
				minPlayer = i;
				minDistance = currentDistance;
			}
		}
		
		return minPlayer;
	}
	
	#endregion
	
	#region Combat

	protected override void HandleAggro(Character attacker)
	{
		if (attacker.characterType == CharacterType.Player)
		{
			Player player = (Player)attacker;
			int controllerNum = player.input.controller;
			
			for (int i = 0; i < GameRoot.current.players.size; i++)
			{
				if (GameRoot.current.players[i].input.controller == controllerNum)
				{
					targetPlayer = i;
				}
			}
		}
	}
	
	#endregion
}
