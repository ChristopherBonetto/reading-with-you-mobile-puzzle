using System;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Game state
/// </summary>
public enum GameState
{
    Menu = 0,		// Out of level
    Playing = 1,	// In level, placing blocks
    Moving = 2		// In level, player walking
}

/// <summary>
/// Difficulty of the game
/// </summary>
public enum Mode
{
    Easy = 0,
    Hard = 1
}

public class GameManager : Singleton<GameManager>
{
	#region Variables

	public PlayerActions Player;

	public FinalObjectActions FinalObject;

	public GameObject Grid;

	public World[] Worlds;

	private Level m_currentLevelInfo;
	public int CurrentLevel { get; private set; }
	public int CurrentWorld { get; set; }
	private GameObject m_currentMap;

	private GameState m_currentState;
	private Mode m_Mode;

	public GameState CurrentState => m_currentState;
	public Mode Mode => m_Mode;
    
	/// <summary>
	/// Event on player movement start
	/// </summary>
	public Action OnMovement;

    /// <summary>
    /// Event on load level
    /// </summary>
    public Action OnUpdateLevel;

    public MobileKeyboard keyboard;
    public string m_playerName = "";

    private PlayerDataNew data;

	public List<bool> easyLevels = new List<bool>();
	public List<bool> hardLevels = new List<bool>();

	#endregion

	private void Start()
	{
		PoolWorlds();
        
        LoadGame();
        LoadLevelProgress();
        
        ObjectPooler.Instance.StartPooling();

		OnUpdateLevel += DisableWalkingPlayer;

        SoundManager.Instance.PlayStartMenuAudio();
    }
    

