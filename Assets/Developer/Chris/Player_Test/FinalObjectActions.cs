using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalObjectActions : MonoBehaviour
{
    [SerializeField] private GameObject m_tutorialLevel;
    private int currentLevel = 1;

    

    public void NextLevel()
    {
        m_tutorialLevel.SetActive(false);
        GameManager.Instance.ChangeLevel(0, currentLevel);
        GameManager.Instance.SetGameState(GameState.Playing);
        currentLevel++;
    }
}
