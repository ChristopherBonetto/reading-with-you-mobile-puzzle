using UnityEngine;
using UnityEditor;

public class BlockoutReader : MonoBehaviour
{
	[SerializeField]
	private Level m_levelPrototype = null;

	[SerializeField]
	private PoolID m_poolIDPrototype = null;

	[ContextMenu("Parse")]
    public void ReadBlockout()
	{
		// Take scene reference
		string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().path;

		// Create map prefab pool ID and save
		PoolID poolMapID = Instantiate(m_poolIDPrototype);
		poolMapID.GenerateID();
		string poolMapIDName = sceneName.Replace(".unity", "_ID.asset");
		poolMapIDName = AssetDatabase.GenerateUniqueAssetPath(poolMapIDName);
		AssetDatabase.CreateAsset(poolMapID, poolMapIDName);

		// Create map prefab
		GameObject map = GameObject.Find("Map");
		for (int i = 0; i < map.transform.childCount; i++)
		{
			map.transform.GetChild(i).position = new Vector3(map.transform.GetChild(i).position.x, map.transform.GetChild(i).position.y - 1f, map.transform.GetChild(i).position.z);
		}
		string pfName = sceneName.Replace(".unity", ".prefab");
		pfName = AssetDatabase.GenerateUniqueAssetPath(pfName);

		// Assign pool ID SO
		PoolableObject poolMap = map.AddComponent<PoolableObject>();
		poolMap.uniqueID = AssetDatabase.LoadAssetAtPath<PoolID>(poolMapIDName);

		// Save and connect
		PrefabUtility.SaveAsPrefabAssetAndConnect(map, pfName, InteractionMode.AutomatedAction);

		// Create level info
		Level poolLevel = Instantiate(m_levelPrototype);
		string poolLevelName = sceneName.Replace(".unity", "_Info.asset");
		poolLevelName = AssetDatabase.GenerateUniqueAssetPath(poolLevelName);

		// Assign map prefab
		poolLevel.LevelPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(pfName);

		// Save
		AssetDatabase.CreateAsset(poolLevel, poolLevelName);
	}
}
