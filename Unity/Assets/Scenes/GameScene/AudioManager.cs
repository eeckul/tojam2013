using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public static AudioManager current;
	public AudioClip[] soundfx;
	public AudioSource src;
	
	public void Awake()
	{
		current = this;
	}
	
	public void PlaySound(string soundName)
	{
		for (int i = 0; i < soundfx.Length; i++)
		{
			if (soundfx[i].name == soundName)
			{
				src.clip = soundfx[i];
				src.Play();
			}
		}
	}
}
