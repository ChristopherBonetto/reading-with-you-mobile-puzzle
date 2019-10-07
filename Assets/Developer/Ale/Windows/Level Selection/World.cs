using UnityEngine;
using System.Collections;

[System.Serializable]
public class Level
{
    public Sprite Icon;

    private LevelSelectionWindow m_levelSelection => UIManager.Instance.Controls[UIControlName.LevelSelection] as LevelSelectionWindow;

    public int Index { get; set; }
}

[CreateAssetMenu(fileName = "World name", menuName = "PangoBlocks/UI/World")]
public class World : ScriptableObject
{
    public Sprite Preview;

    public Level[] Levels;
}
