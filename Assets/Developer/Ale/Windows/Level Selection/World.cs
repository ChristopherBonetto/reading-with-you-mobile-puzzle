using UnityEngine;
using System.Collections;

[CreateAssetMenu(fileName = "World name", menuName = "PangoBlocks/World")]
public class World : ScriptableObject
{
    public Sprite Preview;

    public Level[] Levels;
}
