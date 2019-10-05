using System.Collections.Generic;
using UnityEngine;

public class UIManager : SingletonBehaviour<UIManager>
{
    private Dictionary<string, UIControl> m_controls = new Dictionary<string, UIControl>();

    /// <summary>
    /// Register a ui control
    /// </summary>
    /// <param name="uiControl"></param>
    public void Register(UIControl uiControl)
    {
        if (uiControl != null && !m_controls.ContainsKey(uiControl.Name))
        {
            Debug.Log("Register: " + uiControl.Name);
            m_controls.Add(uiControl.Name, uiControl);
        }
    }

    /// <summary>
    /// Unregister a ui control
    /// </summary>
    /// <param name="uiControl"></param>
    public void Unregister(UIControl uiControl)
    {
        if (uiControl != null && !m_controls.ContainsKey(uiControl.Name))
        {
            m_controls.Remove(uiControl.Name);
        }
    }

    /// <summary>
    /// Show a ui control
    /// </summary>
    /// <param name="uiControlName"></param>
    public void Show(string uiControlName)
    {
        if (m_controls.TryGetValue(uiControlName, out UIControl control))
        {
            control.OnShow();
        }
    }

    /// <summary>
    /// Hide a ui control
    /// </summary>
    /// <param name="uiControlName"></param>
    public void Hide(string uiControlName)
    {
        if (m_controls.TryGetValue(uiControlName, out UIControl control))
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
    public void ShowAndHide(string uiControlName, UIControl cToHide)
    {
        if (uiControlName == cToHide.Name)
        {
            return;
        }

        if (m_controls.TryGetValue(uiControlName, out UIControl control))
        {
            if (!control.gameObject.activeSelf) control.OnShow();
            if (cToHide.gameObject.activeSelf) cToHide.OnHide();
        }
        else
            Debug.Log(uiControlName + " or " + control.Name + " panel doesn't exist");
    }
}
