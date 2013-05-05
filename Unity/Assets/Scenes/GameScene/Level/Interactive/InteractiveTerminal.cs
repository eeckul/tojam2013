using UnityEngine;
using System.Collections;

public class InteractiveTerminal : LevelInteractive
{
	public enum TerminalType
	{
		A,
		B,
		X,
		Y
	}
	
	public TerminalType terminalType;
	
	public enum TerminalState
	{
		Offline,
		WaitingInput,
		Rejected,
		Accepted
	}
	
	public TerminalState terminalState;
	
	public string offlineSpriteName = "sprite_computeroffline";
	public string inputSpriteName = "sprite_computerinput";
	public string acceptedSpriteName = "sprite_computeraccepted";
	public string rejectSpriteBaseName = "sprite_computerreject";
	
	public int rejectFrameCount = 2;
	public float rejectFrameRate = 4;
	private int rejectFrameIndex;
	private float rejectFrameTime;
	
	public UISprite buttonSprite;
	public TweenAlpha buttonTweenAlpha;
	
	public bool isActivated;
	public bool activatedCorrectly;
	
	private void Start()
	{
		ToggleButton(false);
		SetTerminalState(TerminalState.Offline);
	}
	
	private void Update()
	{
		switch (terminalState)
		{
			case TerminalState.Offline:
			{
				sprite.spriteName = offlineSpriteName;
				break;
			}
			case TerminalState.WaitingInput:
			{
				sprite.spriteName = inputSpriteName;
				break;
			}
			case TerminalState.Accepted:
			{
				sprite.spriteName = acceptedSpriteName;
				break;
			}
			case TerminalState.Rejected:
			{
				rejectFrameTime -= Time.deltaTime;
				if (rejectFrameTime <= 0f)
				{
					rejectFrameTime = 1f / rejectFrameRate;
					rejectFrameIndex = (rejectFrameIndex + 1) % rejectFrameCount;
					sprite.spriteName = rejectSpriteBaseName + (rejectFrameIndex + 1).ToString();
				}
				break;
			}
			default:
			{
				break;
			}
		}
	}
	
	public void ToggleButton(bool toggle)
	{
		if (toggle != NGUITools.GetActive(buttonSprite.gameObject))
		{
			if (toggle)
			{
				if (terminalState == TerminalState.WaitingInput)
				{
					buttonTweenAlpha.Play(true);
					buttonTweenAlpha.Reset();
					NGUITools.SetActive(buttonSprite.gameObject, true);
				}
			}
			else
			{
				NGUITools.SetActive(buttonSprite.gameObject, false);
			}
		}
	}
	
	public void SetTerminalState(TerminalState state)
	{
		terminalState = state;
	}
	
	public bool Activate(TerminalType activationType)
	{
		if (terminalState == TerminalState.WaitingInput && !isActivated)
		{
			isActivated = true;
			activatedCorrectly = activationType == terminalType;
			boxCollider.enabled = false;
			ToggleButton(false);
			SetTerminalState(TerminalState.Accepted);
			Debug.Log(name + " activated correctly: " + activatedCorrectly);
			return true;
		}
		
		return false;
	}
}
