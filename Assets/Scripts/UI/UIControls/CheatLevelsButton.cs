using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatLevelsButton : MonoBehaviour
{
    public Image Image;

    public Sprite Unlock;
    public Sprite Lock;

    private bool m_isLocked = true;

    private void Awake()
    {
        Image = GetComponent<Image>();
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.Mode == Mode.Easy)
            {
                for (int i = 0; i < GameManager.Instance.Worlds.Length; i++)
                {
                    for (int j = 0; j < GameManager.Instance.Worlds[i].EasyLevels.Length; j++)
                    {
                        if (!GameManager.Instance.Worlds[i].EasyLevels[j].IsPlayable)
                        {
                            Image.sprite = Lock;
                            return;
                        }
                        else
                        {
                            Image.sprite = Unlock;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < GameManager.Instance.Worlds.Length; i++)
                {
                    for (int j = 0; j < GameManager.Instance.Worlds[i].HardLevels.Length; j++)
                    {
                        if (!GameManager.Instance.Worlds[i].HardLevels[j].IsPlayable)
                        {
                            Image.sprite = Lock;
                            return;
                        }
                        else
                        {
                            Image.sprite = Unlock;
                        }
                    }
                }
            }
        }
    }

    public void LockUnlockLevels()
    {
        m_isLocked = !m_isLocked;

        if (m_isLocked)
        {
            Image.sprite = Lock;
        }
        else
        {
            Image.sprite = Unlock;
        }

        GameManager.Instance.LockOrUnlockLevels(!m_isLocked);
    }
}
