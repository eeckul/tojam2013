using UnityEngine;
using System.Collections;

public class Player : Character
{
	public CharacterInput input;
	
	#region Input Info
	
	private bool isHoldingDown;
	private bool allowDoubleJump = true;
	
	#endregion
	
	#region Interaction Info
	
	private bool isInteracting;
	private LevelInteractive currentInteractive;
	
	#endregion
	
	protected override void Start()
	{
		base.Start();
		
		GameRoot.current.players.Add(this);
		
		input.OnAPress += OnAPress;
	}
	
	protected override void FixedUpdate()
	{
		UpdateInput();
		
		base.FixedUpdate();
		
		UpdateInteraction();
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
		
		if (isGrounded)
		{
			allowDoubleJump = true;
		}
	}
	
	private void OnAPress(bool pressed)
	{
		if (pressed)
		{
			if (isInteracting)
			{
				
			}
			else if (isGrounded && isHoldingDown)
			{
				if (IsPlatformDownJumpable(currentPlatform))
				{
					isDownJumping = true;
					downJumpingPlatform = currentPlatform;
				}
			}
			else if (isGrounded || allowDoubleJump)
			{
				allowDoubleJump = false;
				TriggerJump();
			}
		}
	}
	
	#endregion
	
	#region Interaction
	
	private void UpdateInteraction()
	{
		if (isInteracting)
		{
			ToggleCurrentInteractive(true);
		}
	}
	
	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		
		LevelInteractive interactive = other.gameObject.GetComponent<LevelInteractive>();
		if (interactive != null)
		{
			isInteracting = true;
			currentInteractive = interactive;
		}
	}
	
	protected override void OnTriggerExit(Collider other)
	{
		base.OnTriggerExit(other);
		
		LevelInteractive interactive = other.gameObject.GetComponent<LevelInteractive>();
		if (interactive != null)
		{
			if (currentInteractive == interactive)
			{
				ToggleCurrentInteractive(false);
				isInteracting = false;
				currentInteractive = null;
			}
		}
	}
	
	private void ToggleCurrentInteractive(bool toggle)
	{
		InteractiveTerminal terminal = currentInteractive.GetComponent<InteractiveTerminal>();
		if (terminal != null)
		{
			terminal.ToggleButton(toggle);
		}
	}
	
	private void ActivateInteractive()
	{
		InteractiveTerminal terminal = currentInteractive.GetComponent<InteractiveTerminal>();
		if (terminal != null)
		{
			terminal.Activate();
		}
	}
	
	#endregion
}
