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
		LightAttack,
		LightAttack2,
		HeavyAttack,
		HeavyAttackChain,
		Block,
		KnockBack,
		Jump,
		Down
	}
	
	public enum AttackState
	{
		None,
		LightAttack,
		LightAttack2,
		HeavyAttack,
		HeavyAttackChain
	}
	
	public AnimationState animationState = AnimationState.Standing;
	private AnimationState currentState = AnimationState.Standing;
	
	protected AttackState nextAttackState = AttackState.None;
	protected AttackState currentAttackState = AttackState.None;
	
	public string standingSpriteName;
	public int standingFrameCount;
	public float standingFrameRate;
	
	public string runningSpriteName;
	public int runningFrameCount;
	public float runningFrameRate;
	
	public string lightAttackSpriteName;
	public int lightAttackFrameCount;
	public float lightAttackFrameRate;
	
	public string lightAttack2SpriteName;
	public int lightAttack2FrameCount;
	public float lightAttack2FrameRate;
	
	public string heavyAttackSpriteName;
	public int heavyAttackFrameCount;
	public float heavyAttackFrameRate;
	
	public string heavyAttackChainSpriteName;
	public int heavyAttackChainFrameCount;
	public float heavyAttackChainFrameRate;
	
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
	
	#endregion
	
	#region Combat Info
	
	public BoxCollider hitBox;
	
	public int maxHealth;
	public int currHealth;
	public int damage;
	
	public enum CharacterType
	{
		Player,
		Enemy
	}
	
	public CharacterType characterType;
	
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
			damageFlashTimeSinceLastFlash += Time.deltaTime;
			
			while(damageFlashTimeSinceLastFlash > damageFlashRate)
			{
				damageFlashTimeSinceLastFlash -= damageFlashRate;
			}
			
			Color c = sprite.color;
			c.a = damageFlashTimeSinceLastFlash / damageFlashRate;
			sprite.color = c;
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
		else if (knockbackTime > 0.0f)
		{
			//clear attack state
			nextAttackState = AttackState.None;
			currentAttackState = AttackState.None;
			
			knockbackTime -= Time.deltaTime;
			animationState = AnimationState.KnockBack;
		}
		else if ( nextAttackState != AttackState.None )
		{
			switch ( nextAttackState )
			{
				case AttackState.LightAttack:
				{
					switch ( currentAttackState )
					{
						case AttackState.LightAttack:
						{
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
							animationState = AnimationState.LightAttack;
							currentAttackState = AttackState.LightAttack;
							break;
						}					
					}
					break;
				}
				case AttackState.LightAttack2:
				{
					switch ( currentAttackState )
					{
						case AttackState.LightAttack2:
						{
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
							animationState = AnimationState.LightAttack2;
							currentAttackState = AttackState.LightAttack2;
							break;
						}					
					}
					break;
				}
				case AttackState.HeavyAttack:
				{
					switch ( currentAttackState )
					{
						case AttackState.HeavyAttack:
						{
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
							animationState = AnimationState.HeavyAttack;
							currentAttackState = AttackState.HeavyAttack;
							break;
						}					
					}
					break;
				}
				case AttackState.HeavyAttackChain:
				{
					switch ( currentAttackState )
					{
						case AttackState.HeavyAttackChain:
						{
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
							animationState = AnimationState.HeavyAttackChain;
							currentAttackState = AttackState.HeavyAttackChain;
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
		
			isUpJumping = rigidbody.velocity.y > jumpSpeed * 0.05f;
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
		if ( nextAttackState == AttackState.None && !IsDead() )
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
	
	protected void TriggerLightAttack()
	{
		if ( nextAttackState == AttackState.None )
		{
			nextAttackState = AttackState.LightAttack;
		}
		else if ( nextAttackState == AttackState.LightAttack )
		{
			nextAttackState = AttackState.LightAttack2;
		}		
	}
	
	protected void TriggerHeavyAttack()
	{
		if ( nextAttackState == AttackState.None )
		{
			nextAttackState = AttackState.HeavyAttack;
		}
		else if ( nextAttackState == AttackState.LightAttack || nextAttackState == AttackState.LightAttack2 )
		{
			nextAttackState = AttackState.HeavyAttackChain;
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
			case AnimationState.LightAttack:
			{
				if (changedState) SetAnimationInfo(lightAttackFrameCount, lightAttackFrameRate, true);
				Animate (lightAttackSpriteName);
				break;
			}
			case AnimationState.LightAttack2:
			{
				if (changedState) SetAnimationInfo(lightAttack2FrameCount, lightAttack2FrameRate, true);
				Animate (lightAttack2SpriteName);
				break;
			}
			case AnimationState.HeavyAttack:
			{
				if (changedState) SetAnimationInfo(heavyAttackFrameCount, heavyAttackFrameRate, true);
				Animate (heavyAttackSpriteName);
				break;
			}
			case AnimationState.HeavyAttackChain:
			{
				if (changedState) SetAnimationInfo(heavyAttackChainFrameCount, heavyAttackChainFrameRate, true);
				Animate (heavyAttackChainSpriteName);
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
		
			int damage = attacker.damage;
		
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
	
	#endregion
}
