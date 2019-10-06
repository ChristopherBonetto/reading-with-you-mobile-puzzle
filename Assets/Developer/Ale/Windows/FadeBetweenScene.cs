using UnityEngine.SceneManagement;
using UnityEngine;

public class FadeBetweenScene : UIControl
{
    public override string Name => "Fade";

    private Animator m_anim;

    protected override void Awake()
    {
        base.Awake();
        m_anim = GetComponent<Animator>();
    }

    public void OnFadeInCompleted()
    {
        m_anim.SetBool("isSceneLoaded", true);
    }

    // Change this with a delegate.
    public void OnFadeOutCompleted()
    {
        UIManager.Instance.ShowAndHide("Game", this);
    }
}
