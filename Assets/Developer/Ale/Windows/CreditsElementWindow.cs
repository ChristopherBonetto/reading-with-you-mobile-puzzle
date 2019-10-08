using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsElementWindow : UIControl
{
    public override UIControlName Name => UIControlName.CreditsElements;

    public void OnBackToMainElements()
    {
        UIManager.Instance.ShowAndHide(UIControlName.MainElements, this);
    }
}
