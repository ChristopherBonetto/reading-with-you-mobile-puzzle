using UnityEngine;
using System;

public class FadeBetweenScene : UIControl
{
    public override UIControlName Name => UIControlName.Fade;

    #region Properties
    /// <summary>
    /// Store every void method.
    /// Execute when fade in is completed.
    /// </summary>
    public Action FadeInCompleted;

    /// <summary>
    /// Store every void method.
    /// Execute when fade out is completed.
    /// </summary>
    public Action FadeOutCompleted;
    #endregion

    private Animator m_anim;


    protected void Awake()
    {
        m_anim = GetComponent<Animator>();
    }

    #region Animation event methods
    /// <summary>
    /// Called in animation event
    /// </summary>
    public void OnFadeInCompleted()
    {
        // execute a method putted in (when fade in is completed)
        FadeInCompleted?.Invoke();

        m_anim.SetBool("isSceneLoaded", true);
    }

    /// <summary>
    /// Called in animation event
    /// </summary>
    public void OnFadeOutCompleted()
    {
        FadeOutCompleted?.Invoke();
    }
    #endregion
}
