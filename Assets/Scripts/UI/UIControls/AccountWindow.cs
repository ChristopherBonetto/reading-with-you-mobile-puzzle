using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AccountWindow : UIControl
{
    public override UIControlName Name => UIControlName.AccountElements;

    [SerializeField] private Image m_TextMode = null;
    [SerializeField] private Sprite m_easyMode = null;
    [SerializeField] private Sprite m_hardMode = null;

    /// <summary>
    /// text of difficult mode in main menu.
    /// </summary>
    public Image TextMode => m_TextMode;


    /// <summary>
    /// Close account info window and turn on main elements.
    /// </summary>
    public void OnCloseAccountWindow()
    {
        UIManager.Instance.ShowAndHide(UIControlName.MainElements, this);
        SoundManager.Instance.UIPlaySound(SoundManager.Instance.TapTwoAudioClip);
    }

    public void ConfirmNewAccountCreation()
    {
        // call => new instance of the player / acoount.
    }

    /// <summary>
    /// Set the game to easy mode
    /// </summary>
    public void ChangeToEasyMode()
    {
        TextMode.sprite = m_easyMode;

        if (GameManager.Instance.Mode != Mode.Easy)
        {
            GameManager.Instance.SetMode(Mode.Easy);
            GameManager.Instance.OnUpdateLevel?.Invoke();   //@TEMP
        }
        SoundManager.Instance.UIPlaySound(SoundManager.Instance.TapTwoAudioClip);
    }

    /// <summary>
    /// Set the game to hard mode.
    /// </summary>
    public void ChangeToHardMode()
    {
        TextMode.sprite = m_hardMode;

        if (GameManager.Instance.Mode != Mode.Hard)
        {
            GameManager.Instance.SetMode(Mode.Hard);
            GameManager.Instance.OnUpdateLevel?.Invoke();   //@TEMP
        }
        SoundManager.Instance.UIPlaySound(SoundManager.Instance.TapTwoAudioClip);
    }
}
