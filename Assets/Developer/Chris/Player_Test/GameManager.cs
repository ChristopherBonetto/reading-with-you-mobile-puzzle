using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

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

    [SerializeField] private GameObject[] m_worldsPooler;
    public List<World> m_listOfWorlds = new List<World>();

	//private GameObject m_currentLevel = null;
	//private GameObject m_currentWorld = null;

	private Vector3 m_playerStartPosition;

	private GameObject m_currentMap;

	/// <summary>
	/// Event on player movement start
	/// </summary>
	public Action OnMovement;

	private GameState m_currentState;

	public GameState CurrentState => m_currentState;

    public void AdvanceToNextScene()
    {
        int currentScene = SceneManager.GetActiveScene().buildIndex;
        int nextScene = (currentScene < SceneManager.sceneCountInBuildSettings - 1 ?
            // Load next scene
            currentScene + 1 :
            // Load first scene
            0);

        SceneManager.LoadScene(nextScene);
        SetGameState(GameState.Playing);
    }

    public void ReceiveLevelLoaded()
	{
		BlockManager.Instance.ResetAllBlocks();
	}

    protected override void Awake()
    {
        base.Awake();
        //StartWorldsInstantiate();
    }

	private void Start()
	{
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

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    ChangeLevel(0, 1);
        //}
        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    ChangeLevel(0, 2);
        //}
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    ChangeLevel(1, 1);
        //}

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

	public void LoadLevel(int LevelID)
	{
		if (m_currentMap)
		{
			m_currentMap.SetActive(false);
		}
		m_currentMap = ObjectPooler.Instance.GetPooledObject(LevelID);
		m_currentMap.SetActive(true);

		Player.EnableCollider(true);
		Player.canMove = false;
		Player.transform.position = m_playerStartPosition;
		SetGameState(GameState.Playing);
	}

	//private void StartWorldsInstantiate()
	//{
	//    for(int i = 0; i < m_worldsPooler.Length; i++)
	//    {
	//        GameObject world = Instantiate(m_worldsPooler[i].gameObject);
	//        m_listOfWorlds.Add(world.GetComponent<ObjectPooler>());
	//        world.transform.name = "World" + i;
	//        world.SetActive(false);
	//    }
	//}

	//public void ChangeLevel(int worldNumber, int levelNumber)
	//{       

	//    if(worldNumber <= m_listOfWorlds.Count)
	//    {
	//        if(m_currentLevel != null)
	//        {
	//            m_currentLevel.SetActive(false);
	//            m_currentLevel = null;
	//        }

	//        if(m_currentWorld != m_listOfWorlds[worldNumber].transform.gameObject)
	//        {
	//            if(m_currentWorld != null)
	//            {
	//                m_currentWorld.SetActive(false);
	//            }

	//            m_currentWorld = m_listOfWorlds[worldNumber].transform.gameObject;
	//            m_currentWorld.SetActive(true);

	//            m_currentLevel = m_listOfWorlds[worldNumber].GetPooledObject(levelNumber);
	//        }
	//        else if(m_currentWorld == m_listOfWorlds[worldNumber].transform.gameObject)
	//        {
	//            m_currentLevel = m_listOfWorlds[worldNumber].GetPooledObject(levelNumber);
	//        }

	//        if(m_currentLevel != null)
	//        {
	//            m_currentLevel.SetActive(true);
	//        }
	//    }
	//    else
	//    {
	//        Debug.Log("Not enought worlds");
	//    }

	//}


}
