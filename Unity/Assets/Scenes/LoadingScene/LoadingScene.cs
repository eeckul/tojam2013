using UnityEngine;
using System.Collections;

public class LoadingScene : MonoBehaviour
{
	public CharacterInput input0;
	public CharacterInput input1;
	public CharacterInput input2;
	public CharacterInput input3;
	
	private void Awake()
	{
		input0.OnStartPress += StartPressed;
		input1.OnStartPress += StartPressed;
		input2.OnStartPress += StartPressed;
		input3.OnStartPress += StartPressed;
	}

	private void StartPressed (bool pressed)
	{
		Application.LoadLevel("GameScene");
	}
}
