using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Use this class as father of every Manager/Singleton class.
/// </summary>
public class SingletonBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T m_Instance;
    public static T Instance
    {
        get
        {
            if (m_Instance != null)
            {
                return m_Instance;
            }
            m_Instance = FindObjectOfType<T>();

            if (m_Instance != null)
            {
                return m_Instance;
            }

            GameObject go = new GameObject("Singleton - " + typeof(T).Name);
            m_Instance = go.AddComponent<T>();

            if (m_Instance == null)
            {
                throw new System.Exception("Can't generate " + typeof(T).Name + " Singleton");
            }

            return m_Instance;
        }
    }

    private bool m_DontDestroy = false;
    public bool DontDestroy => m_DontDestroy;

    #region MonoBehaviour Methods

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }

        EnableDontDestroy();
    }

    #endregion

    #region Public Methods

    public void EnableDontDestroy()
    {
        if (m_DontDestroy) return;
        DontDestroyOnLoad(this.gameObject);
        m_DontDestroy = true;
    }

    #endregion
}
