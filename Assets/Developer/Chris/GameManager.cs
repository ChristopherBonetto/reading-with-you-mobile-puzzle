using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu = 0,
    Loading = 1,
    Playing = 2,
    Moving = 3
}

public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PlayerActions m_myPlayer;

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
        m_myPlayer.EnableCollider(false);
        m_myPlayer.canMove = true;
        SetGameState(GameState.Moving);
        OnMovement?.Invoke();
	}
}
