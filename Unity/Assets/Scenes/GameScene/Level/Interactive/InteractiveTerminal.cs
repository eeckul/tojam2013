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
	
	private void Start()
	{
		Vector3 size = boxCollider.size;
		size.x *= 3f;
		boxCollider.size = size;
	}
}