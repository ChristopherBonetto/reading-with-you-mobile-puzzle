using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component attached to buttons in level selection.
/// </summary>
public class LevelButton : MonoBehaviour
{
    /// <summary>
    /// Level assigne to this button.
    /// </summary>
    public Level Level { get; set; }
    /// <summary>
    /// Level's progressive number in current world
    /// </summary>
    public int LevelNumber { get; set; }

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

    //private void OnEnable()
    //{
    //    Image.sprite = Level.Icon;
    //    LevelNumber = Level.LevelID;
    //}
    #endregion

    /// <summary>
    /// Called when click a button in level selection.
    /// </summary>
    public void OnLoadLevel()
    {
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
        fade.OnFadeInComplete = ShowAndHideGameAndThis;
        fade.OnFadeOutComplete = HideFade;

        // Turn on fade panel
        UIManager.Instance.Show(UIControlName.Fade);
        GameManager.Instance.ReceiveLevelLoaded();
    }
}
