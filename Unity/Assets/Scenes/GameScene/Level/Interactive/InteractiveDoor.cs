using UnityEngine;
using System.Collections;

public class InteractiveDoor : LevelInteractive
{
	public enum DoorType
	{
		A,
		B
	}
	
	public DoorType doorType;
	
	private void Start()
	{
		Vector3 size = boxCollider.size;
		size.x *= 1.25f;
		size.y *= 1.25f;
		boxCollider.size = size;
	}
}
