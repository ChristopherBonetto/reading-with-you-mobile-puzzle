using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private PlayerActions m_myPlayer;

    private int m_IndexScene;

    private void Awake()
    {
        Instance = this;        
    }
    // Start is called before the first frame update
    void Start()
    {
        m_IndexScene = SceneManager.GetActiveScene().buildIndex;

    }

    public void AdvanceToNextScene()
    {
        if (m_IndexScene < SceneManager.sceneCountInBuildSettings - 1)
        {
            // Load next scene
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else
        {
            // Load first scene
            SceneManager.LoadScene(0);
        }

    }

    public void StartWalkingPlayer()
    {
        m_myPlayer.EnableCollider(false);
        m_myPlayer.SetNewPlayerState(PlayerStates.Forwards);
        
    }
}
