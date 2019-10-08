using UnityEngine.SceneManagement;
using UnityEngine;
using System;

public class FadeBetweenScene : UIControl
{
    public override UIControlName Name => UIControlName.Fade;

    private Action m_OnFadeInComplete;
    public Action OnFadeInComplete
    {
        get { return m_OnFadeInComplete; }
        set { m_OnFadeInComplete = value; }
    }

    private Action m_OnFadeOutComplete;
    public Action OnFadeOutComplete
    {
        get { return m_OnFadeOutComplete; }
        set { m_OnFadeOutComplete = value; }
    }

    private Animator m_anim;

    protected void Awake()
    {
        m_anim = GetComponent<Animator>();
    }

    /// <summary>
    /// Called in animation event
    /// </summary>
    public void OnFadeInCompleted()
    {
        // execute a method putted in (when fade in is completed)
        OnFadeInComplete?.Invoke();
        OnFadeInComplete = null;

        m_anim.SetBool("isSceneLoaded", true);
    }

    /// <summary>
    /// Called in animation event
    /// </summary>
    public void OnFadeOutCompleted()
    {
        OnFadeOutComplete?.Invoke();
        OnFadeOutComplete = null;
    }
}
