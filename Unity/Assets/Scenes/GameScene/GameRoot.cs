using UnityEngine;
using System.Collections;

public class GameRoot : MonoBehaviour
{
	public static GameRoot current;
	public static int nextLevelIndex = -1;
	
	public static int saboteur = -1;
	public bool isSaboteurStage;
	public bool isReadyToCheckSaboteur;
	public float saboteurStageTime = 15f;
	private bool nextTutorialStep = false;
	
	public CharacterInput input0;
	public CharacterInput input1;
	public CharacterInput input2;
	public CharacterInput input3;
	
	public bool isEndGame;
	
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
	
	private bool nextScreenHack;
	private bool isScreenReady;
	public bool screenTransitionReady;
	private bool nextLevelTriggered;
	
	private bool playedTerminalFailSound = false;
	
	private bool endGameTriggered;
	
	private void Awake()
	{
		current = this;
		
		if (saboteur == -1)
		{
			StartCoroutine(StartSaboteurSequence());
		}
	}
	
	private IEnumerator Start()
	{
		AudioManager.current.PlaySound("level_transition");
		
		yield return new WaitForSeconds(1);
		
		nextScreenHack = true;
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
		
		if (isEndGame)
		{
			StartCoroutine(TriggerEndGame());
		}
	}
	
	private IEnumerator StartSaboteurSequence()
	{
		isSaboteurStage = true;
		saboteur = Random.Range(0, 4);
		Debug.Log("Saboteur: " + saboteur);
		
		while (UIManager.current == null) yield return new WaitForEndOfFrame();
		
		input0.OnStartPress += StartPressed;
		input1.OnStartPress += StartPressed;
		input2.OnStartPress += StartPressed;
		input3.OnStartPress += StartPressed;
		
		Vector3 exitVector = new Vector3(-350f, 0, 0);
		
		NGUITools.SetActive(UIManager.current.startSprite.gameObject, true);
		
		NGUITools.SetActive(UIManager.current.storySprite.gameObject, true);
		TweenPosition.Begin(UIManager.current.storySprite.gameObject, 1f, Vector3.zero);
		nextTutorialStep = false;
		while (!nextTutorialStep) yield return new WaitForEndOfFrame();
		TweenPosition.Begin(UIManager.current.storySprite.gameObject, 1f, exitVector);
		
		NGUITools.SetActive(UIManager.current.controlsSprite.gameObject, true);
		TweenPosition.Begin(UIManager.current.controlsSprite.gameObject, 1f, Vector3.zero);
		nextTutorialStep = false;
		while (!nextTutorialStep) yield return new WaitForEndOfFrame();
		TweenPosition.Begin(UIManager.current.controlsSprite.gameObject, 1f, exitVector);
		
		NGUITools.SetActive(UIManager.current.tutorialSprite.gameObject, true);
		TweenPosition.Begin(UIManager.current.tutorialSprite.gameObject, 1f, Vector3.zero);
		nextTutorialStep = false;
		while (!nextTutorialStep) yield return new WaitForEndOfFrame();
		TweenPosition.Begin(UIManager.current.tutorialSprite.gameObject, 1f, exitVector);
		
		NGUITools.SetActive(UIManager.current.startSprite.gameObject, false);
		
		input0.OnStartPress -= StartPressed;
		input1.OnStartPress -= StartPressed;
		input2.OnStartPress -= StartPressed;
		input3.OnStartPress -= StartPressed;
		
		isReadyToCheckSaboteur = true;
		
		TriggerNextLevel(saboteurStageTime);
	}
	
	private void StartPressed(bool pressed)
	{
		if (pressed) nextTutorialStep = true;
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
					AudioManager.current.PlaySound("door_enemy");
					OpenEnemyDoors();
				}
				else
				{
					StartCoroutine(TriggerNextScreen());
				}
			}
			else if (allTerminalsActivated)
			{
				foreach (Player player in players) player.playInteractionAnimation = false;
				
				if (exitDoor != null)
				{
					if (allTerminalsCorrect || !HasClosedEnemyDoors())
					{
						SetTerminalStates(InteractiveTerminal.TerminalState.Accepted);
						
						if (!exitDoor.isOpen && !exitDoor.isReady)
						{
							AudioManager.current.PlaySound("terminal_success");
						}
						
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
				if (!playedTerminalFailSound)
				{
					AudioManager.current.PlaySound("terminal_fail");
					playedTerminalFailSound = true;
				}
				
				SetTerminalStates(InteractiveTerminal.TerminalState.Rejected);
				foreach (Player player in players) player.playInteractionAnimation = false;
			}
			else
			{
				SetTerminalStates(InteractiveTerminal.TerminalState.Offline);
			}
		}
		
		bool allRobotsDead = true;
		
		foreach (Player player in players)
		{
			if (player.playerIndex != saboteur && !player.IsDead())
			{
				allRobotsDead = false;
			}
		}
		
		if (allRobotsDead)
		{
			StartCoroutine(TriggerGameOver());
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
		if (!nextScreenHack) yield break;
		if (isSaboteurStage || isEndGame) yield break;
		
		UIManager.current.ToggleNextScreenArrow(true);
		
		isScreenReady = false;
		screenTransitionReady = false;
		
		while (!screenTransitionReady)
		{
			yield return new WaitForEndOfFrame();
		}
		
		gameCamera.NextScreen();
	}
	
	private IEnumerator TriggerGameOver()
	{
		if (endGameTriggered) yield break;
		
		endGameTriggered = true;
		
		foreach (Enemy enemy in enemiesOnCamera)
		{
			enemy.isEnabled = false;
		}
		
		yield return new WaitForSeconds(2);
		
		UIManager.current.ToggleGoatsWin(true);
		
		foreach (Player player in players)
		{
			player.currHealth = player.maxHealth;
			
			if (player.playerIndex == saboteur)
			{
				player.victoryState = Character.VictoryState.Win;
			}
			else
			{
				player.victoryState = Character.VictoryState.Lose;
			}
		}
		
		AudioManager.current.PlaySound("theme_defeat");
	}
	
	private IEnumerator TriggerEndGame()
	{
		if (endGameTriggered) yield break;
		
		endGameTriggered = true;
		
		UIManager.current.ToggleRobotsWin(true);
		
		foreach (Player player in players)
		{
			player.currHealth = player.maxHealth;
			
			if (player.playerIndex != saboteur)
			{
				player.victoryState = Character.VictoryState.Win;
			}
			else
			{
				player.victoryState = Character.VictoryState.Lose;
			}
		}
		
		AudioManager.current.PlaySound("theme_victory");
	}
	
	public void TriggerNextLevel(float delay = 0)
	{
		if (nextLevelTriggered)
		{
			return;
		}
		
		nextLevelTriggered = true;
		StartCoroutine(DelayedNextLevel(delay));
	}
	
	private IEnumerator DelayedNextLevel(float delay)
	{
		yield return new WaitForSeconds(delay);
		
		nextLevelIndex++;
		Application.LoadLevel("GameScene");
	}
}
