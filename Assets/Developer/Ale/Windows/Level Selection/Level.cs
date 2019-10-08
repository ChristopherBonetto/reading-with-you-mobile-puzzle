using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level name", menuName = "PangoBlocks/Level")]
public class Level : ScriptableObject
{
    #region Inspector
    [Header("UI field")]
    public Sprite Icon;

    [Header("Level")] [Tooltip("Drag here the entire level prefab")]
    public GameObject LevelPrefab;

    [Tooltip("All necessary block to complete the level")]
    public GameObject[] BlocksToPlace;
    #endregion
}
