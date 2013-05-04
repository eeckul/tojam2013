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
	protected LevelFloor currentPlatform;
	
	private bool isUpJumping;
	protected bool isDownJumping;
	protected LevelFloor downJumpingPlatform;
	
	protected bool isBlockedOnRight;
	protected BetterList<GameObject> rightBlockingObjects = new BetterList<GameObject>();
	protected bool isBlockedOnLeft;
	protected BetterList<GameObject> leftBlockingObjects = new BetterList<GameObject>();
	
	protected bool isOnCamera;
	
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
	
	protected virtual void FixedUpdate()
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
		foreach (ContactPoint contact in collisionInfo.contacts)
		{
			if (contact.normal.x == -1)
			{
				rightBlockingObjects.Add(collisionInfo.gameObject);
			}
			
			if (contact.normal.x == 1)
			{
				leftBlockingObjects.Add(collisionInfo.gameObject);
			}
		}
		
		LevelFloor platform = collisionInfo.gameObject.GetComponent<LevelFloor>();
		if (platform != null)
		{
			isGrounded = true;
			currentPlatform = platform;
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
		if (other.gameObject == GameRoot.current.gameCamera.gameObject)
		{
			isOnCamera = true;
		}
	}
	
	protected virtual void OnTriggerExit(Collider other)
	{
		if (other.gameObject == GameRoot.current.gameCamera.gameObject)
		{
			isOnCamera = false;
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
	
	protected bool IsPlatformDownJumpable(LevelFloor platform)
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