    #region States

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
		if (Grid)
		{
			Grid.SetActive(false);
		}
        OnMovement?.Invoke();
	}

	private void DisableWalkingPlayer()
	{
		SetGameState(GameState.Menu);
	}

	#endregion

	#region Level loading

	/// <summary>
	/// Add all levels to pool
	/// </summary>
	private void PoolWorlds()
	{
		for (int i = 0; i < Worlds.Length; i++)
		{
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
	}

	/// <summary>
	/// Public interface to prepare a map
	/// </summary>
	/// <param name="levelNo">Selected level from current world</param>
	public void LoadLevel(int levelNo)
	{
		LoadLevel(CurrentWorld, levelNo);
	}

	/// <summary>
	/// Prepare map elements for selected level
	/// </summary>
	/// <param name="worldNo">Selected world from current mode</param>
	/// <param name="levelNo">Selected level</param>
	private void LoadLevel(int worldNo, int levelNo)
	{
		// Unload current level
		if (m_currentMap)
		{
			m_currentMap.SetActive(false);
			BlockManager.Instance.UnloadBlocks();
		}

		// Update current level info
		CurrentWorld = worldNo;
		CurrentLevel = levelNo;
		int levelID = GetLevelID();

		Player.SetAnimationLayer(CurrentWorld);
        SoundManager.Instance.PlayBackgroundSound(CurrentWorld);

		// Load map elements
        if (levelID >= 0)
		{
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
			FinalObject.ResetLevel(m_currentLevelInfo.GoalObject.Coords, m_currentLevelInfo.InGameIcon);
			if (Grid)
			{
				Grid.SetActive(true);
			}
			BlockManager.Instance.LoadBlocks(m_currentLevelInfo.Blocks);
			SetGameState(GameState.Playing);
		}
		else
		{
			Debug.Log("Level number out of bounds");
		}

		// Update save info
        SetWorldBooleans();
        SaveGame();
    }

	/// <summary>
	/// Read pool id from level and store level info
	/// </summary>
	/// <returns>Pool id for current level</returns>
    private int GetLevelID()
	{
		Level[] ModeLevels = GetCurrentWorldLevels();
		
		if (CurrentLevel < ModeLevels.Length)
		{
			m_currentLevelInfo = ModeLevels[CurrentLevel];
			return m_currentLevelInfo.LevelID;
		}

        return -1;
	}

	/// <summary>
	/// Read level list from world
	/// </summary>
	/// <returns>Level list for current world</returns>
	private Level[] GetCurrentWorldLevels()
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
		return ModeLevels;
	}

	/// <summary>
	/// Called when player completes movement
	/// </summary>
	/// <param name="bWin">True if player has reached the objective</param>
	public void EndLevel(bool bWin)
	{
        // Store UI controls ref
        FadeBetweenScene fade = UIManager.Instance.Controls[UIControlName.Fade] as FadeBetweenScene;
        GameWindow gameWindow = UIManager.Instance.Controls[UIControlName.InGame] as GameWindow;

		// Win level and load new
		if (bWin)
		{
			Level[] ModeLevels = GetCurrentWorldLevels();

			// Load next level in same world
			if (CurrentLevel < ModeLevels.Length - 1)
			{
                void LoadAfterFade()
                {
					ModeLevels[CurrentLevel + 1].IsPlayable = true;
				    LoadLevel(CurrentLevel + 1);
                }

                fade.FadeInCompleted = LoadAfterFade;
				fade.FadeTint = Worlds[CurrentWorld].WorldColor;

                // remake visible game window and turn off fade.
                gameWindow.OnLevelCompleted();
            }
			// Load first level in next world
			else if (CurrentWorld < Worlds.Length - 1)
			{
                void LoadAfterFade()
                {
                    LoadLevel(CurrentWorld + 1, 0);
                    WorldBackgroundWindow background = UIManager.Instance.Controls[UIControlName.WorldBackground] as WorldBackgroundWindow;
                    background.Image.sprite = Worlds[CurrentWorld].Background;
                }

                fade.FadeInCompleted = LoadAfterFade;
				fade.FadeTint = Worlds[CurrentWorld + 1].WorldColor;

                // remake visible game window and turn off fade.
                gameWindow.OnLevelCompleted();
            }
			// End game
            else if (CurrentWorld >= Worlds.Length - 1)
            {
                // return to level selection
                void ReturnToLevelSelection()
                {
                    UIManager.Instance.Show(UIControlName.LevelSelection);
                    Player.ResetLevel(m_currentLevelInfo.PlayerCoords);
                }
                
                fade.FadeInCompleted = ReturnToLevelSelection;
                fade.FadeOutCompleted = fade.OnHide;
				fade.FadeTint = Worlds[CurrentWorld].WorldColor;

				SoundManager.Instance.StopAllSounds();
				SoundManager.Instance.PlayStartMenuAudio();
            }

			// start fade
            UIManager.Instance.ShowAndHide(UIControlName.Fade, UIManager.Instance.Controls[UIControlName.InGame]);
        }
		// Lose level and restore positions
		else
		{
            void LoadAfterFade()
            {
			    Player.ResetLevel(m_currentLevelInfo.PlayerCoords);
                FinalObject.ToggleCollider(true);
				if (Grid)
				{
					Grid.SetActive(true);
				}
				SetGameState(GameState.Playing);
            }

            fade.FadeInCompleted = LoadAfterFade;

			// remake visible game window and turn off fade.
			gameWindow.OnLevelCompleted();

			// start fade
            UIManager.Instance.ShowAndHide(UIControlName.Fade, UIManager.Instance.Controls[UIControlName.InGame]);
        }
	}

	#endregion

	#region Save

    /// <summary>
    /// Save this gamemanager.
    /// </summary>
	public void SaveGame()
    {
        SaveSystemNew.Save(this);
    }

    /// <summary>
    /// Set the easyLevels and hardLevels array bools equal to the loaded file if it exist.
    /// </summary>
    public void LoadGame()
    {
        data = SaveSystemNew.Load();

        if (data == null)
        {
            m_playerName = "Enter a name";
            return;
        }
        else
        {
            m_playerName = data.PlayerName;
            easyLevels = data.EasyLevels.ToList();
            hardLevels = data.HardLevels.ToList();


            keyboard.m_playerName.text = m_playerName;
            keyboard.field.text = m_playerName;
            
        }
    }

    /// <summary>
    /// With the filled easyLevels and hardLevels array set all the levels's bool.
    /// </summary>
    public void LoadLevelProgress()
    {
        if (data == null)
        {
            for (int i = 0; i < Worlds.Length; i++)
            {
                Worlds[i].EasyLevels[0].IsPlayable = true;
                Worlds[i].HardLevels[0].IsPlayable = true;
            }

            SaveGame();
            return;
        }
        else
        {
            for (int i = 0; i < Worlds.Length; i++)
            {
                for (int j = 0; j < Worlds[i].EasyLevels.Length; j++)
                {
                    Worlds[i].EasyLevels[j].IsPlayable = data.EasyLevels[j];
                    Worlds[i].HardLevels[j].IsPlayable = data.HardLevels[j];
                }
            }
        }
        
    }

    /// <summary>
    /// Take the current bool of each level and store it into the easyLevels or hardLevels array.
    /// </summary>
    public void SetWorldBooleans()
    {
        easyLevels.Clear();
        hardLevels.Clear();

        for(int i = 0; i < Worlds.Length; i++)
        {
            CheckWorldEasyLevelsBooleans(Worlds[i]);
        }
    }

    /// <summary>
    /// Check all worlds's levels and return true if the current level is playable.
    /// </summary>
    public void CheckWorldEasyLevelsBooleans(World currentWorld)
    {
        
        for(int i = 0; i < currentWorld.EasyLevels.Length; i++)
        {
            currentWorld.EasyLevels[0].IsPlayable = true;

            if (currentWorld.EasyLevels[i].IsPlayable)
            {
                easyLevels.Add(true);
            }
            else
            {
                easyLevels.Add(false);
            }
        }

        for (int i = 0; i < currentWorld.HardLevels.Length; i++)
        {
            currentWorld.HardLevels[0].IsPlayable = true;

            if (currentWorld.HardLevels[i].IsPlayable)
            {
                hardLevels.Add(true);
            }
            else
            {
                hardLevels.Add(false);
            }
        }
    }
#endregion
    
    
    /// <summary>
    /// USe for unlock or lock all levels.
    /// </summary>
    public void LockOrUnlockLevels(bool value)
    {
        for (int i = 0; i < Worlds.Length; i++)
        {
            for (int j = 0; j < Worlds[i].EasyLevels.Length; j++)
            {
                Worlds[i].EasyLevels[j].IsPlayable = value;
                Worlds[i].HardLevels[j].IsPlayable = value;
            }
            // Unlock first level ofevery world.
            Worlds[i].EasyLevels[0].IsPlayable = true;
            Worlds[i].HardLevels[0].IsPlayable = true;
        }
    }


    public bool CheckAllLevelsPlayable()
    {
        bool isPlayable = true;

        for (int i = 0; i < Worlds.Length; i++)
        {
            for (int j = 0; j < Worlds[i].EasyLevels.Length; j++)
            {
                if (!Worlds[i].EasyLevels[j].IsPlayable)
                    return isPlayable = false;

                if (!Worlds[i].HardLevels[j].IsPlayable)
                    return isPlayable = false;
            }
        }
        return isPlayable;
    }
}
