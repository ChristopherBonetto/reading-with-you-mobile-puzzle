using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectionWindow : UIControl
{
    // it is equal to write: string Name { get { return "..." } }
    public override string Name => "LevelSelection";

    // On load game scene disable this window.
    public void OnLoadLevel()
    {
        UIManager.Instance.ShowAndHide("Fade", this);
    }
}
