using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
	public UISprite sprite;
	public CharacterInput input;
	public BoxCollider boxCollider;
	
	#region Motion Info
	
	public float movementForce;
	public float movementMax;
	public float jumpSpeed;
	
	private float movementMagnitude;
	private bool isHoldingDown;
	
	private bool isGrounded;
	private LevelPlatform currentPlatform;
	
	private bool isUpJumping;
	private bool isDownJumping;
	private LevelPlatform downJumpingPlatform;
	
	#endregion
	
	#region Animation Info
	
	public enum AnimationState
	{
		Standing,
		Running
	}
	
	public AnimationState animationState = AnimationState.Standing;
	private AnimationState currentState = AnimationState.Standing;
	
	public string standingSpriteName;
	public int standingFrameCount;
	public float standingFrameRate;
	
	public string runningSpriteName;
	public int runningFrameCount;
	public float runningFrameRate;
	
	private int currentFrameCount;
	private float currentFrameInterval;
	private int currentFrameIndex;
	private float currentFrameTime;
	
	#endregion
	
	#region Interaction Info
	
	private bool isInteracting;
	private LevelInteractive currentInteractive;
	
	#endregion
	
	private void Start()
	{
		input.OnAPress += JumpPressed;
		
		SetAnimationInfo(standingFrameCount, standingFrameRate);
	}
	
	private void Update()
	{
		UpdateInput();
		
		UpdateMotion();
		
		UpdateAnimation();
	}
	
	#region Input
	
	private void UpdateInput()
	{
		Vector2 leftStickInput = input.GetStick(CharacterInput.Stick.Left);
		
		if (leftStickInput.x == 0)
		{
			movementMagnitude = 0;
		}
		else
		{
			transform.localScale = new Vector3(leftStickInput.x > 0 ? 1 : -1, 1, 1);
			movementMagnitude = leftStickInput.x;
		}
		
		isHoldingDown = leftStickInput.y > 0.5f;
	}
	
	#endregion
	
	#region Motion
	
	private void UpdateMotion()
	{
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
			rigidbody.AddForce(new Vector3(movementMagnitude * movementForce, 0, 0));
		}
		
		if (rigidbody.velocity.x != 0)
		{
			Vector3 velocity = rigidbody.velocity;
			velocity.x = Mathf.Clamp(velocity.x, -movementMax, movementMax);
			rigidbody.velocity = velocity;
			animationState = AnimationState.Running;
		}
		else
		{
			animationState = AnimationState.Standing;
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
	
	private void JumpPressed(bool pressed)
	{
		if (pressed)
		{
			if (!isInteracting && isGrounded)
			{
				if (isHoldingDown)
				{
					if (IsPlatformDownJumpable(currentPlatform))
					{
						isDownJumping = true;
						downJumpingPlatform = currentPlatform;
					}
				}
				else
				{
					Vector3 velocity = rigidbody.velocity;
					velocity.y = jumpSpeed;
					rigidbody.velocity = velocity;
				}
			}
		}
	}
	
	private void OnCollisionEnter(Collision collisionInfo)
	{
		LevelPlatform platform = collisionInfo.gameObject.GetComponent<LevelPlatform>();
		if (platform != null)
		{
			isGrounded = true;
			currentPlatform = platform;
		}
	}
	
	private void OnCollisionExit(Collision collisionInfo)
	{
		LevelPlatform platform = collisionInfo.gameObject.GetComponent<LevelPlatform>();
		if (platform != null)
		{
			if (currentPlatform == platform)
			{
				isGrounded = false;
				currentPlatform = null;
			}
		}
	}
	
	private void ToggleJumpCollider(bool enable)
	{
		Vector3 center = boxCollider.center;
		center.z = enable ? -25f : 0;
		boxCollider.center = center;
	}
	
	private bool IsPlatformDownJumpable(LevelPlatform platform)
	{
		BoxCollider platformCollider = (BoxCollider)platform.collider;
		return platformCollider.size.z <= 25f;
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
			default:
			{
				break;
			}
		}
	}
	
	private void SetAnimationInfo(int frameCount, float frameRate)
	{
		currentFrameCount = frameCount;
		currentFrameInterval = frameRate > 0 ? 1f / frameRate : 0;
		currentFrameIndex = 0;
		currentFrameTime = currentFrameInterval;
	}
	
	private void Animate(string baseSpriteName)
	{
		currentFrameTime += Time.deltaTime;
		
		if (currentFrameTime > currentFrameInterval)
		{
			string frameSpriteName = baseSpriteName + (currentFrameIndex + 1).ToString();
			sprite.spriteName = frameSpriteName;
			
			currentFrameTime = 0;
			currentFrameIndex = (currentFrameIndex + 1) % (currentFrameCount + 1);
		}
	}
	
	#endregion
	
	#region Interaction
	
	private void OnTriggerEnter(Collider other)
	{
		LevelInteractive interactive = other.gameObject.GetComponent<LevelInteractive>();
		if (interactive != null)
		{
			isInteracting = true;
			currentInteractive = interactive;
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		LevelInteractive interactive = other.gameObject.GetComponent<LevelInteractive>();
		if (interactive != null)
		{
			if (currentInteractive == interactive)
			{
				isInteracting = false;
				currentInteractive = null;
			}
		}
	}
	
	#endregion
}
