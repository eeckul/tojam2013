using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
	public static UIManager current;
	
	public UISprite nextScreenArrow;
	public UISprite goatsWin;
	public UISprite robotsWin;
	
	private void Awake()
	{
		current = this;
	}
	
	private void Start()
	{
		ToggleNextScreenArrow(false);
		ToggleGoatsWin(false);
		ToggleRobotsWin(false);
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
