using UnityEngine;
using UnityEngine.UI;

public class MobileKeyboard : MonoBehaviour
{
    public TouchScreenKeyboard Keyboard;
    public InputField field;

    public Text m_playerName;

    public Text m_playerEditName;

    /// <summary>
    /// Open the mobile KeyBoard
    /// </summary>
    public void OpenKeyBoard()
    {
        // Open the keyboard 
        // @TODO : initializate the keyboard's string with player's name.
        Keyboard = TouchScreenKeyboard.Open(GameManager.Instance.m_playerName, TouchScreenKeyboardType.Default);
    }

    /// <summary>
    /// On End edit
    /// </summary>
    public void OnEndEdit()
    {
        // Set player name.
        ShowPlayerNameInMainMenu();

        // Save the name into player data
        GameManager.Instance.m_playerName = m_playerName.text;
        GameManager.Instance.SaveGame();
    }

    public void ShowPlayerNameInMainMenu()
    {
        m_playerName.text = m_playerEditName.text;
    }
}
