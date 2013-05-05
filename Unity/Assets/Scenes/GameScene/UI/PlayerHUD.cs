using UnityEngine;
using System.Collections;

public class PlayerHUD : MonoBehaviour
{
	public UISprite healthSprite;
	public float minFill = 0.26f, maxFill = 0.76f;
	
	public void SetHealth(float percentHealth)
	{
		healthSprite.fillAmount = Mathf.Lerp(minFill, maxFill, percentHealth);
	}
}
