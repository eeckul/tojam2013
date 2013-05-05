using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	public Camera gameCamera;
	public TweenPosition cameraTween;
	public BoxCollider boxCollider;
	public BoxCollider leftBoundary;
	public BoxCollider rightBoundary;
	
	private float levelScreenWidth;
	
	private IEnumerator Start()
	{
		levelScreenWidth = (float)Screen.width / (float)Screen.height * gameCamera.orthographicSize * 2f;
		Vector3 camPosition = transform.localPosition;
		camPosition.x = levelScreenWidth * 0.5f;
		camPosition.y = gameCamera.orthographicSize;
		transform.localPosition = camPosition;
		
		boxCollider.size = new Vector3(levelScreenWidth,
			gameCamera.orthographicSize * 2f,
			gameCamera.farClipPlane - gameCamera.nearClipPlane);
		
		leftBoundary.size = new Vector3(5f, boxCollider.size.y, boxCollider.size.z);
		rightBoundary.size = new Vector3(5f, boxCollider.size.y, boxCollider.size.z);
		
		ToggleBoundaries(true);
		
		yield return new WaitForEndOfFrame();
		
		GameRoot.current.EnteredNewScreen();
	}
	
	public void NextScreen()
	{
		ToggleBoundaries(false);
		
		Vector3 camPosition = transform.localPosition;
		cameraTween.from = camPosition;
		camPosition.x += levelScreenWidth;
		cameraTween.to = camPosition;
		cameraTween.Reset();
		cameraTween.Play(true);
	}
	
	private void CameraTweenFinished(UITweener tween)
	{
		ToggleBoundaries(true);
		GameRoot.current.EnteredNewScreen();
	}
	
	private void ToggleBoundaries(bool toggle)
	{
		if (toggle)
		{
			Vector3 boundaryCenter = leftBoundary.center;
			boundaryCenter.x = -levelScreenWidth * 0.5f - 2.5f;
			leftBoundary.center = boundaryCenter;
			boundaryCenter.x = levelScreenWidth * 0.5f + 2.5f;
			rightBoundary.center = boundaryCenter;
		}
			
		NGUITools.SetActive(leftBoundary.gameObject, toggle);
		NGUITools.SetActive(rightBoundary.gameObject, toggle);
	}
	
	private void OnTriggerEnter(Collider other)
	{
		Enemy enemy = other.gameObject.GetComponent<Enemy>();
		if (enemy != null)
		{
			GameRoot.current.enemiesOnCamera.Add(enemy);
		}
		
		LevelInteractive interactive = other.gameObject.GetComponent<LevelInteractive>();
		if (interactive != null)
		{
			GameRoot.current.interactivesOnCamera.Add(interactive);
		}
	}
	
	private void OnTriggerExit(Collider other)
	{
		Enemy enemy = other.gameObject.GetComponent<Enemy>();
		if (enemy != null)
		{
			GameRoot.current.enemiesOnCamera.Remove(enemy);
		}
		
		LevelInteractive interactive = other.gameObject.GetComponent<LevelInteractive>();
		if (interactive != null)
		{
			GameRoot.current.interactivesOnCamera.Remove(interactive);
		}
	}
}
