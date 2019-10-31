using UnityEngine;
using UnityEditor;

/// <summary>
/// A parser to read blockout scenes and store all relevant map info
/// </summary>
public class BlockoutReader : MonoBehaviour
{
#if UNITY_EDITOR
	[SerializeField]
	private Level m_levelInfoPrototype = null;

	[SerializeField]
	private PoolID m_poolIDPrototype = null;

	/// <summary>
	/// Read blockout scene info and save it into relevant assets
	/// </summary>
	[ContextMenu("Parse")]
    public void ReadBlockout()
	{
		if (!NullChecks())
		{
			Debug.LogError("Fatal: missing prototypes.");
			return;
		}

		// Take scene reference
		string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

		/*** Map pool ID */

		// Create map prefab pool ID and save
		PoolID poolMapID = Instantiate(m_poolIDPrototype);
		poolMapID.GenerateID();
		string poolMapIDName = sceneName.Replace(".unity", "_ID.asset");
		poolMapIDName = AssetDatabase.GenerateUniqueAssetPath(poolMapIDName);
		AssetDatabase.CreateAsset(poolMapID, poolMapIDName);

		/*** Map prefab */

		// Create map prefab
		GameObject map = GameObject.Find("Map");
		string pfName = "";
		if (map)
		{
			// Unconnect map prefab and remove pool id is already existing
			if (PrefabUtility.IsOutermostPrefabInstanceRoot(map))
			{
				PoolableObject poolComponent = GetComponent<PoolableObject>();
				if (poolComponent)
				{
					Destroy(poolComponent);
				}
				PrefabUtility.UnpackPrefabInstance(map, PrefabUnpackMode.OutermostRoot, InteractionMode.AutomatedAction);
				Debug.Log("Reconnecting map prefab in scene " + sceneName + ".");
			}

			pfName = sceneName.Replace(".unity", ".prefab");
			pfName = AssetDatabase.GenerateUniqueAssetPath(pfName);

			// Assign pool ID SO
			PoolableObject poolMap = map.AddComponent<PoolableObject>();
			poolMap.uniqueID = AssetDatabase.LoadAssetAtPath<PoolID>(poolMapIDName);

			// Save and connect
			PrefabUtility.SaveAsPrefabAssetAndConnect(map, pfName, InteractionMode.AutomatedAction); 
		}
		else
		{
			Debug.LogWarning("Missing map in scene " + sceneName + ".");
		}

		/*** Player */

		// Find player position
		BlockoutPlayer[] player = FindObjectsOfType<BlockoutPlayer>();
		Vector2 playerPosition = new Vector2();
		if (player.Length == 1 && player[0])
		{
			playerPosition.x = player[0].transform.position.x;
			playerPosition.y = player[0].transform.position.y;
		}
		else
		{
			Debug.LogWarning("Invalid player in scene " + sceneName + ": zero or multiple found.");
		}

		/*** Objective */

		// Find objective position
		BlockoutObjective[] objective = FindObjectsOfType<BlockoutObjective>();
		Vector2 objectivePosition = new Vector2();
		if (objective.Length == 1 && objective[0])
		{
			objectivePosition.x = objective[0].transform.position.x;
			objectivePosition.y = objective[0].transform.position.y;
		}
		else
		{
			Debug.LogWarning("Invalid objective in scene " + sceneName + ": zero or multiple found.");
		}

		/*** Blocks */

		// Find block type and position
		Block[] blocks = FindObjectsOfType<Block>();
		Level.BlockInfo[] blockInfos = new Level.BlockInfo[blocks.Length];
		for (int i = 0; i < blocks.Length; i++)
		{
			blockInfos[i].XCoord = blocks[i].transform.position.x;
			blockInfos[i].Scale = blocks[i].transform.localScale;
			PoolableObject blockID = blocks[i].GetComponent<PoolableObject>();
			if (blockID && blockID.uniqueID)
			{
				blockInfos[i].ID = blockID.uniqueID.ID;
			}
			else
			{
				Debug.LogWarning("Missing pool ID on block " + blocks[i].name + " from " + sceneName + ".");
			}
		}

		/*** Level info */

		// Create level info
		Level poolLevel = Instantiate(m_levelInfoPrototype);
		string poolLevelName = sceneName.Replace(".unity", "_Info.asset");
		poolLevelName = AssetDatabase.GenerateUniqueAssetPath(poolLevelName);

		// Assign data
		if (pfName != "")
		{
			poolLevel.LevelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(pfName); 
		}
		poolLevel.PlayerCoords = new Vector3(playerPosition.x, playerPosition.y);
		poolLevel.GoalObject.Coords = new Vector3(objectivePosition.x, objectivePosition.y);
		poolLevel.Blocks = blockInfos;

		// Save
		AssetDatabase.CreateAsset(poolLevel, poolLevelName);

		Debug.Log("Scene parsing completed for " + sceneName + ".");
	}

	/// <summary>
	/// Preparation test
	/// </summary>
	/// <returns>True if passed</returns>
	private bool NullChecks()
	{
		return (m_levelInfoPrototype &&
				m_poolIDPrototype);
	}
#endif
}
