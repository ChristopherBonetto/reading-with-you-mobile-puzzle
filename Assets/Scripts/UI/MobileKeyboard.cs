using UnityEngine;

public class MobileKeyboard : MonoBehaviour
{
    public TouchScreenKeyboard Keyboard;

    /// <summary>
    /// Open the mobile KeyBoard
    /// </summary>
    public void OpenKeyBoard()
    {
        // Open the keyboard 
        // @TODO : initializate the keyboard's string with player's name.
        Keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default);
    }

    /// <summary>
    /// On End edit
    /// </summary>
    public void OnEndEdit()
    {
        // Save the name into player data
    }
}
