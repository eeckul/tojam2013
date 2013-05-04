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
	
	protected float movementMagnitude;
	
	protected bool isGrounded;
	protected LevelPlatform currentPlatform;
	
	private bool isUpJumping;
	protected bool isDownJumping;
	protected LevelPlatform downJumpingPlatform;
	
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
	
	protected virtual void Start()
	{
		SetAnimationInfo(standingFrameCount, standingFrameRate);
	}
	
	protected virtual void Update()
	{
		UpdateMotion();
		
		UpdateAnimation();
	}
	
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
	
	protected void TriggerJump()
	{
		Vector3 velocity = rigidbody.velocity;
		velocity.y = jumpSpeed;
		rigidbody.velocity = velocity;
	}
	
	private void ToggleJumpCollider(bool enable)
	{
		Vector3 center = boxCollider.center;
		center.z = enable ? -26f : 0;
		boxCollider.center = center;
	}
	
	protected bool IsPlatformDownJumpable(LevelPlatform platform)
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
}
