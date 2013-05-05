using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour
{
	public static GameRoot current;
	public static int nextLevelIndex = 0;
	
	public GameCamera gameCamera;
	
	public BetterList<Player> players = new BetterList<Player>();
	public BetterList<Enemy> enemies = new BetterList<Enemy>();
	
	public BetterList<Enemy> enemiesOnCamera = new BetterList<Enemy>();
	public BetterList<LevelInteractive> interactivesOnCamera = new BetterList<LevelInteractive>();
	
	private InteractiveDoor exitDoor = null;
	
	private bool nextLevelTriggered = false;
	
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
	
	public void EnteredNewScreen()
	{
		exitDoor = null;
		
		foreach (LevelInteractive interactive in interactivesOnCamera)
		{
			InteractiveDoor door = interactive as InteractiveDoor;
			if (door != null && door.doorType == InteractiveDoor.DoorType.Exit)
			{
				exitDoor = door;
				break;
			}
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
			if (exitDoor != null)
			{
				exitDoor.isOpen = true;
				
				if (allTerminalsCorrect)
				{
					exitDoor.isReady = true;
				}
				else
				{
					// Spawn enemies.
				}
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
			if (!enemy.IsDead())
			{
				allEnemiesKilled = false;
				break;
			}
		}
		
		if (allEnemiesKilled)
		{
			if (exitDoor != null)
			{
				exitDoor.isOpen = true;
				exitDoor.isReady = true;
			}
			else
			{
				gameCamera.NextScreen();
			}
		}
	}
	
	public void TriggerNextLevel()
	{
		if (nextLevelTriggered)
		{
			return;
		}
		
		nextLevelTriggered = true;
		
		StartCoroutine(DelayedNextLevel());
	}
	
	private IEnumerator DelayedNextLevel()
	{
		UIPanel[] panels = current.gameObject.GetComponentsInChildren<UIPanel>();
		
		float delay = 3f;
		float timeRemaining = delay;
		while (timeRemaining > 0)
		{
			yield return new WaitForEndOfFrame();
			
			timeRemaining -= Time.deltaTime;
			
			foreach (UIPanel panel in panels)
			{
				panel.alpha = timeRemaining / delay;
			}
		}
		
		nextLevelIndex++;
		Application.LoadLevel("GameScene");
	}
}
