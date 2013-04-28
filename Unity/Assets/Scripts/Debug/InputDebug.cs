using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class InputDebug : SingletonMonoBehaviour<InputDebug>
{
	public UIFont labelFont;
	public UITable[] controllerTables;
	private Dictionary<string, UILabel> mInputValues = new Dictionary<string, UILabel>();
	
	private string[] inputNames = 
	{
		"StickLeftX",
		"StickLeftY",
		"StickRightX",
		"StickRightY",
		"TriggerLeft",
		"TriggerRight",
		"ButtonStart",
		"ButtonBack",
		"ButtonStickLeft",
		"ButtonStickRight",
		"BumperLeft",
		"BumperRight",
		"ButtonA",
		"ButtonB",
		"ButtonX",
		"ButtonY",
#if UNITY_STANDALONE_OSX
		"DPadUp",
		"DPadDown",
		"DPadLeft",
		"DPadRight"
#else
		"DPadX",
		"DPadY"
#endif
	};
	
	protected override void Init()
	{
		string[] joystickNames = Input.GetJoystickNames();
		
		for (int i = 0; i < joystickNames.Length; i++)
		{
			Debug.Log("Controller #" + i.ToString() + ": " + joystickNames[i]);
		}
		
		for (int i = 0; i < controllerTables.Length; i++)
		{
			CreateControllerTable(controllerTables[i], "J" + i.ToString());
		}
	}
	
	private void Update()
	{
		for (int i = 0; i < controllerTables.Length; i++)
		{
			UpdateInputLabels("J" + i.ToString());
		}
	}
	
	private void CreateControllerTable(UITable controllerTable, string prefix)
	{
		for (int i = 0; i < inputNames.Length; i++)
		{
			string inputName = prefix + inputNames[i];
			UILabel label = CreateInputLabel(controllerTable, inputName);
			mInputValues.Add(inputName, label);
		}
		
		controllerTable.Reposition();
	}
	
	private UILabel CreateInputLabel(UITable controllerTable, string inputName)
	{
		UILabel controllerLabel = NGUITools.AddWidget<UILabel>(controllerTable.gameObject);
		controllerLabel.font = labelFont;
		controllerLabel.pivot = UIWidget.Pivot.Left;
		controllerLabel.name = inputName;
		controllerLabel.text = inputName + ":";
		controllerLabel.MakePixelPerfect();
		return controllerLabel;
	}
	
	private void UpdateInputLabels(string prefix)
	{
		for (int i = 0; i < 6; i++)
		{
			string inputName = prefix + inputNames[i];
			float value = Input.GetAxis(inputName);
			mInputValues[inputName].text = inputName + ": " + value.ToString();
		}
		
		for (int i = 6; i < inputNames.Length; i++)
		{
			string inputName = prefix + inputNames[i];
			bool value = Input.GetButton(inputName);
			mInputValues[inputName].text = inputName + ": " + value.ToString();
		}
	}
}
