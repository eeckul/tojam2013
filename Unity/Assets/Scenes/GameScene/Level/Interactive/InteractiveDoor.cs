using UnityEngine;
using System.Collections;

public class InteractiveDoor : LevelInteractive
{
	public enum DoorType
	{
		Enemy,
		Exit
	}
	
	public DoorType doorType;
	
	public bool isOpen;
	private float currentFactor = 1f;
	
	public string animationBaseName;
	public int animationFrameCount;
	public float animationFrameRate;
	
	private void Start()
	{
		Vector3 size = boxCollider.size;
		size.x *= 1.25f;
		size.y *= 1.25f;
		boxCollider.size = size;
	}
	
	private void Update()
	{
		if (isOpen && currentFactor > 0f)
		{
			float factorChangeRate = animationFrameCount > 0f ? animationFrameRate / animationFrameCount : 0f;
			currentFactor = Mathf.Clamp(currentFactor - factorChangeRate * Time.deltaTime, 0f, 1f);
			SetDoorStateByFactor(currentFactor);
		}
		else if (!isOpen && currentFactor < 1f)
		{
			float factorChangeRate = animationFrameCount > 0f ? animationFrameRate / animationFrameCount : 0f;
			currentFactor = Mathf.Clamp(currentFactor + factorChangeRate * Time.deltaTime, 0f, 1f);
			SetDoorStateByFactor(currentFactor);
		}
	}
	
	private void SetDoorStateByFactor(float factor)
	{
		int frame = Mathf.RoundToInt(factor * (animationFrameCount - 1));
		sprite.spriteName = animationBaseName + (frame + 1).ToString();
	}
}
