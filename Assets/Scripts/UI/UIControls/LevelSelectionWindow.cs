using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manage the UI between level selectionand other control / panel
/// </summary>
public class LevelSelectionWindow : UIControl
{
    public override UIControlName Name => UIControlName.LevelSelection;

    [Header("WorldPreview")]
    [SerializeField] private Image m_worldPreview = null;

	private World[] Worlds => GameManager.Instance.Worlds;
	private int WorldsLength => Worlds.Length;


    [Header("Level Buttons")]
    [SerializeField]
    private LevelButton[] m_buttons = new LevelButton[0];

    private int CurrentLevel => GameManager.Instance.m_currentLevel;


    protected override void Start()
    {
        base.Start();

        GameManager.Instance.OnUpdateLevel += UpdateWorldAndLevelInfo;

        UpdateWorldAndLevelInfo();
    }

    /// <summary>
    /// Action invoked when main menu button is pressed.
    /// </summary>
    public void OnReturnToMainMenuButton()
    {
        GameManager.Instance.CurrentWorld = 0;
        UIManager.Instance.ShowAndHide(UIControlName.MainMenu, this);
    }

    #region Switch level (OnClick)
    /// <summary>
    /// Switch current world to the next one
    /// </summary>
    public void SwitchRigth()
    {
        GameManager.Instance.CurrentWorld++;
        GameManager.Instance.CurrentWorld = Mathf.Clamp(GameManager.Instance.CurrentWorld, 0, WorldsLength - 1);

        Debug.Log(GameManager.Instance.CurrentWorld);

        UpdateWorldAndLevelInfo();
    }

    /// <summary>
    /// Switch current worldto the previus one
    /// </summary>
    public void SwitchLeft()
    {
        GameManager.Instance.CurrentWorld--;
        GameManager.Instance.CurrentWorld = Mathf.Clamp(GameManager.Instance.CurrentWorld, 0, WorldsLength - 1);

        Debug.Log(GameManager.Instance.CurrentWorld);

        UpdateWorldAndLevelInfo();
    }
    #endregion

    /// <summary>
    /// Update when switch world
    /// </summary>
    public void UpdateWorldAndLevelInfo()
    {

		//@TEMP Not enough worlds in the list
		// Refactor world update
		if (WorldsLength <= GameManager.Instance.CurrentWorld)
		{
			return;
		}

        m_worldPreview.sprite = Worlds[GameManager.Instance.CurrentWorld].Preview;
        Mode mode = GameManager.Instance.Mode;

        switch (mode)
        {
            case Mode.Easy:
                for (int i = 0; i < m_buttons.Length; i++)
                {
                    // Turn off all buttons
                    if (i >= Worlds[GameManager.Instance.CurrentWorld].EasyLevels.Length)
                    {
                        // Turn off 
                        m_buttons[i].gameObject.SetActive(false);
                    }

                    // Turn on all buttons that exist in that world.
                    if (i < Worlds[GameManager.Instance.CurrentWorld].EasyLevels.Length /*&& Worlds[GameManager.Instance.CurrentWorld].EasyLevels[i] != null*/)
                    {
                        // Set a level (index) for each button
                        m_buttons[i].LevelNumber = i;

                        // Check if it's playable.
                        m_buttons[i].IsPlayable = Worlds[GameManager.Instance.CurrentWorld].EasyLevels[i].IsPlayable;

                        // Set sprite
                        m_buttons[i].Image.sprite = Worlds[GameManager.Instance.CurrentWorld].EasyLevels[i].Icon;

                        if (m_buttons[i].IsPlayable)
                            m_buttons[i].Image.color = Color.white;
                        else
                            m_buttons[i].Image.color = Color.black;

                        // Turn on
                        m_buttons[i].gameObject.SetActive(true);
                    }
                }
                break;

            case Mode.Hard:
                for (int i = 0; i < m_buttons.Length; i++)
                {
                    // Turn off all buttons.
                    if (i >= Worlds[GameManager.Instance.CurrentWorld].HardLevels.Length)
                    {
                        // Turn off
                        m_buttons[i].gameObject.SetActive(false);
                    }

                    // Turn on all button that exist in that level.
                    if (i < Worlds[GameManager.Instance.CurrentWorld].HardLevels.Length && Worlds[GameManager.Instance.CurrentWorld].HardLevels[i] != null)
                    {
                        // Set level (index) for each button
                        m_buttons[i].LevelNumber = i;

                        // Check if it's playable
                        m_buttons[i].IsPlayable = Worlds[GameManager.Instance.CurrentWorld].HardLevels[i].IsPlayable;

                        // Set sprite
                        m_buttons[i].Image.sprite = Worlds[GameManager.Instance.CurrentWorld].HardLevels[i].Icon;

                        if (m_buttons[i].IsPlayable)
                            m_buttons[i].Image.color = Color.white;
                        else
                            m_buttons[i].Image.color = Color.black;

                        // Turn on
                        m_buttons[i].gameObject.SetActive(true);
                    }
                }
                break;
        }
    }
}
