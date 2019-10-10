using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectionWindow : UIControl
{
    // it is equal to write: string Name { get { return "..." } }
    public override UIControlName Name => UIControlName.LevelSelection;

    // Scriptable
    public World[] Worlds;

    public int CurrentWorld { get; private set; } = 0;

    public int WorldsLength => Worlds.Length;

    [SerializeField]
    private Image m_worldPreview;

    [SerializeField]
    private Button[] m_buttons;


    protected override void Start()
    {
        base.Start();

        UpdateWorldAndLevelInfo();

        SetIndexOfTheLevel();
    }

    /// <summary>
    /// CAlled when click a level.
    /// </summary>
    public void OnLoadLevel()
    {
        FadeBetweenScene fade = UIManager.Instance.Controls[UIControlName.Fade] as FadeBetweenScene;

        #region Local Method
        // Show game panel and turn off level panel (or this).
        void ShowAndHideGameAndThis()
        {
            UIManager.Instance.ShowAndHide(UIControlName.InGame, this);
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

    #region Switch level (OnClick)
    public void SwitchRigth()
    {
        CurrentWorld++;
        CurrentWorld = Mathf.Clamp(CurrentWorld, 0, WorldsLength - 1);

        Debug.Log(CurrentWorld);

        UpdateWorldAndLevelInfo();
    }

    public void SwitchLeft()
    {
        CurrentWorld--;
        CurrentWorld = Mathf.Clamp(CurrentWorld, 0, WorldsLength - 1);

        Debug.Log(CurrentWorld);

        UpdateWorldAndLevelInfo();
    }
    #endregion

    /// <summary>
    /// Update when switch world
    /// </summary>
    private void UpdateWorldAndLevelInfo()
    {
        m_worldPreview.sprite = Worlds[CurrentWorld].Preview;

        for (int i = 0; i < m_buttons.Length; i++)
        {
            if (i >= Worlds[CurrentWorld].Levels.Length)
            {
                m_buttons[i].gameObject.SetActive(false);
            }

            if (i < Worlds[CurrentWorld].Levels.Length && Worlds[CurrentWorld].Levels[i] != null)
            {
                // Active and set icon
                m_buttons[i].image.sprite = Worlds[CurrentWorld].Levels[i].Icon;
                m_buttons[i].gameObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Set the level's index.
    /// </summary>
    public void SetIndexOfTheLevel()
    {
        int index = 0;

        for (int i = 0; i < WorldsLength; i++)
        {
            for (int j = 0; j < Worlds[i].Levels.Length; j++)
            {
                // assign the index of the level "j", of the world "i".
                Worlds[i].Levels[j].Index = index;

                //Debug.Log(Worlds[i].Levels[j].Index);

                index++;
            }
        }
    }
}
