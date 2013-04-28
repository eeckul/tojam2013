using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerManager : MonoBehaviour
{
	public int controller;
	
	public delegate void ButtonPressDelegate(bool pressed);
	public event ButtonPressDelegate OnStartPress;
	public event ButtonPressDelegate OnBackPress;
	public event ButtonPressDelegate OnLeftStickPress;
	public event ButtonPressDelegate OnRightStickPress;
	public event ButtonPressDelegate OnLeftBumperPress;
	public event ButtonPressDelegate OnRightBumperPress;
	public event ButtonPressDelegate OnAPress;
	public event ButtonPressDelegate OnBPress;
	public event ButtonPressDelegate OnXPress;
	public event ButtonPressDelegate OnYPress;
	public event ButtonPressDelegate OnDPadUpPress;
	public event ButtonPressDelegate OnDPadDownPress;
	public event ButtonPressDelegate OnDPadLeftPress;
	public event ButtonPressDelegate OnDPadRightPress;
	
	private Dictionary<string, bool> mButtonStates = new Dictionary<string, bool>();
	
	private void Update()
	{
		CheckButtonState("ButtonStart", OnStartPress);
		CheckButtonState("ButtonBack", OnBackPress);
	}
	
	private void CheckButtonState(string buttonName, ButtonPressDelegate pressEvent)
	{
		bool pressed = Input.GetButton(buttonName);
		bool changed = mButtonStates[buttonName] != pressed;
		mButtonStates[buttonName] = pressed;
		
		if (changed && pressEvent != null)
		{
			pressEvent(pressed);
		}
	}
}
