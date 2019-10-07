using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class takes care of panels for UI.
/// </summary>
public abstract class UIControl : MonoBehaviour
{
    public abstract UIControlName Name { get; }


    #region Monobehaviour
    protected virtual void Awake()
    {
        UIManager.Instance.Register(this);
    }

    protected virtual void Start()
    {
        this.gameObject.SetActive(false);
    }

    protected virtual void OnDestroy()
    {
        UIManager.Instance.Unregister(this);
    }
    #endregion

    #region Control method
    public virtual void OnShow()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void OnHide()
    {
        this.gameObject.SetActive(false);
    }
    #endregion
}
