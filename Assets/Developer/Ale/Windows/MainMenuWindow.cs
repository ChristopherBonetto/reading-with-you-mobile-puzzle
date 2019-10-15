using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWindow : UIControl
{
    public override UIControlName Name => UIControlName.MainMenu;


    protected override void Start()
    {
        base.Start();

        // Show always at the start of the game
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Executed when Start button is pressed
    /// </summary>
    public void OnStartButton()
    {
        UIManager.Instance.ShowAndHide(UIControlName.LevelSelection, this);
    }
}
