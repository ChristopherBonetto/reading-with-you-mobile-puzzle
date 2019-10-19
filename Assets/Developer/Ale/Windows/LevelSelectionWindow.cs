using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage the UI between level selectionand other control / panel
/// </summary>
public class LevelSelectionWindow : UIControl
{
    public override UIControlName Name => UIControlName.LevelSelection;


    //[Header("Worlds")]
	private World[] Worlds => GameManager.Instance.Worlds;
	private int WorldsLength => Worlds.Length;
	public int CurrentWorld { get { return GameManager.Instance.m_currentWorld; } private set { GameManager.Instance.m_currentWorld = value; } } //@TEMP @ALE change this variable.

    [Header("WorldPreview")]
    [SerializeField]
    private Image m_worldPreview = null;

    [Header("Buttons")]
    [SerializeField]
    private LevelButton[] m_buttons = new LevelButton[0];


    protected override void Start()
    {
        base.Start();

        UpdateWorldAndLevelInfo();
    }

    /// <summary>
    /// return to Main menu
    /// </summary>
    public void ReturnToMainMenu()
    {
        UIManager.Instance.ShowAndHide(UIControlName.MainMenu, this);
    }

    #region Switch level (OnClick)
    /// <summary>
    /// Switch current world to the next one
    /// </summary>
    public void SwitchRigth()
    {
        CurrentWorld++;
        CurrentWorld = Mathf.Clamp(CurrentWorld, 0, WorldsLength - 1);

        Debug.Log(CurrentWorld);

        UpdateWorldAndLevelInfo();
    }

    /// <summary>
    /// Switch current worldto the previus one
    /// </summary>
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
		//@TEMP Not enough worlds in the list
		// Refactor world update
		if (WorldsLength <= CurrentWorld)
		{
			return;
		}

        m_worldPreview.sprite = Worlds[CurrentWorld].Preview;

        for (int i = 0; i < m_buttons.Length; i++)
        {
            if (i >= Worlds[CurrentWorld].Levels.Length)
            {
                m_buttons[i].gameObject.SetActive(false);
            }

            if (i < Worlds[CurrentWorld].Levels.Length && Worlds[CurrentWorld].Levels[i] != null)
            {
				// Set a level for each button
				m_buttons[i].LevelNumber = i;
				m_buttons[i].Image.sprite = Worlds[CurrentWorld].Levels[i].Icon;
				m_buttons[i].gameObject.SetActive(true);
            }
        }
    }
}
