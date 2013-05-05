using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	public GameObject levelRoot;
	public GameObject enemiesRoot;
	
	public UISprite backgroundSprite;
	
	public Player[] playerObjects;
	private BetterList<Player> spawnPlayerList = new BetterList<Player>();
	
	public Texture2D levelMap;
	private int width;
	private int height;
	private Color[] colors;
	
	public GameObject groundPrefab;
	public Color groundColor = RGBColor(0, 0, 0);
	
	public GameObject platformShortPrefab;
	public Color platformShortColor = RGBColor(0, 0, 64);
	public GameObject platformShortMediumPrefab;
	public Color platformShortMediumColor = RGBColor(192, 0, 0);
	public GameObject platformMediumPrefab;
	public Color platformMediumColor = RGBColor(0, 0, 128);
	public GameObject platformLongPrefab;
	public Color platformLongColor = RGBColor(0, 0, 192);
	
	public GameObject enemyPrefab1;
	public Color enemyColor1 = RGBColor(0, 64, 0);
	public GameObject enemyPrefab2;
	public Color enemyColor2 = RGBColor(0, 128, 0);
	public GameObject enemyPrefab3;
	public Color enemyColor3 = RGBColor(0, 192, 0);
	
	public GameObject terminalAPrefab;
	public Color terminalAColor = RGBColor(64, 64, 0);
	public GameObject terminalBPrefab;
	public Color terminalBColor = RGBColor(128, 128, 0);
	public GameObject terminalXPrefab;
	public Color terminalXColor = RGBColor(0, 64, 64);
	public GameObject terminalYPrefab;
	public Color terminalYColor = RGBColor(0, 128, 128);
	
	public GameObject doorPrefab1;
	public Color doorColor1 = RGBColor(64, 0, 0);
	public GameObject doorPrefab2;
	public Color doorColor2 = RGBColor(128, 0, 0);
	
	public Color spawnColor = RGBColor(192, 192, 0);
	
	private void Awake()
	{
		foreach (Player playerObject in playerObjects)
		{
			spawnPlayerList.Add(playerObject);
		}
		
		if (GameRoot.nextLevelIndex != 0)
		{
			levelMap = Resources.Load("level_" + GameRoot.nextLevelIndex.ToString()) as Texture2D;
		}
		
		width = levelMap.width;
		height = levelMap.height;
		colors = levelMap.GetPixels();
		
		GroundPass();
		ObjectPass();
	}
	
	private static Color RGBColor(int r, int g, int b)
	{
		return new Color((float)r / 255f, (float)g / 255f, (float)b / 255f);
	}
	
	private void GroundPass()
	{
		int blockStart = 0;
		int blockWidth = 0;
		int blockHeight = 0;
		
		for (int w = 0; w < width; w++)
		{
			int segmentHeight = 0;
			
			for (int h = 0; h < height; h++)
			{
				if (colors[w + h * width] == groundColor)
				{
					segmentHeight++;
				}
				else
				{
					if (blockHeight > 0 && segmentHeight != blockHeight)
					{
						CreateGroundCollider(blockStart, blockWidth, blockHeight);
						blockStart = w;
						blockWidth = 0;
					}
					
					blockHeight = segmentHeight;
					blockWidth++;
					break;
				}				
			}
			
			if (w == width - 1)
			{
				CreateGroundCollider(blockStart, blockWidth, blockHeight);
			}
		}
		
		Vector3 backgroundScale = backgroundSprite.transform.localScale;
		backgroundScale.x = (float)width;
		backgroundSprite.transform.localScale = backgroundScale;
	}
	
	private void ObjectPass()
	{
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				Color color = colors[w + h * width];
				
				CreateObject(color, platformShortColor, platformShortPrefab, levelRoot, w, h, true);
				CreateObject(color, platformShortMediumColor, platformShortMediumPrefab, levelRoot, w, h, true);
				CreateObject(color, platformMediumColor, platformMediumPrefab, levelRoot, w, h, true);
				CreateObject(color, platformLongColor, platformLongPrefab, levelRoot, w, h, true);
				
				CreateObject(color, enemyColor1, enemyPrefab1, enemiesRoot, w, h, false);
				CreateObject(color, enemyColor2, enemyPrefab2, enemiesRoot, w, h, false);
				
				CreateObject(color, terminalAColor, terminalAPrefab, levelRoot, w, h);
				CreateObject(color, terminalBColor, terminalBPrefab, levelRoot, w, h);
				CreateObject(color, terminalXColor, terminalXPrefab, levelRoot, w, h);
				CreateObject(color, terminalYColor, terminalYPrefab, levelRoot, w, h);
				
				CreateObject(color, doorColor1, doorPrefab1, levelRoot, w, h);
				CreateObject(color, doorColor2, doorPrefab2, levelRoot, w, h);
				
				SpawnPlayer(color, spawnColor, w, h);
			}
		}
	}
	
	private void CreateGroundCollider(int blockStart, int blockWidth, int blockHeight)
	{
		LevelFloor ground = NGUITools.AddChild(levelRoot, groundPrefab).GetComponent<LevelFloor>();
		ground.boxCollider.size = new Vector3(blockWidth, blockHeight, 50f);
		ground.boxCollider.center = new Vector3(blockWidth * 0.5f, -blockHeight * 0.5f, 0);
		ground.transform.localPosition = new Vector3(blockStart, blockHeight, 0);
	}
	
	private void CreateObject(Color pixelColor, Color objectColor, GameObject prefab, GameObject rootObject, int x, int y, bool topAnchor = false)
	{
		if (pixelColor == objectColor)
		{
			Debug.Log("Creating " + prefab.name + " at (" + x + ", " + y + ")");
			
			GameObject prefabObject = NGUITools.AddChild(rootObject, prefab);
			BoxCollider prefabCollider = prefabObject.GetComponentInChildren<BoxCollider>();
			prefabObject.transform.localPosition = new Vector3(x + prefabCollider.size.x * 0.5f,
				(topAnchor ? y - prefabCollider.size.y * 0.5f : y + prefabCollider.size.y * 0.5f),
				0);
		}
	}
	
	private void SpawnPlayer(Color pixelColor, Color spawnColor, int x, int y)
	{
		if (pixelColor == spawnColor)
		{
			int randomIndex = UnityEngine.Random.Range(0, spawnPlayerList.size);
			Player randomPlayer = spawnPlayerList[randomIndex];
			spawnPlayerList.RemoveAt(randomIndex);
			
			Debug.Log("Spawning " + randomPlayer.name + " at (" + x + ", " + y + ")");
			
			Vector3 spawnPosition = randomPlayer.transform.localPosition;
			spawnPosition.x = x + randomPlayer.boxCollider.size.x * 0.5f;
			spawnPosition.y = y + randomPlayer.boxCollider.size.y * 0.5f;
			randomPlayer.transform.localPosition = spawnPosition;
		}
	}
}
