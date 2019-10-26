using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainElementsWindow : UIControl
{
    [SerializeField] private Text m_TextMode;
    [SerializeField] private string m_easyModeText;
    [SerializeField] private string m_hardModeText;

    public override UIControlName Name => UIControlName.MainElements;

    /// <summary>
    /// text of difficult mode in main menu.
    /// </summary>
    public Text TextMode => m_TextMode;


    protected override void Start()
    {
        base.Start();

        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Action invoked when credit button is pressed.
    /// </summary>
    public void OnCreditsButton()
    {
        UIManager.Instance.ShowAndHide(UIControlName.CreditsElements, this);
    }

    /// <summary>
    /// Set the game to easy mode
    /// </summary>
    public void ChangeToEasyMode()
    {
        TextMode.text = m_easyModeText;

        GameManager.Instance.SetMode(Mode.Easy);
        GameManager.Instance.OnUpdateLevel?.Invoke();   //@TEMP
    }

    /// <summary>
    /// Set the game to hard mode.
    /// </summary>
    public void ChangeToHardMode()
    {
        TextMode.text = m_hardModeText;

        GameManager.Instance.SetMode(Mode.Hard);
        GameManager.Instance.OnUpdateLevel?.Invoke();   //@TEMP
    }
}
