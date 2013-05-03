using UnityEngine;
using System.Collections;

public class Character : MonoBehaviour
{
	public UISprite sprite;
	public CharacterInput input;
	
	#region Motion Info
	
	public float movementForce;
	public float movementMax;
	public float jumpSpeed;
	
	private float movementMagnitude;
	private bool isGrounded = false;
	
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
	}
	
	private void JumpPressed(bool pressed)
	{
		if (pressed)
		{
			Vector3 velocity = rigidbody.velocity;
			velocity.y = jumpSpeed;
			rigidbody.velocity = velocity;
		}
	}
	
	private void OnCollisionEnter(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.GetComponent<LevelPlatform>() != null)
		{
			isGrounded = true;
		}
	}
	
	private void OnCollisionExit(Collision collisionInfo)
	{
		if (collisionInfo.gameObject.GetComponent<LevelPlatform>() != null)
		{
			isGrounded = false;
		}
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
