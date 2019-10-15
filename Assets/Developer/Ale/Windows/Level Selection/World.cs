using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "World name", menuName = "PangoBlocks/World")]
public class World : ScriptableObject
{
    /// <summary>
    /// World preview icon
    /// </summary>
    [Header("World Preciew")]
    public Sprite Preview;

    /// <summary>
    /// Container for the levels of this world.
    /// </summary>
    [Header("Levels container")]
    public Level[] Levels;
}
