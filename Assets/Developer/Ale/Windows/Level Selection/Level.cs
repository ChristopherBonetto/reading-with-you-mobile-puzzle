using UnityEngine;

[CreateAssetMenu(menuName = "PangoBlocks/Level", fileName = "Name Level")]
public class Level : ScriptableObject
{
    [System.Serializable]
    public struct Block
    {
        public int ID;
        public float OffSetOnX;
    }

    [System.Serializable]
    public struct Goal
    {
        public int ID;
        public Vector3 Coord;
    }

    // *** UI *** \\
    [Header("UI field")]
    public Sprite Icon;
    public Sprite IconLocked;

    // *** Level *** \\
    [Header("Level reference")]
	public GameObject LevelPrefab;
    public PoolableObject LevelToPool => LevelPrefab.GetComponent<PoolableObject>();
    public int LevelID => LevelToPool.uniqueID.ID;

    //// *** Player *** \\
    //[Header("Player")]
    //public Transform PlayerSpawnPoint;

    // *** Blocks *** \\
    [Header("Block")]
    public Block[] Blocks;

    // *** Goal *** \\
    [Header("Goal")]
    public Goal GoalObject;

    // don't need anymore
    public int Index { get; set; }

}
