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
	
	public void Activate()
	{
		
	}
}
