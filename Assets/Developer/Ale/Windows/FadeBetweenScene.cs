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

    protected override void Awake()
    {
        base.Awake();
        m_anim = GetComponent<Animator>();
    }

    public void OnFadeInCompleted()
    {
        // delegate, store action
        OnFadeInComplete?.Invoke();

        m_anim.SetBool("isSceneLoaded", true);
    }

    // Change this with a delegate.
    public void OnFadeOutCompleted()
    {
        OnFadeOutComplete?.Invoke();
    }
}
