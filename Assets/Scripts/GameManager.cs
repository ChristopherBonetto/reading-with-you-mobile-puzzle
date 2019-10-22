using System;
using UnityEngine;

public enum GameState
{
    Menu = 0,
    Loading = 1,
    Playing = 2,
    Moving = 3
}

/// <summary>
/// Difficult of the game.
/// </summary>
public enum Mode
{
    Easy,
    Hard,
}

public class GameManager : Singleton<GameManager>
{
	public PlayerActions Player;

	public FinalObjectActions FinalObject;

	public World[] Worlds;

	private Level m_currentLevelInfo;
	private int m_currentLevel;
	public int CurrentWorld { get; set; }
	private GameObject m_currentMap;

	/// <summary>
	/// Event on player movement start
	/// </summary>
	public Action OnMovement;

	private GameState m_currentState;
    private Mode m_Mode;

	public GameState CurrentState => m_currentState;
    public Mode Mode => m_Mode;

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

    public void SetMode(Mode inMode)
    {
        if (m_Mode == inMode)
        {
            return;
        }

        m_Mode = inMode;
    }

    public void StartWalkingPlayer()
    {
        Player.EnableMovement(true);
        SetGameState(GameState.Moving);
        OnMovement?.Invoke();
	}

	public void LoadLevel(int levelNo)
	{
		LoadLevel(CurrentWorld, levelNo);
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
		CurrentWorld = worldNo;
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
        if (Mode == Mode.Easy)
        {
		    if (CurrentWorld < Worlds.Length && m_currentLevel < Worlds[CurrentWorld].EasyLevels.Length)
		    {
			    m_currentLevelInfo = Worlds[CurrentWorld].EasyLevels[m_currentLevel];
			    return m_currentLevelInfo.LevelID;
		    }
		    else
		    {
			    return -1;
		    }
        }

        else if (Mode == Mode.Hard)
        {
            if (CurrentWorld < Worlds.Length && m_currentLevel < Worlds[CurrentWorld].HardLevels.Length)
            {
                m_currentLevelInfo = Worlds[CurrentWorld].HardLevels[m_currentLevel];
                return m_currentLevelInfo.LevelID;
            }
            else
            {
                return -1;
            }
        }

        return -1;
	}

	public void EndLevel(bool bWin)
	{
		if (bWin)
		{
            if (Mode == Mode.Easy)
            {
			    // Load next level
			    if (m_currentLevel < Worlds[CurrentWorld].EasyLevels.Length - 1)
			    {
				    LoadLevel(m_currentLevel + 1);
                }
			    else if (CurrentWorld < Worlds.Length - 1)
			    {
				    //@TODO Handle end world animations
				    LoadLevel(CurrentWorld + 1, 0);
			    }
            }

            else if (Mode == Mode.Hard)
            {
                // Load next level
                if (m_currentLevel < Worlds[CurrentWorld].HardLevels.Length - 1)
                {
                    LoadLevel(m_currentLevel + 1);
                }
                else if (CurrentWorld < Worlds.Length - 1)
                {
                    //@TODO Handle end world animations
                    LoadLevel(CurrentWorld + 1, 0);
                }
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
