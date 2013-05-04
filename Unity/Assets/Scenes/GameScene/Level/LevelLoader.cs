using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	public GameObject levelRoot;
	
	public string textureName;
	private Texture2D colliderMap;
	private int width;
	private int height;
	private Color[] colors;
	
	public GameObject groundPrefab;
	private Color groundColor = RGBColor(0, 0, 0);
	
	public GameObject platformPrefab1;
	private Color platformColor1 = RGBColor(0, 0, 64);
	public GameObject platformPrefab2;
	private Color platformColor2 = RGBColor(0, 0, 128);
	public GameObject platformPrefab3;
	private Color platformColor3 = RGBColor(0, 0, 192);
	
	private void Awake()
	{
		colliderMap = Resources.Load(textureName) as Texture2D;
		width = colliderMap.width;
		height = colliderMap.height;
		colors = colliderMap.GetPixels();
		
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
	}
	
	private void ObjectPass()
	{
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				Color color = colors[w + h * width];
				
				CreateObject(color, platformColor1, platformPrefab1, w, h, true);
				CreateObject(color, platformColor2, platformPrefab2, w, h, true);
				CreateObject(color, platformColor3, platformPrefab3, w, h, true);
			}
		}
	}
	
	private void CreateGroundCollider(int blockStart, int blockWidth, int blockHeight)
	{
		BoxCollider groundCollider = NGUITools.AddChild(levelRoot, groundPrefab).GetComponent<BoxCollider>();
		groundCollider.size = new Vector3(blockWidth, blockHeight, 50f);
		groundCollider.center = new Vector3(blockWidth * 0.5f, blockHeight * 0.5f, 0);
		groundCollider.transform.localPosition = new Vector3(blockStart, 0, 0);
	}
	
	private void CreateObject(Color pixelColor, Color objectColor, GameObject prefab, int x, int y, bool topAnchor = false)
	{
		if (pixelColor == objectColor)
		{
			Debug.Log("Creating " + prefab.name + " at (" + x + ", " + y + ")");
			
			GameObject prefabObject = NGUITools.AddChild(levelRoot, prefab);
			BoxCollider prefabCollider = prefabObject.GetComponent<BoxCollider>();
			prefabObject.transform.localPosition = new Vector3(x + prefabCollider.size.x * 0.5f,
				(topAnchor ? y - prefabCollider.size.y * 0.5f : y + prefabCollider.size.y * 0.5f),
				0);
		}
	}
}
