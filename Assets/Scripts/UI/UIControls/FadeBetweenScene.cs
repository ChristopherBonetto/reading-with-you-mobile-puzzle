using UnityEngine;
using UnityEngine.UI;
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

	/// <summary>
	/// Color tint to apply to fade image.
	/// </summary>
	public Color FadeTint;
    #endregion

    private Animator m_anim;

	private Image m_image;

    protected void Awake()
    {
        m_anim = GetComponent<Animator>();
		m_image = GetComponent<Image>();
    }

	private void OnEnable()
	{
		m_image.color = FadeTint;
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
