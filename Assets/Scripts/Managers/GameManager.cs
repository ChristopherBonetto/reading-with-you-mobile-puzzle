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
/// Difficulty of the game
/// </summary>
public enum Mode
{
    Easy,
    Hard
}

public class GameManager : Singleton<GameManager>
{
	public PlayerActions Player;

	public FinalObjectActions FinalObject;

	public World[] Worlds;

	private Level m_currentLevelInfo;
	public int m_currentLevel { get; private set; }
	public int CurrentWorld { get; set; }
	private GameObject m_currentMap;

	/// <summary>
	/// Event on player movement start
	/// </summary>
	public Action OnMovement;

    /// <summary>
    /// Event on load level
    /// </summary>
    public Action OnUpdateLevel;

	private GameState m_currentState;
    private Mode m_Mode;

	public GameState CurrentState => m_currentState;
    public Mode Mode => m_Mode;

	private void Start()
	{
        //@TEMP @ALE
        for (int i = 0; i < Worlds.Length; i++)
        {
            Worlds[i].EasyLevels[0].IsPlayable = true;
            Worlds[i].HardLevels[0].IsPlayable = true;

			for (int j = 0; j < Worlds[i].EasyLevels.Length; j++)
			{
				PoolableObject map = Worlds[i].EasyLevels[j].LevelToPool;
				if (map)
				{
					ObjectPooler.Instance.AddPoolItem(map, 1, false); 
				}
			}

			for (int j = 0; j < Worlds[i].HardLevels.Length; j++)
			{
				PoolableObject map = Worlds[i].HardLevels[j].LevelToPool;
				if (map)
				{
					ObjectPooler.Instance.AddPoolItem(map, 1, false);
				}
			}
		}

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
			FinalObject.ResetLevel(m_currentLevelInfo.GoalObject.Coords, m_currentLevelInfo.Icon);
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
		Level[] ModeLevels = new Level[0];
		if (CurrentWorld < Worlds.Length)
		{
			if (Mode == Mode.Easy)
			{
				ModeLevels = Worlds[CurrentWorld].EasyLevels;
			}
			else if (Mode == Mode.Hard)
			{
				ModeLevels = Worlds[CurrentWorld].HardLevels;
			}
		}
		
		if (m_currentLevel < ModeLevels.Length)
		{
			m_currentLevelInfo = ModeLevels[m_currentLevel];
			return m_currentLevelInfo.LevelID;
		}

        return -1;
	}

	public void EndLevel(bool bWin)
	{
        //@TEMP
        //@ALE
        // Store UI controls ref
        FadeBetweenScene fade = UIManager.Instance.Controls[UIControlName.Fade] as FadeBetweenScene;
        GameWindow gameWindow = UIManager.Instance.Controls[UIControlName.InGame] as GameWindow;

		if (bWin)
		{
            if (Mode == Mode.Easy)
            {
			    // Load next level
			    if (m_currentLevel < Worlds[CurrentWorld].EasyLevels.Length - 1)
			    {
                    void LoadAfterFade()
                    {
                        Worlds[CurrentWorld].EasyLevels[m_currentLevel + 1].IsPlayable = true;
				        LoadLevel(m_currentLevel + 1);
                    }

                    fade.FadeInCompleted = LoadAfterFade;

                    // remake visible game window and turn off fade.
                    gameWindow.OnLevelCompleted();
                }
			    else if (CurrentWorld < Worlds.Length - 1)
			    {
                    void LoadAfterFade()
                    {
                        //@TODO Handle end world animations
                        LoadLevel(CurrentWorld + 1, 0);
                    }

                    fade.FadeInCompleted = LoadAfterFade;

                    // remake visible game window and turn off fade.
                    gameWindow.OnLevelCompleted();
                }
                else if (CurrentWorld >= Worlds.Length - 1)
                {
                    // return to level selection
                    void ReturnToLevelSelection()
                    {
                        Player.ResetLevel(m_currentLevelInfo.PlayerCoords);
                        UIManager.Instance.Show(UIControlName.LevelSelection);
                    }

                    fade.FadeInCompleted = ReturnToLevelSelection;
                    fade.FadeOutCompleted = fade.OnHide;
                }
                UIManager.Instance.ShowAndHide(UIControlName.Fade, UIManager.Instance.Controls[UIControlName.InGame]);
            }

            else if (Mode == Mode.Hard)
            {
                // Load next level
                if (m_currentLevel < Worlds[CurrentWorld].HardLevels.Length - 1)
                {
                    void LoadAfterFade()
                    {
                        Worlds[CurrentWorld].HardLevels[m_currentLevel + 1].IsPlayable = true;
                        LoadLevel(m_currentLevel + 1);
                    }

                    fade.FadeInCompleted = LoadAfterFade;

                    // remake visible game window and turn off fade.
                    gameWindow.OnLevelCompleted();
                }
                else if (CurrentWorld < Worlds.Length - 1)
                {
                    void LoadAfterFade()
                    {
                        //@TODO Handle end world animations
                        LoadLevel(CurrentWorld + 1, 0);
                    }

                    fade.FadeInCompleted = LoadAfterFade;

                    // remake visible game window and turn off fade.
                    gameWindow.OnLevelCompleted();
                }
                else if (CurrentWorld >= Worlds.Length - 1)
                {
                    // return to level selection
                    void ReturnToLevelSelection()
                    {
                        Player.ResetLevel(m_currentLevelInfo.PlayerCoords);
                        UIManager.Instance.Show(UIControlName.LevelSelection);
                    }

                    fade.FadeInCompleted = ReturnToLevelSelection;
                    fade.FadeOutCompleted = fade.OnHide;
                }
                 UIManager.Instance.ShowAndHide(UIControlName.Fade, UIManager.Instance.Controls[UIControlName.InGame]);
            }
        }
		else
		{
            void LoadAfterFade()
            {
			    //@TODO Handle reload animation
			    Player.ResetLevel(m_currentLevelInfo.PlayerCoords);
                FinalObject.EnableDisableCollider(true);
			    SetGameState(GameState.Playing);
            }

            fade.FadeInCompleted = LoadAfterFade;
            gameWindow.OnLevelCompleted();

            UIManager.Instance.ShowAndHide(UIControlName.Fade, UIManager.Instance.Controls[UIControlName.InGame]);
        }
	}
}
