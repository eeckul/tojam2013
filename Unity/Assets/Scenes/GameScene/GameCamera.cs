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
	
	private void Start()
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
		
		Vector3 boundaryCenter = leftBoundary.center;
		boundaryCenter.x = -levelScreenWidth * 0.5f - 2.5f;
		leftBoundary.center = boundaryCenter;
		boundaryCenter.x = levelScreenWidth * 0.5f + 2.5f;
		rightBoundary.center = boundaryCenter;
	}
	
	public void NextScreen()
	{
		NGUITools.SetActive(leftBoundary.gameObject, false);
		NGUITools.SetActive(rightBoundary.gameObject, false);
		
		Vector3 camPosition = transform.localPosition;
		cameraTween.from = camPosition;
		camPosition.x += levelScreenWidth;
		cameraTween.to = camPosition;
		cameraTween.Reset();
		cameraTween.Play(true);
	}
	
	private void CameraTweenFinished(UITweener tween)
	{
		NGUITools.SetActive(leftBoundary.gameObject, true);
		NGUITools.SetActive(rightBoundary.gameObject, true);
	}
}
