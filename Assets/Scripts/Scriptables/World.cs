using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "World name", menuName = "PangoBlocks/World")]
public class World : ScriptableObject
{
    /// <summary>
    /// World preview icon
    /// </summary>
    [Header("World Preview")]
    public Sprite Preview;

    [Header("World Background")]
    public Sprite Background;

	/// <summary>
	/// World main color tint
	/// </summary>
	public Color WorldColor;

    /// <summary>
    /// Container for the levels of this world.
    /// </summary>
    [Header("Levels container")]
    public Level[] EasyLevels;
    public Level[] HardLevels;
}
