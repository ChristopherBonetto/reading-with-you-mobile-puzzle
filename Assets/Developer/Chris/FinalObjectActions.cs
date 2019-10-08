using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalObjectActions : MonoBehaviour
{
    private int m_IndexScene;

    // Start is called before the first frame update
    void Start()
    {
        m_IndexScene = SceneManager.GetActiveScene().buildIndex;

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerActions>())
        {
            Debug.Log("ciao");
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
    }
}
