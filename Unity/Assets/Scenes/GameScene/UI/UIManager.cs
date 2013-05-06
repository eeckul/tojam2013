using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
	public static UIManager current;
	
	public UISprite nextScreenArrow;
	public UISprite goatsWin;
	public UISprite robotsWin;
	
	public UISprite storySprite;
	public UISprite controlsSprite;
	public UISprite tutorialSprite;
	
	private void Awake()
	{
		current = this;
	}
	
	private void Start()
	{
		ToggleNextScreenArrow(false);
		ToggleGoatsWin(false);
		ToggleRobotsWin(false);
		
		NGUITools.SetActive(storySprite.gameObject, GameRoot.current.isSaboteurStage);
		NGUITools.SetActive(controlsSprite.gameObject, GameRoot.current.isSaboteurStage);
		NGUITools.SetActive(tutorialSprite.gameObject, GameRoot.current.isSaboteurStage);
	}
	
	public void ToggleNextScreenArrow(bool toggle)
	{
		ToggleBlinkingSprite(toggle, nextScreenArrow);
	}
	
	public void ToggleGoatsWin(bool toggle)
	{
		ToggleBlinkingSprite(toggle, goatsWin);
	}
	
	public void ToggleRobotsWin(bool toggle)
	{
		ToggleBlinkingSprite(toggle, robotsWin);
	}
	
	private void ToggleBlinkingSprite(bool toggle, UISprite sprite)
	{
		if (toggle && !NGUITools.GetActive(sprite.gameObject))
		{
			TweenAlpha tweenAlpha = sprite.gameObject.GetComponent<TweenAlpha>();
			tweenAlpha.Play(true);
			tweenAlpha.Reset();
		}
		
		NGUITools.SetActive(sprite.gameObject, toggle);
	}
}
