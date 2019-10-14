using UnityEngine;

[CreateAssetMenu(menuName = "PangoBlocks/Level", fileName = "Name Level")]
public class Level : ScriptableObject
{
    #region Struct
    /// <summary>
    /// Store all blocks used to complete the level.
    /// </summary>
    [System.Serializable]
    public struct Block
    {
        public int ID;
        public float OffSetOnX;
    }

    /// <summary>
    /// Store the coord of final object / goal.
    /// </summary>
    [System.Serializable]
    public struct Goal
    {
        public int ID;
        public Vector3 Coord;
    }
    #endregion

    [Header("UI field")]
    public Sprite Icon;
    public Sprite IconLocked;

    [Header("Level reference")]
	public GameObject LevelPrefab;
    public PoolableObject LevelToPool => LevelPrefab.GetComponent<PoolableObject>();
    public int LevelID => LevelToPool.uniqueID.ID;

    [Header("Player")]
    public Vector3 PlayerSpawnPoint;

    [Header("Block")]
    public Block[] Blocks;

    [Header("Goal")]
    public Goal GoalObject;
}
