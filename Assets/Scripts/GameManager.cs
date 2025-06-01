using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Unity.AI.Navigation;

public class GameManager : MonoBehaviour
{
	public List<LevelData> levelDataList;
	public LevelData CurrentLevelData { get; private set; }

	public GridManager gridManager;
	public TowerManager towerManager;
	public NPCManager npcManager;
	public GameUIManager gameUIManager;
	public NavMeshSurface surface;
	
	private float timeRemaining;
	private bool isGameOver = false;

	public static GameManager Instance;
	
	private void Awake()
	{
		if ( Instance != null )
		{
			Destroy( gameObject );
		}
		else
		{
			Instance = this;
		}
	}

	private void Start()
	{
		InitializeLevel(levelDataList[PersistentGameData.CurrentLevelIndex]);
		timeRemaining = CurrentLevelData.timeLimit;
	}

	private void Update()
	{
		if (isGameOver) return;

		timeRemaining -= Time.deltaTime;
		if (timeRemaining <= 0)
		{
			FailLevel();
		}
	}

	private void InitializeLevel(LevelData levelData)
	{
		CurrentLevelData = levelData;
		
		gridManager.InitializeGrid(this);
		towerManager.InitializeTower(this);
		npcManager.InitializeNPCManager( this );
		gameUIManager.InitializeUI(this);
		
		surface.BuildNavMesh();
	}

	public void CompleteLevel()
	{
		isGameOver = true;
		
		gameUIManager.ShowLevelCompletePanel();
	}

	private void FailLevel()
	{
		isGameOver = true;
		
		gameUIManager.ShowTimeUpPanel();
	}

	public float GetTimeRemaining()
	{
		return timeRemaining;
	}
	
	public void RestartLevel()
	{
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	
	public void GoToMainMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}
	
	public void LoadNextLevel()
	{
		PersistentGameData.CurrentLevelIndex++;
		
		if(PersistentGameData.CurrentLevelIndex >= levelDataList.Count)
		{
			PersistentGameData.CurrentLevelIndex = 0;
		}

		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
}