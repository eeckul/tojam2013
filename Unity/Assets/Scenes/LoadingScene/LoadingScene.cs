using UnityEngine;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(3);
		
		Application.LoadLevel("InputTestScene");
	}
}
