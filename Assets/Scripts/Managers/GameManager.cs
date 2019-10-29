using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Linq;

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

	public GameObject Grid;

	public World[] Worlds;

	private Level m_currentLevelInfo;
	public int m_currentLevel { get; private set; }
	public int CurrentWorld { get; set; }
	private GameObject m_currentMap;

    public List<bool> easyLevels = new List<bool>();
    public List<bool> hardLevels = new List<bool>();

    [SerializeField]
	private bool m_debugUnlockLevels = false;
    

	/// <summary>
	/// Event on player movement start
	/// </summary>
	public Action OnMovement;

    /// <summary>
    /// Event on load level
    /// </summary>
    public Action OnUpdateLevel;

	private GameState m_currentState;
    private Mode m_Mode = Mode.Easy;

	public GameState CurrentState => m_currentState;
    public Mode Mode => m_Mode;

    public MobileKeyboard keyboard;
    public string m_playerName = "";

    private PlayerDataNew data;


    private void Start()
	{
		PoolWorlds();

        LoadGame();
        LoadLevel();

        Debug.Log(easyLevels.Count);
        Debug.Log(hardLevels.Count);
        ObjectPooler.Instance.StartPooling();
        
	}


    private void PoolWorlds()
	{
		for (int i = 0; i < Worlds.Length; i++)
		{
			for (int j = 0; j < Worlds[i].EasyLevels.Length; j++)
			{
				Worlds[i].EasyLevels[j].IsPlayable = m_debugUnlockLevels;
				PoolableObject map = Worlds[i].EasyLevels[j].LevelToPool;
				if (map)
				{
					ObjectPooler.Instance.AddPoolItem(map, 1, false);
				}
			}

			for (int j = 0; j < Worlds[i].HardLevels.Length; j++)
			{
				Worlds[i].HardLevels[j].IsPlayable = m_debugUnlockLevels;
				PoolableObject map = Worlds[i].HardLevels[j].LevelToPool;
				if (map)
				{
					ObjectPooler.Instance.AddPoolItem(map, 1, false);
				}
			}

			Worlds[i].EasyLevels[0].IsPlayable = true;
			Worlds[i].HardLevels[0].IsPlayable = true;
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
	}

	private int GetLevelID()
	{
		Level[] ModeLevels = GetCurrentWorldLevels();
		
		if (m_currentLevel < ModeLevels.Length)
		{
			m_currentLevelInfo = ModeLevels[m_currentLevel];
			return m_currentLevelInfo.LevelID;
		}

        return -1;
	}

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

	public void EndLevel(bool bWin)
	{
        // Store UI controls ref
        FadeBetweenScene fade = UIManager.Instance.Controls[UIControlName.Fade] as FadeBetweenScene;
        GameWindow gameWindow = UIManager.Instance.Controls[UIControlName.InGame] as GameWindow;

		// Win level and load new
		if (bWin)
		{
			Level[] ModeLevels = GetCurrentWorldLevels();

			// Load next level
			if (m_currentLevel < ModeLevels.Length - 1)
			{
                void LoadAfterFade()
                {
					ModeLevels[m_currentLevel + 1].IsPlayable = true;
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
                    LoadLevel(CurrentWorld + 1, 0);
                    WorldBackgroundWindow background = UIManager.Instance.Controls[UIControlName.WorldBackground] as WorldBackgroundWindow;
                    background.Image.sprite = Worlds[CurrentWorld].Background;
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
            SetWorldBooleans();
            SaveGame();
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
            gameWindow.OnLevelCompleted();

            UIManager.Instance.ShowAndHide(UIControlName.Fade, UIManager.Instance.Controls[UIControlName.InGame]);
        }
	}

    public void SaveGame()
    {
        SaveSystemNew.Save(this);
    }
    
    public void LoadGame()
    {
        data = SaveSystemNew.Load();

        if (data == null)
        {
            m_playerName = "";
            return;
        }
        else
        {
            m_playerName = data.playerName;
            easyLevels = data.easyLevels.ToList();
            hardLevels = data.hardLevels.ToList();


            keyboard.m_playerName.text = m_playerName;
            keyboard.field.text = m_playerName;

            Debug.Log(easyLevels.Count);
            Debug.Log(hardLevels.Count);
        }
    }

    public void LoadLevel()
    {
        
        if (data == null)
        {
            for (int i = 0; i < 4; i++)
            {
                Worlds[i].EasyLevels[0].IsPlayable = true;
                Worlds[i].HardLevels[0].IsPlayable = true;
            }

            SaveGame();
            return;
        }
        else
        {
            for (int i = 0; i < 8; i++)
            {
                Worlds[0].EasyLevels[i].IsPlayable = data.easyLevels[i];
                Worlds[1].EasyLevels[i].IsPlayable = data.easyLevels[i + 8];
                Worlds[2].EasyLevels[i].IsPlayable = data.easyLevels[i + 16];
                Worlds[3].EasyLevels[i].IsPlayable = data.easyLevels[i + 24];
                Worlds[4].EasyLevels[i].IsPlayable = data.easyLevels[i + 32];

                Worlds[0].HardLevels[i].IsPlayable = data.hardLevels[i];
                Worlds[1].HardLevels[i].IsPlayable = data.hardLevels[i + 8];
                Worlds[2].HardLevels[i].IsPlayable = data.hardLevels[i + 16];
                Worlds[3].HardLevels[i].IsPlayable = data.hardLevels[i + 24];
                Worlds[4].HardLevels[i].IsPlayable = data.hardLevels[i + 32];
            }
        }
    }

    
    public void SetWorldBooleans()
    {
        easyLevels.Clear();
        hardLevels.Clear();

        for(int i = 0; i < Worlds.Length; i++)
        {
            CheckWorldEasyLevelsBooleans(Worlds[i]);
        }
    }

    public void CheckWorldEasyLevelsBooleans(World currentWorld)
    {
        
        for(int i = 0; i < currentWorld.EasyLevels.Length; i++)
        {
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

    /// <summary>
    /// USe for unlock or lock all levels.
    /// </summary>
    public void LockOrUnlockLevels(bool value)
    {
        for (int i = 0; i < 8; i++)
        {
            Worlds[0].EasyLevels[i].IsPlayable = value;
            Worlds[1].EasyLevels[i].IsPlayable = value;
            Worlds[2].EasyLevels[i].IsPlayable = value;
            Worlds[3].EasyLevels[i].IsPlayable = value;
            Worlds[4].EasyLevels[i].IsPlayable = value;

            Worlds[0].HardLevels[i].IsPlayable = value;
            Worlds[1].HardLevels[i].IsPlayable = value;
            Worlds[2].HardLevels[i].IsPlayable = value;
            Worlds[3].HardLevels[i].IsPlayable = value;
            Worlds[4].HardLevels[i].IsPlayable = value;
        }

        for (int i = 0; i < Worlds.Length; i++)
        {
            Worlds[i].EasyLevels[0].IsPlayable = true;
            Worlds[i].HardLevels[0].IsPlayable = true;
        }
    }

    //public void SaveGame()
    //{
    //    easyLevels.Clear();
    //    hardLevels.Clear();

    //    SetWorldBooleans();

    //    m_savePlayerData = new PlayerData();

    //    m_savePlayerData.playerName = m_playerName;        
    //    m_savePlayerData.easyLevels = easyLevels.ToArray();
    //    m_savePlayerData.hardLevels = hardLevels.ToArray();

    //    m_currentPlayer.GetComponent<AccountStats>().accountSaved = m_savePlayerData;
    //}


    //public void CreateNewAccount(string newAccountName)
    //{
    //    easyLevels.Clear();
    //    hardLevels.Clear();

    //    SetWorldBooleans();

    //    m_savePlayerData = new PlayerData();

    //    m_savePlayerData.playerName = newAccountName;
    //    m_playerName = newAccountName;
    //    m_savePlayerData.easyLevels = easyLevels.ToArray();
    //    m_savePlayerData.hardLevels = hardLevels.ToArray();


    //    CreateAccountPrefab(newAccountName);
    //}

    //// Creates a new menu item 'Examples > Create Prefab' in the main menu.
    //[MenuItem("Examples/Create Prefab")]
    //public void CreateAccountPrefab(string newAccountName)
    //{

    //    GameObject tempAccount = m_playerAcccount;
    //    tempAccount.GetComponent<AccountStats>().accountSaved = m_savePlayerData;


    //    string localPath = "Assets/Resources/Accounts/" + "Account" + newAccountName + ".prefab";
    //    localPath = AssetDatabase.GenerateUniqueAssetPath(localPath);

    //    // Create the new Prefab.
    //    PrefabUtility.SaveAsPrefabAssetAndConnect(tempAccount, localPath, InteractionMode.UserAction);
    //    m_currentPlayer = tempAccount;
    //}
}


