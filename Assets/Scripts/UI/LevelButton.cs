using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component attached to buttons in level selection.
/// </summary>
public class LevelButton : MonoBehaviour
{
    /// <summary>
    /// Level's progressive number in current world
    /// </summary>
    public int LevelNumber { get; set; }

    private bool m_IsPlayable;
    /// <summary>
    /// if it's locked u can't play this level.
    /// </summary>
    public bool IsPlayable
    {
        get { return m_IsPlayable; }
        set
        {
            m_IsPlayable = value;

            if (m_IsPlayable)
                Image.color = Color.white;
            else
                Image.color = Color.black;
        }
    }

    /// <summary>
    /// Button component
    /// </summary>
    public Button Button { get; private set; }

    /// <summary>
    /// Image component
    /// </summary>
    public Image Image { get; private set; }

    #region Monobehaviour

    private void Awake()
    {
        Button = GetComponent<Button>();
        Image = GetComponentInChildren<Image>();
    }
	
    #endregion

    /// <summary>
    /// Called when click a button in level selection.
    /// </summary>
    public void OnLoadLevel()
    {
        if (IsPlayable)
        {
            // Set world background
            WorldBackgroundWindow backGround = UIManager.Instance.Controls[UIControlName.WorldBackground] as WorldBackgroundWindow;
            backGround.SetBackgroundImage(GameManager.Instance.Worlds[GameManager.Instance.CurrentWorld].Background);

            FadeBetweenScene fade = UIManager.Instance.Controls[UIControlName.Fade] as FadeBetweenScene;

            #region Local Method
            // Show game panel and turn off level panel (or this).
            void ShowAndHideGameAndThis()
            {
                UIManager.Instance.ShowAndHide(UIControlName.InGame, UIManager.Instance.Controls[UIControlName.LevelSelection]);

                // load level assigned to this button.
                GameManager.Instance.LoadLevel(LevelNumber);
            }

            // Turn off fade panel
            void HideFade()
            {
                UIManager.Instance.Hide(UIControlName.Fade);
            }
            #endregion

            //Store into delegate
            fade.FadeInCompleted = ShowAndHideGameAndThis;
            fade.FadeOutCompleted = HideFade;

            // Turn on fade panel
            UIManager.Instance.Show(UIControlName.Fade);
        }
    }
}
