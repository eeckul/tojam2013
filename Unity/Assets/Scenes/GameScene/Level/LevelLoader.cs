using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	public GameObject levelRoot;
	public GameObject enemiesRoot;
	
	public UISprite backgroundSprite;
	
	public Player[] playerObjects;
	private BetterList<Player> spawnPlayerList = new BetterList<Player>();
	
	private Texture2D levelMap;
	public Texture2D forceLevel;
	private int width;
	private int height;
	private Color[] colors;
	
	private bool spawnSaboteurFirst = true;
	
	public GameObject groundPrefab;
	public Color groundColor = RGBColor(0, 0, 0);
	
	public GameObject platformReallyShortPrefab;
	public Color platformReallyShortColor = RGBColor(0, 192, 192);
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
	
	public GameObject doorEnemyPrefab;
	public Color doorEnemyColor = RGBColor(64, 0, 0);
	public GameObject doorExitPrefab;
	public Color doorExitColor = RGBColor(128, 0, 0);
	
	public Color spawnColor = RGBColor(192, 192, 0);
	
	private void Awake()
	{
		foreach (Player playerObject in playerObjects)
		{
			spawnPlayerList.Add(playerObject);
		}
		
		if (GameRoot.nextLevelIndex == -1)
		{
			levelMap = Resources.Load("level_tut") as Texture2D;
		}
		else if (forceLevel != null)
		{
			levelMap = forceLevel;
		}
		else
		{
			levelMap = Resources.Load("level_" + GameRoot.nextLevelIndex.ToString()) as Texture2D;
			if (levelMap == null)
			{
				levelMap = Resources.Load("level_end") as Texture2D;
				GameRoot.current.isEndGame = true;
			}
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
				Color nextColor = (h + 1) < height ? colors[w + (h + 1) * width] : Color.white;
				
				CreateObject(color, platformReallyShortColor, platformReallyShortPrefab, levelRoot, w, h, true);
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
				
				CreateEnemyDoor(color, doorEnemyColor, nextColor, doorEnemyPrefab, levelRoot, w, h);
				CreateObject(color, doorExitColor, doorExitPrefab, levelRoot, w, h);
				
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
	
	private GameObject CreateObject(Color pixelColor, Color objectColor, GameObject prefab, GameObject rootObject, int x, int y, bool topAnchor = false)
	{
		if (pixelColor == objectColor)
		{
			Debug.Log("Creating " + prefab.name + " at (" + x + ", " + y + ")");
			
			GameObject prefabObject = NGUITools.AddChild(rootObject, prefab);
			BoxCollider prefabCollider = prefabObject.GetComponentInChildren<BoxCollider>();
			prefabObject.transform.localPosition = new Vector3(x + prefabCollider.size.x * 0.5f,
				(topAnchor ? y - prefabCollider.size.y * 0.5f : y + prefabCollider.size.y * 0.5f),
				0);
			
			return prefabObject;
		}
		
		return null;
	}
	
	private void CreateEnemyDoor(Color pixelColor, Color objectColor, Color nextColor, GameObject prefab, GameObject rootObject, int x, int y, bool topAnchor = false)
	{
		GameObject enemyDoorObject = CreateObject(pixelColor, objectColor, prefab, rootObject, x, y, topAnchor);
		
		if (enemyDoorObject != null && pixelColor == objectColor && nextColor != Color.white)
		{
			InteractiveDoor door = enemyDoorObject.GetComponent<InteractiveDoor>();
			if (door != null)
			{
				door.enemyCount[0] = Mathf.RoundToInt(nextColor.r * 255f);
				door.enemyCount[1] = Mathf.RoundToInt(nextColor.g * 255f);
				door.enemyCount[2] = Mathf.RoundToInt(nextColor.b * 255f);
			}
		}
	}
	
	private void SpawnPlayer(Color pixelColor, Color spawnColor, int x, int y)
	{
		if (pixelColor == spawnColor)
		{
			int randomIndex = 0;
			
			if (GameRoot.current.isEndGame && spawnSaboteurFirst)
			{
				for (int i = 0; i < spawnPlayerList.size; i++)
				{
					if (spawnPlayerList[i].playerIndex == GameRoot.saboteur)
					{
						randomIndex = i;
						break;
					}
				}
			}
			else
			{
				randomIndex = UnityEngine.Random.Range(0, spawnPlayerList.size);
			}
			
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
