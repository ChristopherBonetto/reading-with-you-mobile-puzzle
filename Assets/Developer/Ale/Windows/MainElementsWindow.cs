using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainElementsWindow : UIControl
{
    public override UIControlName Name => UIControlName.MainElements;

    // Serializefield
    [SerializeField] private Text m_TextMode;
    [SerializeField] private string m_easyModeText;
    [SerializeField] private string m_hardModeText;

    // Properties
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
    /// Execute when credit button is pressed
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
        /*@TODO set in game manager the easy mode.
        the default mode is easy. */

        TextMode.text = m_easyModeText;

        GameManager.Instance.SetMode(Mode.Easy);

        LevelSelectionWindow levelWindow = UIManager.Instance.Controls[UIControlName.LevelSelection] as LevelSelectionWindow;
        levelWindow.UpdateWorldAndLevelInfo();
    }

    /// <summary>
    /// Set the game to hard mode.
    /// </summary>
    public void ChangeToHardMode()
    {
        /*@TODO set in game manager the hard mode.
        the default mode is easy. */

        TextMode.text = m_hardModeText;

        GameManager.Instance.SetMode(Mode.Hard);

        LevelSelectionWindow levelWindow = UIManager.Instance.Controls[UIControlName.LevelSelection] as LevelSelectionWindow;
        levelWindow.UpdateWorldAndLevelInfo();
    }
}
