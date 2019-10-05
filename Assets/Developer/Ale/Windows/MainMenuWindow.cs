using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWindow : UIControl
{
    // it is equal to write: string Name { get { return "..." } }
    public override string Name => "MainMenu";

    protected override void Start()
    {
        // Do nothing
    }

    public void OnShowLevelSelection() // button should execute this.
    {
        UIManager.Instance.ShowAndHide("LevelSelection", this);
    }
}
