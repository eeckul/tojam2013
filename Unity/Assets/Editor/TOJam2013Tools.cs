using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;

public class TOJam2013Tools
{
	[MenuItem("TOJam 2013 Tools/Set Input for OSX")]
	private static void SetInputForOSX()
	{
		ChangeInputExtension(".osx", ".win");
	}
	
	[MenuItem("TOJam 2013 Tools/Set Input for Windows")]
	private static void SetInputForWindows()
	{
		ChangeInputExtension(".win", ".osx");
	}
	
	private static void ChangeInputExtension(string desiredExtension, string previousExtension)
	{
		string inputFile = Application.dataPath + "/../ProjectSettings/InputManager.asset";
		
		if (File.Exists(inputFile + desiredExtension))
		{
			if (File.Exists(inputFile))
			{
				File.Move(inputFile, inputFile + previousExtension);
			}
			
			File.Move(inputFile + desiredExtension, inputFile);
		}
	}
}
