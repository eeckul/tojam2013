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
	
	public GameObject enemiesRoot;
	public GameObject enemyPrefab1;
	public GameObject enemyPrefab2;
	public GameObject enemyPrefab3;
	
	private bool allEnemiesKilled;
	private bool hasTerminals;
	private bool allTerminalsActivated;
	private bool allTerminalsCorrect;
	private InteractiveDoor exitDoor = null;
	private BetterList<InteractiveDoor> enemyDoors = new BetterList<InteractiveDoor>();
	
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
		
		isScreenReady = true;
		exitDoor = null;
		enemyDoors.Clear();
		
		foreach (LevelInteractive interactive in interactivesOnCamera)
		{
			InteractiveDoor door = interactive as InteractiveDoor;
			if (door != null)
			{
				if (door.doorType == InteractiveDoor.DoorType.Exit)
				{
					exitDoor = door;
				}
				else if (door.doorType == InteractiveDoor.DoorType.Enemy)
				{
					enemyDoors.Add(door);
				}
			}
		}
		
		Vector3 warpPoint;
		warpPoint.x = gameCamera.leftBoundary.transform.localPosition.x +
			((BoxCollider)gameCamera.leftBoundary.collider).center.x +
				((BoxCollider)gameCamera.leftBoundary.collider).size.x * 0.5f;
		warpPoint.y = gameCamera.transform.localPosition.y + gameCamera.camera.orthographicSize;
		warpPoint.z = 12.5f;
		
		Ray ray = new Ray(warpPoint, Vector3.down);
		RaycastHit hitInfo;
		Physics.Raycast(ray, out hitInfo);
		float xOffset = 8f;
		
		foreach (Player player in players)
		{
			if (player.transform.localPosition.x < gameCamera.leftBoundary.transform.localPosition.x)
			{
				Vector3 playerPosition = player.transform.localPosition;
				playerPosition.x = hitInfo.point.x + player.boxCollider.size.x * 0.5f + xOffset;
				playerPosition.y = hitInfo.point.y + player.boxCollider.size.y * 0.5f;
				player.transform.localPosition = playerPosition;
				xOffset += 8f;
			}
		}
	}
	
	private void UpdateTerminalStates()
	{
		hasTerminals = false;
		allTerminalsActivated = true;
		allTerminalsCorrect = true;
		
		foreach (LevelInteractive interactive in interactivesOnCamera)
		{
			InteractiveTerminal terminal = interactive as InteractiveTerminal;
			if (terminal != null)
			{
				hasTerminals = true;
				
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
		
		if (EnemyDoorsSpawning())
		{
			allEnemiesKilled = false;
		}
		else
		{
			foreach (Enemy enemy in enemiesOnCamera)
			{
				if (!enemy.IsDead())
				{
					allEnemiesKilled = false;
					break;
				}
			}
		}
	}
	
	private void CheckGameState()
	{
		if (allEnemiesKilled)
		{
			if (!hasTerminals)
			{
				if (HasClosedEnemyDoors())
				{
					OpenEnemyDoors();
				}
				else
				{
					StartCoroutine(TriggerNextScreen());
				}
			}
			else if (allTerminalsActivated)
			{
				if (exitDoor != null)
				{
					if (allTerminalsCorrect || !HasClosedEnemyDoors())
					{
						SetTerminalStates(InteractiveTerminal.TerminalState.Accepted);
						exitDoor.isOpen = true;
						exitDoor.isReady = true;
					}
					else
					{
						OpenEnemyDoors();
					}
				}
				else if (isScreenReady)
				{
					SetTerminalStates(InteractiveTerminal.TerminalState.Accepted);
					StartCoroutine(TriggerNextScreen());
				}
			}
			else
			{
				SetTerminalStates(InteractiveTerminal.TerminalState.WaitingInput);
			}
		}
		else
		{
			if (allTerminalsActivated)
			{
				SetTerminalStates(InteractiveTerminal.TerminalState.Rejected);
			}
			else
			{
				SetTerminalStates(InteractiveTerminal.TerminalState.Offline);
			}
		}
	}
	
	private bool HasClosedEnemyDoors()
	{
		foreach (InteractiveDoor door in enemyDoors)
		{
			if (!door.IsFullyOpen)
			{
				return true;
			}
		}
		
		return false;
	}
	
	private bool EnemyDoorsSpawning()
	{
		foreach (InteractiveDoor door in enemyDoors)
		{
			if (door.isSpawning)
			{
				return true;
			}
		}
		
		return false;
	}
	
	private void OpenEnemyDoors()
	{
		foreach (InteractiveDoor door in enemyDoors)
		{
			door.isOpen = true;
		}
	}
	
	private void SetTerminalStates(InteractiveTerminal.TerminalState terminalState)
	{
		foreach (LevelInteractive interactive in interactivesOnCamera)
		{
			InteractiveTerminal terminal = interactive as InteractiveTerminal;
			if (terminal != null)
			{
				if (terminal.isActivated && terminalState == InteractiveTerminal.TerminalState.WaitingInput)
				{
					terminal.terminalState = InteractiveTerminal.TerminalState.Accepted;
				}
				else
				{
					terminal.terminalState = terminalState;
				}
			}
		}
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
		yield return new WaitForSeconds(1);
		
		nextLevelIndex++;
		Application.LoadLevel("GameScene");
	}
}
