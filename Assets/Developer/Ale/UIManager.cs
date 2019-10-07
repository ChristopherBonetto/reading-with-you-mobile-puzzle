using System.Collections.Generic;
using UnityEngine;

// All names of ui controls
public enum UIControlName
{
    MainMenu,
    MainElements,
    CreditsElements,
    LevelSelection,
    InGame,
    Fade,
}

public class UIManager : SingletonBehaviour<UIManager>
{
    /// <summary>
    /// All controls stored.
    /// </summary>
    public Dictionary<UIControlName, UIControl> Controls = new Dictionary<UIControlName, UIControl>();

    /// <summary>
    /// Register a ui control
    /// </summary>
    /// <param name="uiControl"></param>
    public void Register(UIControl uiControl)
    {
        if (uiControl != null && !Controls.ContainsKey(uiControl.Name))
        {
            Debug.Log("Register: " + uiControl.Name);
            Controls.Add(uiControl.Name, uiControl);
        }
    }

    /// <summary>
    /// Unregister a ui control
    /// </summary>
    /// <param name="uiControl"></param>
    public void Unregister(UIControl uiControl)
    {
        if (uiControl != null && !Controls.ContainsKey(uiControl.Name))
        {
            Controls.Remove(uiControl.Name);
        }
    }

    /// <summary>
    /// Show a ui control
    /// </summary>
    /// <param name="uiControlName"></param>
    public void Show(UIControlName uiControlName)
    {
        if (Controls.TryGetValue(uiControlName, out UIControl control))
        {
            control.OnShow();
        }
    }

    /// <summary>
    /// Hide a ui control
    /// </summary>
    /// <param name="uiControlName"></param>
    public void Hide(UIControlName uiControlName)
    {
        if (Controls.TryGetValue(uiControlName, out UIControl control))
        {
            control.OnHide();
        }
    }

    /// <summary>
    /// Show the first ui control with the same name as the string,
    /// and hide the second put as parameter.
    /// </summary>
    /// <param name="uiControlName"></param>
    /// <param name="cToHide"></param>
    public void ShowAndHide(UIControlName uiControlName, UIControl cToHide)
    {
        if (uiControlName == cToHide.Name)
        {
            return;
        }

        if (Controls.TryGetValue(uiControlName, out UIControl control))
        {
            if (!control.gameObject.activeSelf) control.OnShow();
            if (cToHide.gameObject.activeSelf) cToHide.OnHide();
        }
        else
            Debug.Log(uiControlName + " or " + control.Name + " panel doesn't exist");
    }
}
