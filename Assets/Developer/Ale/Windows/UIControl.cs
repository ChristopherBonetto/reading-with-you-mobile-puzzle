using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UIControl : MonoBehaviour
{
    public abstract string Name { get; }

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

    public virtual void OnShow()
    {
        this.gameObject.SetActive(true);
    }

    public virtual void OnHide()
    {
        this.gameObject.SetActive(false);
    }
}
