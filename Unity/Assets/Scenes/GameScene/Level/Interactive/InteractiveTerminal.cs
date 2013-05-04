using UnityEngine;
using System.Collections;

public class InteractiveTerminal : LevelInteractive
{
	public enum TerminalType
	{
		A,
		B,
		X,
		Y
	}
	
	public TerminalType terminalType;
}
