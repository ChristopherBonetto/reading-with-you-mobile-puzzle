using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "PangoBlecks/Level", fileName = "Name Level")]
public class Level : ScriptableObject
{
    public Sprite Icon;
    public Sprite IconLocked;

    public GameObject LevelPrefab;

    public int Index { get; set; }
}
