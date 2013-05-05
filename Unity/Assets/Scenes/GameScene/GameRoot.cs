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
	
	private bool allEnemiesKilled;
	private bool allTerminalsActivated;
	private bool allTerminalsCorrect;
	private InteractiveDoor exitDoor = null;
	
	private bool terminalPenaltyTriggered;
	private bool isScreenReady;
	public bool screenTransitionReady;
	private bool nextLevelTriggered;
	
	private void Awake()
	{
		current = this;
	}
	
	private void LateUpdate()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
		
		if (isScreenReady)
		{
			UpdateTerminalStates();
			UpdateEnemyStates();
			CheckGameState();
		}
	}
	
	public void EnteredNewScreen()
	{
		UIManager.current.ToggleNextScreenArrow(false);
		
		terminalPenaltyTriggered = false;
		isScreenReady = true;
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
	
	private void UpdateTerminalStates()
	{
		allTerminalsActivated = true;
		allTerminalsCorrect = true;
		
		foreach (LevelInteractive interactive in interactivesOnCamera)
		{
			InteractiveTerminal terminal = interactive as InteractiveTerminal;
			if (terminal != null)
			{
				if (!terminal.isActivated)
				{
					allTerminalsActivated = false;
					allTerminalsCorrect = false;
					break;
				}
				else if (!terminal.activatedCorrectly)
				{
					allTerminalsCorrect = false;
				}
			}
		}
	}
	
	private void UpdateEnemyStates()
	{
		allEnemiesKilled = true;
		
		foreach (Enemy enemy in enemiesOnCamera)
		{
			if (!enemy.IsDead())
			{
				allEnemiesKilled = false;
				break;
			}
		}
	}
	
	private void CheckGameState()
	{
		if (allEnemiesKilled)
		{
			if (allTerminalsActivated)
			{
				if (exitDoor != null)
				{
					exitDoor.isOpen = true;
						
					if (allTerminalsCorrect || terminalPenaltyTriggered)
					{
						exitDoor.isReady = true;
					}
					else
					{
						TriggerTerminalPenalty();
					}
				}
				else if (isScreenReady)
				{
					StartCoroutine(TriggerNextScreen());
				}
			}
		}
	}
	
	private void TriggerTerminalPenalty()
	{
		terminalPenaltyTriggered = true;
	}
	
	private IEnumerator TriggerNextScreen()
	{
		UIManager.current.ToggleNextScreenArrow(true);
		
		isScreenReady = false;
		screenTransitionReady = false;
		
		while (!screenTransitionReady)
		{
			yield return new WaitForEndOfFrame();
		}
		
		gameCamera.NextScreen();
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
		yield return new WaitForSeconds(2f);
		
		nextLevelIndex++;
		Application.LoadLevel("GameScene");
	}
}
