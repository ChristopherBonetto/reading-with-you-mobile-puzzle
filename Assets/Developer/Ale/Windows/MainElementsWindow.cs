using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainElementsWindow : UIControl
{
    public override UIControlName Name => UIControlName.MainElements;


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
}
