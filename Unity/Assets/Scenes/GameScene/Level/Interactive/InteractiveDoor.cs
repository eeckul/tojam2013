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
	
	public bool isReady;
	
	public bool spawnedEnemies;
	public bool isSpawning;
	public int[] enemyCount = { 0, 0, 0 };
	
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
		
		if (isOpen && currentFactor == 0f && !spawnedEnemies)
		{
			StartCoroutine(SpawnEnemies());
		}
	}
	
	public bool IsFullyOpen
	{
		get
		{
			return isOpen && currentFactor == 0f;
		}
	}
	
	public bool IsFullyClosed
	{
		get
		{
			return !isOpen && currentFactor == 1f;
		}
	}
	
	private void SetDoorStateByFactor(float factor)
	{
		int frame = Mathf.RoundToInt(factor * (animationFrameCount - 1));
		sprite.spriteName = animationBaseName + (frame + 1).ToString();
	}
	
	private IEnumerator SpawnEnemies()
	{
		spawnedEnemies = true;
		isSpawning = true;
		
		for (int i = 0; i < enemyCount[0]; i++)
		{
			GameObject enemyObject = NGUITools.AddChild(GameRoot.current.enemiesRoot, GameRoot.current.enemyPrefab1);
			Vector3 enemyPosition = enemyObject.transform.localPosition;
			enemyPosition.x = transform.localPosition.x;
			enemyPosition.y = transform.localPosition.y;
			enemyObject.transform.localPosition = enemyPosition;
			yield return new WaitForSeconds(0.25f);
		}
		
		for (int i = 0; i < enemyCount[1]; i++)
		{
			GameObject enemyObject = NGUITools.AddChild(GameRoot.current.enemiesRoot, GameRoot.current.enemyPrefab2);
			Vector3 enemyPosition = enemyObject.transform.localPosition;
			enemyPosition.x = transform.localPosition.x;
			enemyPosition.y = transform.localPosition.y;
			enemyObject.transform.localPosition = enemyPosition;
			yield return new WaitForSeconds(0.25f);
		}
		
		for (int i = 0; i < enemyCount[2]; i++)
		{
			GameObject enemyObject = NGUITools.AddChild(GameRoot.current.enemiesRoot, GameRoot.current.enemyPrefab3);
			Vector3 enemyPosition = enemyObject.transform.localPosition;
			enemyPosition.x = transform.localPosition.x;
			enemyPosition.y = transform.localPosition.y;
			enemyObject.transform.localPosition = enemyPosition;
			yield return new WaitForSeconds(0.25f);
		}
		
		isSpawning = false;
	}
}
