using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour
{
	public static GameRoot current;
	
	public GameCamera gameCamera;
	
	public BetterList<Player> players = new BetterList<Player>();
	public BetterList<Enemy> enemies = new BetterList<Enemy>();
	
	public BetterList<Enemy> enemiesOnCamera = new BetterList<Enemy>();
	public BetterList<LevelInteractive> interactivesOnCamera = new BetterList<LevelInteractive>();
	
	private void Awake()
	{
		current = this;
	}
	
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
	
	public void ActivatedLevelTerminal()
	{
		bool allTerminalsActivated = true;
		bool allTerminalsCorrect = true;
		
		foreach (LevelInteractive interactive in interactivesOnCamera)
		{
			InteractiveTerminal terminal = interactive as InteractiveTerminal;
			if (terminal != null)
			{
				if (!terminal.isActivated)
				{
					allTerminalsActivated = false;
					break;
				}
				else if (!terminal.activatedCorrectly)
				{
					allTerminalsCorrect = false;
				}
			}
		}
		
		if (allTerminalsActivated)
		{
			InteractiveDoor exitDoor = null;
			
			foreach (LevelInteractive interactive in interactivesOnCamera)
			{
				InteractiveDoor door = interactive as InteractiveDoor;
				if (door != null && door.doorType == InteractiveDoor.DoorType.Exit)
				{
					exitDoor = door;
					break;
				}
			}
			
			if (exitDoor != null)
			{
				// Check for correct activation.
			}
			else
			{
				gameCamera.NextScreen();
			}
		}
	}
	
	public void KilledEnemy()
	{
		bool allEnemiesKilled = true;
		
		foreach (Enemy enemy in enemiesOnCamera)
		{
			// Check for living enemies.
		}
		
		if (allEnemiesKilled)
		{
			gameCamera.NextScreen();
		}
	}
}
