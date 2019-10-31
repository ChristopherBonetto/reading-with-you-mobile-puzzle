using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheatLevelsButton : MonoBehaviour
{
    private Image Image;

    public Sprite Unlock;
    public Sprite Lock;

    private bool m_isLocked = true;

    private void Awake()
    {
        Image = GetComponent<Image>();
    }

    private void Start()
    {
        if (GameManager.Instance.CheckAllLevelsPlayable())
            Image.sprite = Unlock;
        else
            Image.sprite = Lock;

        m_isLocked = Image.sprite == Lock ? true : false;

        SetImage();

        Debug.Log(m_isLocked);
    }

    public void LockUnlockLevels()
    {
        m_isLocked = !m_isLocked;

        SetImage();

        SoundManager.Instance.UIPlaySound(SoundManager.Instance.TapTwoAudioClip);
        GameManager.Instance.LockOrUnlockLevels(!m_isLocked);
    }

    private void SetImage()
    {
        if (m_isLocked)
            Image.sprite = Lock;
        else
            Image.sprite = Unlock;
    }
}
