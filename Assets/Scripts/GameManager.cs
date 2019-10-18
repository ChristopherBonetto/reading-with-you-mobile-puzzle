using System;
using UnityEngine;

public enum GameState
{
    Menu = 0,
    Loading = 1,
    Playing = 2,
    Moving = 3
}

public class GameManager : Singleton<GameManager>
{
	public PlayerActions Player;

	public FinalObjectActions FinalObject;

	public World[] Worlds;

	private Level m_currentLevelInfo;
	private int m_currentLevel;
	private int m_currentWorld;
	private GameObject m_currentMap;

	/// <summary>
	/// Event on player movement start
	/// </summary>
	public Action OnMovement;

	private GameState m_currentState;

	public GameState CurrentState => m_currentState;

	private void Start()
	{
		ObjectPooler.Instance.StartPooling();
	}

	public void SetGameState(GameState inGameState)
    {
        if (m_currentState == inGameState)
        {
            return;
        }

        m_currentState = inGameState;
    }

    public void StartWalkingPlayer()
    {
        Player.EnableMovement(true);
        SetGameState(GameState.Moving);
        OnMovement?.Invoke();
	}

	public void LoadLevel(int levelNo)
	{
		LoadLevel(m_currentWorld, levelNo);
	}

	private void LoadLevel(int worldNo, int levelNo)
	{
		// Unload current level
		if (m_currentMap)
		{
			m_currentMap.SetActive(false);
			BlockManager.Instance.UnloadBlocks();
		}

		// Load level
		m_currentWorld = worldNo;
		m_currentLevel = levelNo;
		int levelID = GetLevelID();
		if (levelID >= 0)
		{
			//@TODO Handle load level animation
			m_currentMap = ObjectPooler.Instance.GetPooledObject(levelID);
			if (m_currentMap)
			{
				m_currentMap.SetActive(true);
			}
			else
			{
				Debug.LogWarning("Can't unpool map. ID " + levelID + " not found.");
			}
			Player.ResetLevel(m_currentLevelInfo.PlayerCoords);
			FinalObject.ResetLevel(m_currentLevelInfo.GoalObject.Coords);
			BlockManager.Instance.LoadBlocks(m_currentLevelInfo.Blocks);
			//@TODO Set objective
			SetGameState(GameState.Playing);
		}
		else
		{
			Debug.Log("Level number out of bounds");
		}
	}

	private int GetLevelID()
	{
		if (m_currentWorld < Worlds.Length && m_currentLevel < Worlds[m_currentWorld].Levels.Length)
		{
			m_currentLevelInfo = Worlds[m_currentWorld].Levels[m_currentLevel];
			return m_currentLevelInfo.LevelID;
		}
		else
		{
			return -1;
		}
	}

	public void EndLevel(bool bWin)
	{
		if (bWin)
		{
			// Load next level
			if (m_currentLevel < Worlds[m_currentWorld].Levels.Length - 1)
			{
				LoadLevel(m_currentLevel + 1);
			}
			else if (m_currentWorld < Worlds.Length - 1)
			{
				//@TODO Handle end world animations
				LoadLevel(m_currentWorld + 1, 0);
			}
		}
		else
		{
			//@TODO Handle reload animation
			Player.ResetLevel(m_currentLevelInfo.PlayerCoords);
			SetGameState(GameState.Playing);
		}
	}
}
