using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelInfo : MonoBehaviour
{
    public Text Text;

    private void OnEnable()
    {
        Text.text = "WORLD: " + GameManager.Instance.CurrentWorld.ToString() + " | LEVEL: " + GameManager.Instance.m_currentLevel.ToString();
    }
    //private void OnDisable()
    //{
    //    Text.text = "WORLD: " + GameManager.Instance.CurrentWorld.ToString() + "| LEVEL: " + GameManager.Instance.m_currentLevel.ToString();
        
    //}
}
