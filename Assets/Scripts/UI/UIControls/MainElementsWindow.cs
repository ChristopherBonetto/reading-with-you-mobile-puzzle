using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainElementsWindow : UIControl
{
    public override UIControlName Name => UIControlName.MainElements;


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
    /// Action invoked when account button is pressed.
    /// </summary>
    public void OnAccountInfoButton()
    {
        UIManager.Instance.ShowAndHide(UIControlName.AccountElements, this);
    }

    /// <summary>
    /// Switch from the current account to the next one.
    /// </summary>
    public void SwitchAccount()
    {
        // call switch account function from elsewhere.
    }
}
