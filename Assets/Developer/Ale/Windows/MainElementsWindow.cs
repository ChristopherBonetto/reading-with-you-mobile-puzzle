using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainElementsWindow : UIControl
{
    public override UIControlName Name => UIControlName.MainElements;

    protected override void Start()
    {
        // do nothing
    }

    public void OnCreditsSelection()
    {
        UIManager.Instance.ShowAndHide(UIControlName.CreditsElements, this);
    }
}
