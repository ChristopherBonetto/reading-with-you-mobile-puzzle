using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Component attached to buttons in level selection.
/// </summary>
public class LevelButton : MonoBehaviour
{
    // Button (Unity object) references.
    public Button Button { get; private set; }
    public Image Image { get; private set; }

    public int LevelID { get; set; }

    private void Awake()
    {
        Button = GetComponent<Button>();
        Image = GetComponentInChildren<Image>();
    }

    /// <summary>
    /// Called when click a button
    /// </summary>
    public void OnLoadLevel()
    {
        FadeBetweenScene fade = UIManager.Instance.Controls[UIControlName.Fade] as FadeBetweenScene;

        #region Local Method
        // Show game panel and turn off level panel (or this).
        void ShowAndHideGameAndThis()
        {
            UIManager.Instance.ShowAndHide(UIControlName.InGame, UIManager.Instance.Controls[UIControlName.LevelSelection]);

            // pick level to enable.
            LevelSelectionWindow levelSelection = UIManager.Instance.Controls[UIControlName.LevelSelection] as LevelSelectionWindow;
            int currentWorld = levelSelection.CurrentWorld;
            GameManager.Instance.m_listOfWorlds[currentWorld].GetPooledObject(LevelID);
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
