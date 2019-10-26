using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes care of panels for UI.
/// </summary>
public abstract class UIControl : MonoBehaviour
{
    /// <summary>
    /// UI control name
    /// </summary>
    public abstract UIControlName Name { get; }


    #region Monobehaviour
    protected virtual void Start()
    {
        UIManager.Instance.Register(this);

        this.gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        UIManager.Instance.Unregister(this);
    }
    #endregion

    #region Control method
    /// <summary>
    /// Show gameObject
    /// </summary>
    public virtual void OnShow()
    {
        this.gameObject.SetActive(true);
    }

    /// <summary>
    /// Hide gameObject
    /// </summary>
    public virtual void OnHide()
    {
        this.gameObject.SetActive(false);
    }
    #endregion
}
