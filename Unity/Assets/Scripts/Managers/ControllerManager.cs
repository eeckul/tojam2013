using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ControllerManager : MonoBehaviour
{
	public int controller;
	
	public enum Button
	{
		Start,
		Back,
		LeftStick,
		RightStick,
		LeftBumper,
		RightBumper,
		A,
		B,
		X,
		Y,
		DPadUp,
		DPadDown,
		DPadLeft,
		DPadRight
	}
	
	public enum Stick
	{
		Left,
		Right
	}
	
	public enum Trigger
	{
		Left,
		Right
	}
	
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
	private Dictionary<Stick, Vector2> mStickStates = new Dictionary<Stick, Vector2>();
	private Dictionary<Trigger, float> mTriggerStates = new Dictionary<Trigger, float>();
	
	private void Start()
	{
		mStickStates.Add(Stick.Left, Vector2.zero);
		mStickStates.Add(Stick.Right, Vector2.zero);
		mTriggerStates.Add(Trigger.Left, 0f);
		mTriggerStates.Add(Trigger.Right, 0f);
	}
	
	private void Update()
	{
		CheckButtonState("ButtonStart", OnStartPress);
		CheckButtonState("ButtonBack", OnBackPress);
		CheckButtonState("ButtonStickLeft", OnLeftStickPress);
		CheckButtonState("ButtonStickRight", OnRightStickPress);
		CheckButtonState("BumperLeft", OnLeftBumperPress);
		CheckButtonState("BumperRight", OnRightBumperPress);
		CheckButtonState("ButtonA", OnAPress);
		CheckButtonState("ButtonB", OnBPress);
		CheckButtonState("ButtonX", OnXPress);
		CheckButtonState("ButtonY", OnYPress);
		
#if UNITY_STANDALONE_OSX
		CheckButtonState("DPadUp", OnDPadUpPress);
		CheckButtonState("DPadDown", OnDPadDownPress);
		CheckButtonState("DPadLeft", OnDPadLeftPress);
		CheckButtonState("DPadRight", OnDPadRightPress);
#elif UNITY_STANDALONE_WIN
		CheckDPadStates("DPadY", true, OnDPadUpPress);
		CheckDPadStates("DPadY", false, OnDPadDownPress);
		CheckDPadStates("DPadX", false, OnDPadLeftPress);
		CheckDPadStates("DPadX", true, OnDPadRightPress);
#endif
		
		string controllerPrefix = "J" + controller.ToString();
		mStickStates[Stick.Left] = new Vector2(Input.GetAxis(controllerPrefix + "StickLeftX"), Input.GetAxis(controllerPrefix + "StickLeftY"));
		mStickStates[Stick.Right] = new Vector2(Input.GetAxis(controllerPrefix + "StickRightX"), Input.GetAxis(controllerPrefix + "StickRightY"));
		mTriggerStates[Trigger.Left] = Input.GetAxis(controllerPrefix + "TriggerLeft");
		mTriggerStates[Trigger.Right] = Input.GetAxis(controllerPrefix + "TriggerRight");
	}
	
	private void CheckButtonState(string buttonName, ButtonPressDelegate pressEvent)
	{
		string fullButtonName = "J" + controller.ToString() + buttonName;
		bool pressed = Input.GetButton(fullButtonName);
		bool changed = false;
		
		if (mButtonStates.ContainsKey(buttonName))
		{
			changed = mButtonStates[buttonName] != pressed;
			mButtonStates[buttonName] = pressed;
		}
		else
		{
			mButtonStates.Add(buttonName, pressed);
		}
		
		if (changed && pressEvent != null)
		{
			pressEvent(pressed);
		}
	}
	
#if UNITY_STANDALONE_WIN
	private void CheckDPadStates(string axisName, bool positive, ButtonPressDelegate pressEvent)
	{
		string fullAxisName = "J" + controller.ToString() + axisName;
		bool pressed = positive ? Input.GetAxis(fullAxisName) > 0f : Input.GetAxis(fullAxisName) < 0f;
		bool changed = false;
		
		string axisKey = axisName + (positive ? "Pos" : "Neg");
		
		if (mButtonStates.ContainsKey(axisKey))
		{
			changed = mButtonStates[axisKey] != pressed;
			mButtonStates[axisKey] = pressed;
		}
		else
		{
			mButtonStates.Add(axisKey, pressed);
		}
		
		if (changed && pressEvent != null)
		{
			pressEvent(pressed);
		}
	}
#endif
	
	public bool IsButtonDown(Button button)
	{
		string prefix = "J" + controller.ToString();
		
		switch (button)
		{
			case Button.Start:
				return Input.GetButton(prefix + "ButtonStart");
			case Button.Back:
				return Input.GetButton(prefix + "ButtonBack");
			case Button.LeftStick:
				return Input.GetButton(prefix + "ButtonStickLeft");
			case Button.RightStick:
				return Input.GetButton(prefix + "ButtonStickRight");
			case Button.LeftBumper:
				return Input.GetButton(prefix + "BumperLeft");
			case Button.RightBumper:
				return Input.GetButton(prefix + "BumperRight");
			case Button.A:
				return Input.GetButton(prefix + "ButtonA");
			case Button.B:
				return Input.GetButton(prefix + "ButtonB");
			case Button.X:
				return Input.GetButton(prefix + "ButtonX");
			case Button.Y:
				return Input.GetButton(prefix + "ButtonY");
#if UNITY_STANDALONE_OSX
			case Button.DPadUp:
				return Input.GetButton(prefix + "DPadUp");
			case Button.DPadDown:
				return Input.GetButton(prefix + "DPadDown");
			case Button.DPadLeft:
				return Input.GetButton(prefix + "DPadLeft");
			case Button.DPadRight:
				return Input.GetButton(prefix + "DPadRight");
#elif UNITY_STANDALONE_WIN
			case Button.DPadUp:
				return Input.GetAxis(prefix + "DPadY") > 0f;
			case Button.DPadDown:
				return Input.GetAxis(prefix + "DPadY") < 0f;
			case Button.DPadLeft:
				return Input.GetAxis(prefix + "DPadX") < 0f;
			case Button.DPadRight:
				return Input.GetAxis(prefix + "DPadX") > 0f;
#endif
			default:
				return false;
		}
	}
	
	public Vector2 GetStick(Stick stick)
	{
		return mStickStates[stick];
	}
	
	public float GetTrigger(Trigger trigger)
	{
		return mTriggerStates[trigger];
	}
}
