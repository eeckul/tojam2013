using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
	public UISprite sprite;
	public BoxCollider boxCollider;
	
	#region Input & Motion Info
	
	public float movementForce;
	public float movementMax;
	public float jumpSpeed;
	public float knockbackSpeed;
	
	protected float movementMagnitude;
	
	protected bool isGrounded;
	protected LevelFloor currentPlatform;
	
	private bool isUpJumping;
	protected bool isDownJumping;
	protected LevelFloor downJumpingPlatform;
	
	protected bool isBlockedOnRight;
	public BetterList<GameObject> rightBlockingObjects = new BetterList<GameObject>();
	protected bool isBlockedOnLeft;
	public BetterList<GameObject> leftBlockingObjects = new BetterList<GameObject>();
	
	#endregion
	
	#region Animation Info
	
	public enum AnimationState
	{
		Standing,
		Running,
		Attack1,
		Attack2,
		Attack3,
		Attack4,
		Attack5,
		Block,
		KnockBack,
		Jump,
		Down,
		Win
	}
	
	public enum AttackState
	{
		None,
		Attack1,
		Attack2,
		Attack3,
		Attack4,
		Attack5,
		Block
	}
	
	public AnimationState animationState = AnimationState.Standing;
	private AnimationState currentState = AnimationState.Standing;
	
	protected AttackState nextAttackState = AttackState.None;
	public AttackState currentAttackState = AttackState.None;
	
	public string standingSpriteName;
	public int standingFrameCount;
	public float standingFrameRate;
	
	public string runningSpriteName;
	public int runningFrameCount;
	public float runningFrameRate;
	
	public string attack1SpriteName;
	public int attack1FrameCount;
	public float attack1FrameRate;
	
	public string attack2SpriteName;
	public int attack2FrameCount;
	public float attack2FrameRate;
	
	public string attack3SpriteName;
	public int attack3FrameCount;
	public float attack3FrameRate;
	
	public string attack4SpriteName;
	public int attack4FrameCount;
	public float attack4FrameRate;
	
	public string attack5SpriteName;
	public int attack5FrameCount;
	public float attack5FrameRate;
	
	public string blockSpriteName;
	public int blockFrameCount;
	public float blockFrameRate;
	
	public string knockBackSpriteName;
	public int knockBackFrameCount;
	public float knockBackFrameRate;
	
	public string jumpSpriteName;
	public int jumpFrameCount;
	public float jumpFrameRate;
	
	public string downSpriteName;
	public int downFrameCount;
	public float downFrameRate;
	
	public string winSpriteName;
	public int winFrameCount;
	public float winFrameRate;
	
	private int currentFrameCount;
	private float currentFrameInterval;
	private int currentFrameIndex;
	private float currentFrameTime;
	private int currentLoopCount;
	private bool currentAnimationIsAttack;
	
	private float knockbackTime;
	
	private float damageFlashTime;
	private float damageFlashRate;
	private float damageFlashTimeSinceLastFlash;
	
	private float deadTime;
	
	public enum VictoryState
	{
		None,
		Win,
		Lose
	}
	
	public VictoryState victoryState;
	
	public bool playInteractionAnimation;
	
	public bool requiresRevive;
		
	#endregion
	
	#region Combat Info
	
	public BoxCollider hitBox;
	
	public int maxHealth;
	public int currHealth;
	public int attack1Damage;
	public int attack2Damage;
	public int attack3Damage;
	public int attack4Damage;
	public int attack5Damage;
	public int lives;
	
	public enum CharacterType
	{
		Player,
		Enemy
	}
	
	public CharacterType characterType;
	
	protected int attackCount;
	
	#endregion
	
	protected virtual void Start()
	{
		currHealth = maxHealth;
		knockbackTime = 0.0f;;
		
		NGUITools.SetActive(hitBox.gameObject, false);
		
		SetAnimationInfo(standingFrameCount, standingFrameRate);
	}
	
	protected virtual void FixedUpdate()
	{
		UpdateMotion();
		
		UpdateAnimation();
		
		//update color flash
		if (damageFlashTime > 0)
		{
			damageFlashTime -= Time.deltaTime;
			if ( damageFlashTime < 0 )
			{
				damageFlashTime = 0;
			}
			damageFlashTimeSinceLastFlash += Time.deltaTime;
			
			while(damageFlashTimeSinceLastFlash > damageFlashRate)
			{
				damageFlashTimeSinceLastFlash -= damageFlashRate;
			}
			
			Color c = sprite.color;
			c.a = damageFlashTimeSinceLastFlash / damageFlashRate;
			sprite.color = c;
		}
		else if ( IsDead() )
		{
			if ( lives > 0 )
			{
				Respawn();
			}
			else if ( characterType == CharacterType.Enemy )
			{
				deadTime += Time.deltaTime;
				if ( deadTime > 5.0f )
				{
					deadTime = 5.0f;
				}
				Color c = sprite.color;
				c.a = 1.0f - (deadTime / 5.0f);
				sprite.color = c;
			}
			else
			{
				requiresRevive = true;
				Color c = sprite.color;
				c.a = 1.0f;
				sprite.color = c;
			}
		}
		else
		{		
			Color c = sprite.color;
			c.a = 1.0f;
			sprite.color = c;
		}
	}
	
	#region Motion
	
	private void UpdateMotion()
	{
		if (IsDead())
		{
			animationState = AnimationState.Down;
		}
		else if (victoryState != VictoryState.None)
		{
			if ( victoryState == VictoryState.Win )
			{
				animationState = AnimationState.Win;
			}
			else
			{
				animationState = AnimationState.Down;
			}
		}
		else if (playInteractionAnimation)
		{
			animationState = AnimationState.Standing;	
		}
		else if (knockbackTime > 0.0f)
		{
			//clear attack state
			nextAttackState = AttackState.None;
			currentAttackState = AttackState.None;
			
			knockbackTime -= Time.deltaTime;
			
			if ( knockbackTime < 0.0f )
			{
				knockbackTime = 0.0f;
			}
			
			animationState = AnimationState.KnockBack;
		}
		else if ( nextAttackState != AttackState.None )
		{
			switch ( nextAttackState )
			{
				case AttackState.Attack1:
				{
					switch ( currentAttackState )
					{
						case AttackState.Attack1:
						{
							if ( currentLoopCount > 0 )
							{
								attackCount --;
								if ( attackCount == 0 )
								{
									//end animation
									nextAttackState = AttackState.None;
									currentAttackState = AttackState.None;
								}
								else
								{
									nextAttackState = AttackState.Attack2;
								}
							}
							break;
						}
						default:
						{
							animationState = AnimationState.Attack1;
							currentAttackState = AttackState.Attack1;
							break;
						}					
					}
					break;
				}
				case AttackState.Attack2:
				{
					switch ( currentAttackState )
					{
						case AttackState.Attack2:
						{
							if ( currentLoopCount > 0 )
							{
								attackCount --;
								if ( attackCount == 0 )
								{
									//end animation
									nextAttackState = AttackState.None;
									currentAttackState = AttackState.None;
								}
								else
								{
									nextAttackState = AttackState.Attack3;
								}
							}
							break;
						}
						default:
						{
							animationState = AnimationState.Attack2;
							currentAttackState = AttackState.Attack2;
							break;
						}					
					}
					break;
				}
				case AttackState.Attack3:
				{
					switch ( currentAttackState )
					{
						case AttackState.Attack3:
						{
							if ( currentLoopCount > 0 )
							{
								attackCount --;
								if ( attackCount == 0 )
								{
									//end animation
									nextAttackState = AttackState.None;
									currentAttackState = AttackState.None;
								}
								else
								{
									nextAttackState = AttackState.Attack4;
								}
							}
							break;
						}
						default:
						{
							animationState = AnimationState.Attack3;
							currentAttackState = AttackState.Attack3;
							break;
						}					
					}
					break;
				}
				case AttackState.Attack4:
				{
					switch ( currentAttackState )
					{
						case AttackState.Attack4:
						{
							if ( currentLoopCount > 0 )
							{
								attackCount --;
								if ( attackCount == 0 )
								{
									//end animation
									nextAttackState = AttackState.None;
									currentAttackState = AttackState.None;
								}
								else
								{
									nextAttackState = AttackState.Attack5;
								}
							}
							break;
						}
						default:
						{
							animationState = AnimationState.Attack4;
							currentAttackState = AttackState.Attack4;
							break;
						}					
					}
					break;
				}
				case AttackState.Attack5:
				{
					switch ( currentAttackState )
					{
						case AttackState.Attack5:
						{
							attackCount = 0;
							if ( currentLoopCount > 0 )
							{
								//end animation
								nextAttackState = AttackState.None;
								currentAttackState = AttackState.None;
							}
							break;
						}
						default:
						{
							animationState = AnimationState.Attack5;
							currentAttackState = AttackState.Attack5;
							break;
						}					
					}
					break;
				}

				default:
				{
					//DO NOTHING
					break;
				}
			}
		}
		else
		{
			//clear attack state
			nextAttackState = AttackState.None;
			currentAttackState = AttackState.None;
			
			if (movementMagnitude == 0)
			{
				if (isGrounded)
				{
					Vector3 velocity = rigidbody.velocity;
					velocity.x = 0;
					rigidbody.velocity = velocity;
				}
			}
			else
			{
				float actualMovementForce = movementMagnitude * movementForce;
			
				if (actualMovementForce > 0 && rightBlockingObjects.size == 0
					|| actualMovementForce < 0 && leftBlockingObjects.size == 0)
				{
					rigidbody.AddForce(new Vector3(actualMovementForce, 0, 0));
				}
			}
			
			if (rigidbody.velocity.x != 0)
			{
				Vector3 velocity = rigidbody.velocity;
				velocity.x = Mathf.Clamp(velocity.x, -movementMax, movementMax);
				rigidbody.velocity = velocity;
			}
		
			if (isGrounded)
			{
				if (rigidbody.velocity.x != 0)
				{
					animationState = AnimationState.Running;
				}
				else
				{
					animationState = AnimationState.Standing;
				}
			}
			else
			{
				animationState = AnimationState.Jump;
			}
		
			isUpJumping = (rigidbody.velocity.y > jumpSpeed * 0.05f) && jumpSpeed > 0;
			ToggleJumpCollider(isUpJumping || isDownJumping);
		
			if (isDownJumping)
			{
				float characterY = rigidbody.position.y + boxCollider.size.y * 0.5f + boxCollider.center.y;
				BoxCollider platformCollider = (BoxCollider)downJumpingPlatform.collider;
				float platformY = downJumpingPlatform.transform.localPosition.y - platformCollider.size.y * 0.5f + platformCollider.center.y;
			
				if (characterY < platformY)
				{
					isDownJumping = false;
				}	
			}
		}
	}
	
	private void OnCollisionEnter(Collision collisionInfo)
	{
		foreach (ContactPoint contact in collisionInfo.contacts)
		{
			if (contact.normal.x < -0.9f)
			{
				rightBlockingObjects.Add(collisionInfo.gameObject);
			}
			
			if (contact.normal.x > 0.9f)
			{
				leftBlockingObjects.Add(collisionInfo.gameObject);
			}
		}
		
		LevelFloor platform = collisionInfo.gameObject.GetComponent<LevelFloor>();
		if (platform != null)
		{
			bool isUnderneath = false;
			
			foreach (ContactPoint contact in collisionInfo.contacts)
			{
				if (contact.normal.y > 0.9f)
				{
					isUnderneath = true;
				}
			}
			
			if (isUnderneath)
			{
				isGrounded = true;
				currentPlatform = platform;
			}
		}
	}
	
	private void OnCollisionExit(Collision collisionInfo)
	{
		while (rightBlockingObjects.Contains(collisionInfo.gameObject))
		{
			rightBlockingObjects.Remove(collisionInfo.gameObject);
		}
		
		while (leftBlockingObjects.Contains(collisionInfo.gameObject))
		{
			leftBlockingObjects.Remove(collisionInfo.gameObject);
		}
		
		LevelFloor platform = collisionInfo.gameObject.GetComponent<LevelFloor>();
		if (platform != null)
		{
			if (currentPlatform == platform)
			{
				isGrounded = false;
				currentPlatform = null;
			}
		}
	}
	
	protected virtual void OnTriggerEnter(Collider other)
	{
		HitBox otherHitBox = other.gameObject.GetComponent<HitBox>();
		if (otherHitBox != null && other != hitBox)
		{
			Hit(otherHitBox.owner);
		}
	}
	
	protected virtual void OnTriggerExit(Collider other)
	{
		HitBox otherHitBox = other.gameObject.GetComponent<HitBox>();
		if (otherHitBox != null && other != hitBox)
		{
			// Stub for leaving hit box.
		}
	}
	
	protected void TriggerJump()
	{
		if ( nextAttackState == AttackState.None && !IsDead() && knockbackTime == 0 )
		{
			Vector3 velocity = rigidbody.velocity;
			velocity.y = jumpSpeed;
			rigidbody.velocity = velocity;
		}
	}
	
	private void ToggleJumpCollider(bool enable)
	{
		Vector3 center = boxCollider.center;
		center.z = enable ? -26f : 0;
		boxCollider.center = center;
	}
	
	protected bool IsPlatformDownJumpable(LevelFloor platform)
	{
		BoxCollider platformCollider = (BoxCollider)platform.collider;
		return platformCollider.size.z <= 25f;
	}
	
	protected void TriggerPlayerAttack()
	{
		if ( nextAttackState == AttackState.None )
		{
			attackCount = 1;
			nextAttackState = AttackState.Attack1;
		}
	}
	
	protected void TriggerEnemyAttack()
	{
		if ( nextAttackState == AttackState.None )
		{
			attackCount = 1;
			nextAttackState = AttackState.Attack1;
		}
	}
	
	protected void TriggerKnockback(int direction)
	{
		Vector3 velocity = rigidbody.velocity;
		velocity.x = knockbackSpeed * direction;
		rigidbody.velocity = velocity;
		
		knockbackTime = 1.0f;
	}
	
	#endregion
	
	#region Animation
	
	private void UpdateAnimation()
	{
		bool changedState = false;
		if (currentState != animationState)
		{
			changedState = true;
			currentState = animationState;
		}
		
		switch (animationState)
		{
			case AnimationState.Standing:
			{
				if (changedState) SetAnimationInfo(standingFrameCount, standingFrameRate);
				Animate(standingSpriteName);
				break;
			}
			case AnimationState.Running:
			{
				if (changedState) SetAnimationInfo(runningFrameCount, runningFrameRate);
				Animate(runningSpriteName);
				break;
			}
			case AnimationState.Attack1:
			{
				if (changedState) SetAnimationInfo(attack1FrameCount, attack1FrameRate, true);
				Animate (attack1SpriteName);
				break;
			}
			case AnimationState.Attack2:
			{
				if (changedState) SetAnimationInfo(attack2FrameCount, attack2FrameRate, true);
				Animate (attack2SpriteName);
				break;
			}
			case AnimationState.Attack3:
			{
				if (changedState) SetAnimationInfo(attack3FrameCount, attack3FrameRate, true);
				Animate (attack3SpriteName);
				break;
			}
			case AnimationState.Attack4:
			{
				if (changedState) SetAnimationInfo(attack4FrameCount, attack4FrameRate, true);
				Animate (attack4SpriteName);
				break;
			}
			case AnimationState.Attack5:
			{
				if (changedState) SetAnimationInfo(attack5FrameCount, attack5FrameRate, true);
				Animate (attack5SpriteName);
				break;
			}
			case AnimationState.Block:
			{
				if (changedState) SetAnimationInfo(blockFrameCount, blockFrameRate);
				Animate (blockSpriteName);
				break;
			}
			case AnimationState.KnockBack:
			{
				if (changedState) SetAnimationInfo(knockBackFrameCount, knockBackFrameRate);
				Animate (knockBackSpriteName);
				break;
			}
			case AnimationState.Jump:
			{
				if (changedState) SetAnimationInfo(jumpFrameCount, jumpFrameRate);
				Animate (jumpSpriteName, false);
				break;
			}
			case AnimationState.Down:
			{
				if (changedState) SetAnimationInfo(downFrameCount, downFrameRate);
				Animate(downSpriteName);
				break;
			}
			case AnimationState.Win:
			{
				if (changedState) SetAnimationInfo(winFrameCount, winFrameRate);
				Animate(winSpriteName);
				break;
			}
			default:
			{
				break;
			}
		}
	}
	
	private void SetAnimationInfo(int frameCount, float frameRate, bool isAttack = false)
	{
		currentFrameCount = frameCount;
		currentFrameInterval = frameRate > 0 ? 1f / frameRate : 0;
		currentFrameIndex = 0;
		currentFrameTime = currentFrameInterval;
		currentLoopCount = -1;
		currentAnimationIsAttack = isAttack;
		
		NGUITools.SetActive(hitBox.gameObject, false);
	}
	
	private void Animate(string baseSpriteName, bool shouldLoopAnimation = true)
	{
		currentFrameTime += Time.deltaTime;
		
		if (currentFrameTime > currentFrameInterval)
		{
			string frameSpriteName = baseSpriteName + (currentFrameIndex + 1).ToString();
			sprite.spriteName = frameSpriteName;
			
			currentFrameTime = 0;
			
			if (shouldLoopAnimation)
			{
				currentFrameIndex = (currentFrameIndex + 1) % currentFrameCount;
			}
			else if (currentFrameIndex < currentFrameCount)
			{
				currentFrameIndex++;
			}
			else
			{
				currentLoopCount = 1;
			}
			
			if (shouldLoopAnimation && currentFrameIndex == 0)
			{
				currentLoopCount ++;
			}
		}
		else if (currentAnimationIsAttack)
		{
			NGUITools.SetActive(hitBox.gameObject, true);
		}
	}
	
	#endregion
	
	#region Combat
	
	public bool IsDead()
	{
		return currHealth == 0;
	}
	
	protected void Hit(Character attacker)
	{
		if ( characterType == CharacterType.Player || attacker.characterType == CharacterType.Player )
		{
			if ( IsDead() || damageFlashTime > 0 )
			{
				return;
			}		
		
			int damage = 0;
			switch (attacker.currentAttackState)
			{
				case AttackState.Attack1:
				{
					damage = attacker.attack1Damage;
					break;
				}
				case AttackState.Attack2:
				{
					damage = attacker.attack2Damage;
					break;
				}
				case AttackState.Attack3:
				{
					damage = attacker.attack3Damage;
					break;
				}
				case AttackState.Attack4:
				{
					damage = attacker.attack4Damage;
					break;
				}
				case AttackState.Attack5:
				{
					damage = attacker.attack5Damage;
					break;
				}
			}			
		
			if ( attacker.transform.localPosition.x < transform.localPosition.x )
			{
				TriggerKnockback(1);
			}
			else
			{
				TriggerKnockback(-1);
			}
		
			currHealth -= damage;
			if ( currHealth < 0 )
			{
				currHealth = 0;
			}
		
			damageFlashTime = 1.0f;
			damageFlashRate = 0.25f;
			damageFlashTimeSinceLastFlash = 0.0f;
			
			HandleAggro(attacker);
		}
	}
	
	protected virtual void HandleAggro(Character attacker)
	{
		
	}
	
	protected void Respawn()
	{
		if ( lives > 0 )
		{
			Vector3 playerPosition = transform.localPosition;
			playerPosition.y = 300;
			transform.localPosition = playerPosition;
			damageFlashTime = 4.0f;
			
			knockbackTime = 0;
			currHealth = maxHealth;
			lives--;
		}		
	}
	
	public void Revive()
	{
		if ( requiresRevive )
		{
			requiresRevive = false;
			lives ++;
			Respawn();
		}
	}
	
	#endregion
}
