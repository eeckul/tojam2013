using UnityEngine;
using System.Collections;

public class PlayerHUD : MonoBehaviour
{
	public UISprite healthSprite;
	public float minFill = 0.26f, maxFill = 0.76f;
	
	public UISprite livesSprite;
	
	public void SetHealth(float percentHealth)
	{
		healthSprite.fillAmount = Mathf.Lerp(minFill, maxFill, percentHealth);
	}
	
	public void SetLives(int lives)
	{
		livesSprite.transform.localScale = new Vector3(32f, (float)lives * 32f, 1f);
	}
}
