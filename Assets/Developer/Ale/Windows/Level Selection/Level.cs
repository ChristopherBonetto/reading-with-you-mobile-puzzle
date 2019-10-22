using UnityEngine;

[CreateAssetMenu(menuName = "PangoBlocks/Level", fileName = "Name Level")]
public class Level : ScriptableObject
{
    #region Struct
    /// <summary>
    /// Store all blocks used to complete the level.
    /// </summary>
    [System.Serializable]
    public struct BlockInfo
    {
        public int ID;
        public float XCoord;
		public Vector3 Scale;
    }

    /// <summary>
    /// Store the coord of final object / goal.
    /// </summary>
    [System.Serializable]
    public struct GoalInfo
    {
        public int ID;
        public Vector3 Coords;
    }
    #endregion

    [Header("UI field")]
    public Sprite Icon;
    public Sprite IconLocked;

    [Header("Level reference")]
	public GameObject LevelPrefab;
    public PoolableObject LevelToPool => LevelPrefab.GetComponent<PoolableObject>();
    public int LevelID => LevelToPool.uniqueID.ID;
    public bool IsPlayable;

    [Header("Player")]
    public Vector3 PlayerCoords;

    [Header("Block")]
    public BlockInfo[] Blocks;

    [Header("Goal")]
    public GoalInfo GoalObject;


    // Methods
    public void Reset()
    {
        IsPlayable = false;
    }
}
