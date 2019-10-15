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
    [SerializeField] private PlayerActions m_player;

    public PlayerActions Player
    {
        get
        {
            return m_player;
        }
        set
        {
            m_player = value;
        }
    }

    [SerializeField] private FinalObjectActions m_finalObject;

    public FinalObjectActions FinalObject
    {
        get
        {
            return m_finalObject;
        }
        set
        {
            m_finalObject = value;
        }
    }

    [SerializeField] private ObjectPooler m_blockPooler;

	public World[] Worlds;

	private int m_currentLevel;
	private int m_currentWorld;

	private Vector3 m_playerStartPosition;

	private GameObject m_currentMap;

	/// <summary>
	/// Event on player movement start
	/// </summary>
	public Action OnMovement;

	private GameState m_currentState;

	public GameState CurrentState => m_currentState;

    public void ReceiveLevelLoaded()
	{
		BlockManager.Instance.ResetAllBlocks();
	}

	private void Start()
	{
		//@TEMP This will be loaded with level info
		if (Player)
		{
			m_playerStartPosition = Player.transform.position; 
		}
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.R))
		{
			ReceiveLevelLoaded();
		}

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
        Player.EnableCollider(false);
        Player.canMove = true;
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
			//@TODO Disable blocks
			//@TEMP
			ReceiveLevelLoaded();
			//@TODO Disable objective
		}

		// Load level
		m_currentWorld = worldNo;
		m_currentLevel = levelNo;
		int levelID = GetLevelID();
		if (levelID >= 0)
		{
			//@TODO Handle load level animation
			m_currentMap = ObjectPooler.Instance.GetPooledObject(levelID);
			m_currentMap.SetActive(true);
			Player.ResetLevel(m_playerStartPosition);
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
			return Worlds[m_currentWorld].Levels[m_currentLevel].LevelID;
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
			// Reload animation
			Player.ResetLevel(m_playerStartPosition);
			//@TEMP this will be probably changed
			SetGameState(GameState.Playing);
		}
	}
}
