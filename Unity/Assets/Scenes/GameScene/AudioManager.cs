using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour
{
	public static AudioManager current;
	public AudioClip[] soundfx;
	
	private const int NUM_SOUNDS = 16;
	private AudioSource[] soundsrc = new AudioSource[NUM_SOUNDS];
	
	public void Awake()
	{
		current = this;
		
		for (int i = 0; i < NUM_SOUNDS; i++)
		{
			soundsrc[i] = gameObject.AddComponent<AudioSource>();
		}
	}
	
	public void PlaySound(string soundName)
	{
		for (int i = 0; i < soundfx.Length; i++)
		{
			if (soundfx[i].name == soundName)
			{
				for (int j = 0; j < NUM_SOUNDS; j++)
				{
					if (!soundsrc[j].isPlaying)
					{
						soundsrc[j].clip = soundfx[i];
						soundsrc[j].Play();
						break;
					}
				}
				
				break;
			}
		}
	}
}
