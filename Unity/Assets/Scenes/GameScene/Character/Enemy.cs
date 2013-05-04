using UnityEngine;
using System.Collections;

public class Enemy : Character
{
	protected override void Start()
	{
		base.Start();
		
		GameRoot.current.enemies.Add(this);
	}
}
