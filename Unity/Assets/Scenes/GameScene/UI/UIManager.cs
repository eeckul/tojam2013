using UnityEngine;
using System.Collections;

public class UIManager : MonoBehaviour
{
	public static UIManager current;
	
	public UISprite nextScreenArrow;
	
	private void Awake()
	{
		current = this;
	}
	
	private void Start()
	{
		ToggleNextScreenArrow(false);
	}
	
	public void ToggleNextScreenArrow(bool toggle)
	{
		if (toggle)
		{
			TweenAlpha tweenAlpha = nextScreenArrow.gameObject.GetComponent<TweenAlpha>();
			tweenAlpha.Play(true);
			tweenAlpha.Reset();
		}
		
		NGUITools.SetActive(nextScreenArrow.gameObject, toggle);
	}
}
