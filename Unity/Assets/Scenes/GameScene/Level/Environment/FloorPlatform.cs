using UnityEngine;
using System.Collections;

public class FloorPlatform : LevelFloor
{
	private void Start()
	{
		Vector3 center = boxCollider.center;
		center.y = 11.5f;
		boxCollider.center = center;
		
		Vector3 size = boxCollider.size;
		size.y = 5f;
		boxCollider.size = size;
	}
}
