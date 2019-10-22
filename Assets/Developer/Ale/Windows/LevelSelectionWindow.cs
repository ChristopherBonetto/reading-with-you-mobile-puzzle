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
    private int CurrentLevel => GameManager.Instance.m_currentLevel;

    [Header("WorldPreview")]
    [SerializeField]
    private Image m_worldPreview = null;

    [Header("Buttons")]
    [SerializeField]
    private LevelButton[] m_buttons = new LevelButton[0];


    protected override void Start()
    {
        base.Start();

        GameManager.Instance.OnUpdateLevel += UpdateWorldAndLevelInfo;

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
        Mode mode = GameManager.Instance.Mode;

		//@TEMP Not enough worlds in the list
		// Refactor world update
		if (WorldsLength <= GameManager.Instance.CurrentWorld)
		{
			return;
		}

        m_worldPreview.sprite = Worlds[GameManager.Instance.CurrentWorld].Preview;

        for (int i = 0; i < m_buttons.Length; i++)
        {
            if (mode == Mode.Easy)
            {
                Debug.Log("easy");

                if (i >= Worlds[GameManager.Instance.CurrentWorld].EasyLevels.Length)
                {
                    m_buttons[i].Image.sprite = Worlds[GameManager.Instance.CurrentWorld].EasyLevels[CurrentLevel].IconLocked;
                    m_buttons[i].gameObject.SetActive(false);
                    m_buttons[i].Button.enabled = false;
                }

                if (i < Worlds[GameManager.Instance.CurrentWorld].EasyLevels.Length && Worlds[GameManager.Instance.CurrentWorld].EasyLevels[i] != null)
                {
				    // Set a level for each button
				    m_buttons[i].LevelNumber = i;
                    m_buttons[i].Button.enabled = true;
				    m_buttons[i].gameObject.SetActive(true);

				    m_buttons[i].IsPlayable = Worlds[GameManager.Instance.CurrentWorld].EasyLevels[i].IsPlayable;

                    if (m_buttons[i].IsPlayable)
				        m_buttons[i].Image.sprite = Worlds[GameManager.Instance.CurrentWorld].EasyLevels[i].Icon;
                }
            }
            else if (mode == Mode.Hard)
            {
                Debug.Log("hard");

                if (i >= Worlds[GameManager.Instance.CurrentWorld].HardLevels.Length)
                {
                    m_buttons[i].gameObject.SetActive(false);
                    m_buttons[i].Image.sprite = Worlds[GameManager.Instance.CurrentWorld].HardLevels[CurrentLevel].IconLocked;
                    m_buttons[i].Button.enabled = false;
                }

                if (i < Worlds[GameManager.Instance.CurrentWorld].HardLevels.Length && Worlds[GameManager.Instance.CurrentWorld].HardLevels[i] != null)
                {
                    // Set a level for each button
                    m_buttons[i].LevelNumber = i;
                    m_buttons[i].Button.enabled = true;
                    m_buttons[i].gameObject.SetActive(true);

				    m_buttons[i].IsPlayable = Worlds[GameManager.Instance.CurrentWorld].HardLevels[i].IsPlayable;

                    if (m_buttons[i].IsPlayable)
                        m_buttons[i].Image.sprite = Worlds[GameManager.Instance.CurrentWorld].HardLevels[i].Icon;
                }
            }
        }
    }
}
