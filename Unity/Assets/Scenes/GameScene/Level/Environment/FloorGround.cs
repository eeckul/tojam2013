using UnityEngine;
using System.Collections;

public class FloorGround : LevelFloor
{
	public UISprite sprite;
	
	private void Start()
	{
		Vector3 localScale = sprite.transform.localScale;
		localScale.x = boxCollider.size.x;
		sprite.transform.localScale = localScale;
	}
}
