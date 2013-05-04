using UnityEngine;
using System.Collections;

public class LevelLoader : MonoBehaviour
{
	public Texture2D colliderMap;
	
	private int width;
	private int height;
	private Color[] colors;
	
	private void Awake()
	{
		width = colliderMap.width;
		height = colliderMap.height;
		colors = colliderMap.GetPixels();
	}
	
	private void GroundPass()
	{
		for (int w = 0; w < width; w++)
		{
			for (int h = 0; h < height; h++)
			{
				
			}
		}
	}
}
