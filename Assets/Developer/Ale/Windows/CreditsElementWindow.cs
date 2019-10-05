using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsElementWindow : UIControl
{
    public override string Name => "CreditsElements";

    public void OnBackToMainElements()
    {
        UIManager.Instance.ShowAndHide("MainElements", this);
    }
}
