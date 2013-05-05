using UnityEngine;
using System.Collections;

public class Player : Character
{
	public CharacterInput input;
	public PlayerHUD hud;
	public int playerIndex;
	
	public UISprite saboteurButton;
	
	#region Input Info
	
	private bool isHoldingDown;
	private bool allowDoubleJump = true;
	
	#endregion
	
	#region Interaction Info
	
	private bool isInteracting;
	private LevelInteractive currentInteractive;
	
	#endregion
	
	private void Awake()
	{
		NGUITools.SetActive(saboteurButton.gameObject, false);
	}
	
	protected override void Start()
	{
		base.Start();
		
		GameRoot.current.players.Add(this);
		
		input.OnAPress += OnAPress;
		input.OnBPress += OnBPress;
		input.OnXPress += OnXPress;
		input.OnYPress += OnYPress;
	}
	
	protected override void FixedUpdate()
	{
		UpdateInput();
		
		base.FixedUpdate();
		
		UpdateInteraction();
		
		hud.SetHealth((float)currHealth / (float)maxHealth);
		hud.SetLives(lives);
	}
	
	#region Input
	
	private void UpdateInput()
	{
		Vector2 leftStickInput = input.GetStick(CharacterInput.Stick.Left);
		
		if (leftStickInput.y < -0.5f)
		{
			ActivateReadyDoor();
		}
		
		if (leftStickInput.x == 0 || IsDead () || nextAttackState != AttackState.None)
		{
			movementMagnitude = 0;
		}
		else
		{
			transform.localScale = new Vector3(leftStickInput.x > 0 ? 1 : -1, 1, 1);
			saboteurButton.transform.localScale = new Vector3(leftStickInput.x > 0 ? 32 : -32, 32, 1);
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
			if (GameRoot.current.isSaboteurStage)
			{
				saboteurButton.spriteName = "DC_buttons_" + (GameRoot.saboteur == playerIndex ? "Y" : "A");
				NGUITools.SetActive(saboteurButton.gameObject, true);
			}
			else if (isInteracting)
			{
				ActivateInteractive(CharacterInput.Button.A);
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
				
				if (isGrounded)
				{
					AudioManager.current.PlaySound("Jump");
				}
				else
				{
					AudioManager.current.PlaySound("DoubleJump");
				}
			}
		}
		else
		{
			NGUITools.SetActive(saboteurButton.gameObject, false);
		}
	}
	
	private void OnBPress(bool pressed)
	{
		if (pressed)
		{
			if (GameRoot.current.isSaboteurStage)
			{
				saboteurButton.spriteName = "DC_buttons_" + (GameRoot.saboteur == playerIndex ? "X" : "B");
				NGUITools.SetActive(saboteurButton.gameObject, true);
			}
			else if (isInteracting)
			{
				ActivateInteractive(CharacterInput.Button.B);
			}
			else if (isGrounded && lives > 1)
			{
				for ( int i = 0; i < GameRoot.current.players.size; i++ )
				{
					if (GameRoot.current.players[i] != this && GameRoot.current.players[i].requiresRevive 
						&& (GameRoot.current.players[i].transform.localPosition - transform.localPosition).sqrMagnitude < 256)
					{
						lives--;
						GameRoot.current.players[i].Revive();
						break;
					}
				}
			}
		}
		else
		{
			NGUITools.SetActive(saboteurButton.gameObject, false);
		}
	}
	
	private void OnXPress(bool pressed)
	{
		if (pressed)
		{
			if (GameRoot.current.isSaboteurStage)
			{
				saboteurButton.spriteName = "DC_buttons_" + (GameRoot.saboteur == playerIndex ? "B" : "X");
				NGUITools.SetActive(saboteurButton.gameObject, true);
			}
			else if (isInteracting)
			{
				ActivateInteractive(CharacterInput.Button.X);
			}
			else if (isGrounded)
			{
				TriggerPlayerAttack();
			}
		}
		else
		{
			NGUITools.SetActive(saboteurButton.gameObject, false);
		}
	}
	
	private void OnYPress(bool pressed)
	{
		if (pressed)
		{
			if (GameRoot.current.isSaboteurStage)
			{
				saboteurButton.spriteName = "DC_buttons_" + (GameRoot.saboteur == playerIndex ? "A" : "Y");
				NGUITools.SetActive(saboteurButton.gameObject, true);
			}
			else if (isInteracting)
			{
				ActivateInteractive(CharacterInput.Button.Y);
			}
		}
		else
		{
			NGUITools.SetActive(saboteurButton.gameObject, false);
		}
	}
	
	private void OnRightBumperPress(bool pressed)
	{
		if (pressed && isGrounded)
		{
			TriggerPlayerAttack();	
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
		
		if (rightBlockingObjects.Contains(GameRoot.current.gameCamera.rightBoundary.gameObject))
		{
			GameRoot.current.screenTransitionReady = true;
		}
	}
	
	protected override void OnTriggerEnter(Collider other)
	{
		base.OnTriggerEnter(other);
		
		LevelInteractive interactive = other.gameObject.GetComponent<LevelInteractive>();
		if (interactive != null)
		{
			InteractiveTerminal terminal = interactive as InteractiveTerminal;
			InteractiveDoor door = interactive as InteractiveDoor;
			
			if (terminal != null ||
				door != null && door.doorType == InteractiveDoor.DoorType.Exit && door.isReady)
			{
				isInteracting = true;
				currentInteractive = interactive;
			}
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
		if (terminal != null && !terminal.isActivated)
		{
			terminal.ToggleButton(toggle);
		}
	}
	
	private void ActivateInteractive(CharacterInput.Button button)
	{
		InteractiveTerminal terminal = currentInteractive as InteractiveTerminal;
		if (terminal != null)
		{
			bool terminalActivated = false;
			
			switch (button)
			{
				case CharacterInput.Button.A:
				{
					terminalActivated = terminal.Activate(InteractiveTerminal.TerminalType.A);
					break;
				}
				case CharacterInput.Button.B:
				{
					terminalActivated = terminal.Activate(InteractiveTerminal.TerminalType.B);
					break;
				}
				case CharacterInput.Button.X:
				{
					terminalActivated = terminal.Activate(InteractiveTerminal.TerminalType.X);
					break;
				}
				case CharacterInput.Button.Y:
				{
					terminalActivated = terminal.Activate(InteractiveTerminal.TerminalType.Y);
					break;
				}
				default:
				{
					break;
				}
			}
			
			if (terminalActivated)
			{
				playInteractionAnimation = true;
				isInteracting = false;
			}
		}
	}
	
	private void ActivateReadyDoor()
	{
		InteractiveDoor door = currentInteractive as InteractiveDoor;
		if (door != null && door.isReady)
		{
			GameRoot.current.TriggerNextLevel();
		}
	}
	
	#endregion
}
