using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuWindow : UIControl
{
    public override UIControlName Name => UIControlName.MainMenu;


    protected override void Start()
    {
        base.Start();

        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Action invoked when start button is pressed.
    /// </summary>
    public void OnStartButton()
    {
        UIManager.Instance.ShowAndHide(UIControlName.LevelSelection, this);

        GameManager.Instance.OnUpdateLevel?.Invoke();
    }
}
