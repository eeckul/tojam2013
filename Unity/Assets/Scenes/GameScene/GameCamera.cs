using UnityEngine;
using System.Collections;

public class GameCamera : MonoBehaviour
{
	public Camera gameCamera;
	public TweenPosition cameraTween;
	public BoxCollider boxCollider;
	
	private float levelScreenWidth;
	
	private void Start()
	{
		levelScreenWidth = (float)Screen.width / (float)Screen.height * gameCamera.orthographicSize * 2f;
		Vector3 camPosition = transform.localPosition;
		camPosition.x = levelScreenWidth * 0.5f;
		camPosition.y = gameCamera.orthographicSize;
		transform.localPosition = camPosition;
		
		Vector3 size = boxCollider.size;
		size.x = levelScreenWidth;
		size.y = gameCamera.orthographicSize * 2f;
		size.z = gameCamera.farClipPlane - gameCamera.nearClipPlane;
		boxCollider.size = size;
	}
	
	public void NextScreen()
	{
		Vector3 camPosition = transform.localPosition;
		cameraTween.from = camPosition;
		camPosition.x += levelScreenWidth;
		cameraTween.to = camPosition;
		cameraTween.Reset();
		cameraTween.Play(true);
	}
	
	private void CameraTweenFinished(UITweener tween)
	{
		
	}
}
