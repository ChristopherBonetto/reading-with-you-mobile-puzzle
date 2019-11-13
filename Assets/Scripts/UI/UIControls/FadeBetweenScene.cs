using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

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

    private float m_StartFadingTime;
    [SerializeField] private float m_FadingDuration;

    protected void Awake()
    {
        m_anim = GetComponent<Animator>();
		m_image = GetComponent<Image>();
    }

	private void OnEnable()
	{
        m_image.color = FadeTint;
    }

    public override void OnShow()
    {
        gameObject.SetActive(true);

        StartCoroutine("FadeIn");
    }

    // Use courutine for fade 

    public IEnumerator FadeIn()
    {
        m_StartFadingTime = Time.time;
        float t = 0;

        while (t < 1)
        {
            t = (Time.time - m_StartFadingTime) / m_FadingDuration;
            m_image.color = Color.Lerp(Color.clear, FadeTint, t);
            yield return null;
        }

        FadeInCompleted?.Invoke();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine("FadeOut");
    }

    public IEnumerator FadeOut()
    {
        m_StartFadingTime = Time.time;
        float t = 0;

        while (t < 1)
        {
            t = (Time.time - m_StartFadingTime) / m_FadingDuration;
            m_image.color = Color.Lerp(FadeTint, Color.clear, t);
            yield return null;
        }

        FadeOutCompleted?.Invoke();
        yield return null;
    }
}
