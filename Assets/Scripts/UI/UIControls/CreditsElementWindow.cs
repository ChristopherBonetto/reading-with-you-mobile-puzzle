using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreditsElementWindow : UIControl
{
    public override UIControlName Name => UIControlName.CreditsElements;


    /// <summary>
    /// Execute when credits window exitbutton is pressed.
    /// </summary>
    public void OnCreditsExitButton()
    {
        UIManager.Instance.ShowAndHide(UIControlName.MainElements, this);
    }
}
