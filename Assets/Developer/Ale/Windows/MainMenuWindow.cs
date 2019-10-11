using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWindow : UIControl
{
    // it is equal to write: string Name { get { return "..." } }
    public override UIControlName Name => UIControlName.MainMenu;

    protected override void Start()
    {
        base.Start();
        this.gameObject.SetActive(true);
    }

    public void OnShowLevelSelection() // button should execute this.
    {
        UIManager.Instance.ShowAndHide(UIControlName.LevelSelection, this);
    }
}
