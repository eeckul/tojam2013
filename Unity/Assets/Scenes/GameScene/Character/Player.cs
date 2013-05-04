using UnityEngine;
using System.Collections;

public class Player : Character
{
	public CharacterInput input;
	
	#region Input Info
	
	private bool isHoldingDown;
	
	#endregion
	
	#region Interaction Info
	
	private bool isInteracting;
	private LevelInteractive currentInteractive;
	
	#endregion
	
	protected override void Start()
	{
		base.Start();
		
		input.OnAPress += OnAPress;
	}
	
	protected override void FixedUpdate()
	{
		UpdateInput();
		
		base.FixedUpdate();
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
	
	private void OnAPress(bool pressed)
	{
		if (pressed)
		{
			if (isInteracting)
			{
				
			}
			else if (isGrounded)
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
					TriggerJump();
				}
			}
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
