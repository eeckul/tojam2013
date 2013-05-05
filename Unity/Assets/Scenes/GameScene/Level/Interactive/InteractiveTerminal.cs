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
	
	public UISprite buttonSprite;
	public TweenAlpha buttonTweenAlpha;
	
	public bool isActivated;
	public bool activatedCorrectly;
	
	private void Start()
	{
		ToggleButton(false);
	}
	
	public void ToggleButton(bool toggle)
	{
		if (toggle != NGUITools.GetActive(buttonSprite.gameObject))
		{
			buttonTweenAlpha.Play(true);
			buttonTweenAlpha.Reset();
			NGUITools.SetActive(buttonSprite.gameObject, toggle);
		}
	}
	
	public bool Activate(TerminalType activationType)
	{
		if (!isActivated)
		{
			isActivated = true;
			activatedCorrectly = activationType == terminalType;
			boxCollider.enabled = false;
			ToggleButton(false);
			Debug.Log(name + " activated correctly: " + activatedCorrectly);
			
			GameRoot.current.ActivatedLevelTerminal();
			return true;
		}
		
		return false;
	}
}
