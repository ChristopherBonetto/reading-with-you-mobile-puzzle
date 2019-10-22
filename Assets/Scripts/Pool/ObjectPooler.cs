using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : Singleton<ObjectPooler>
{
	/* Inner class */
	[System.Serializable]
	private class ObjectPoolItem
	{
		[SerializeField]
		public PoolableObject ObjectPrefab = null;

		[SerializeField]
		public int BasePoolSize = 10;

		[SerializeField]
		public bool CanExpand = true;

		[SerializeField]
		public int PoolExpandSize = 1;

		[SerializeField]
		public int MaxPoolSize = 15;

		[SerializeField]
		public PoolID uniqueID = null;

		[HideInInspector]
		public int CurrentCount = 0;
	}

	[SerializeField]
	private List<ObjectPoolItem> m_poolItems = new List<ObjectPoolItem>();

	private List<PoolableObject> m_objectPool = new List<PoolableObject>();

	public void StartPooling()
	{
		foreach (ObjectPoolItem item in m_poolItems)
		{
			for (int i = 0; i < item.BasePoolSize; i++)
			{
				CreateNewObject(item);
			}
		}
	}

	public bool AddPoolItem(PoolableObject newPoolObject, int basePoolsize, bool bCanExpand = true)
	{
		if (!newPoolObject || ContainsPoolItem(newPoolObject.uniqueID.ID))
		{
			return false;
		}

		ObjectPoolItem newItem = new ObjectPoolItem();
		newItem.ObjectPrefab = newPoolObject;
		newItem.uniqueID = newPoolObject.uniqueID;
		newItem.BasePoolSize = basePoolsize;
		newItem.CanExpand = bCanExpand;
		m_poolItems.Add(newItem);
		Debug.Log(newItem.ObjectPrefab);
		// Pool has been previously initialized, add new item
		if (m_objectPool.Count > 0)
		{
			CreateNewObject(newItem);
		}

		return true;
	}

	private bool ContainsPoolItem(int poolID)
	{
		foreach (ObjectPoolItem item in m_poolItems)
		{
			if (item.uniqueID.ID == poolID)
			{
				return true;
			}
		}
		return false;
	}
    
    /* Pooled objects might have an interface to Reset when they aren't needed any more */

    public GameObject GetPooledObject(int poolID)
	{
		for (int i = 0; i < m_objectPool.Count; i++)
		{
			if (m_objectPool[i].uniqueID.ID == poolID)
			{
				GameObject go = m_objectPool[i].gameObject;

				if (!go.activeInHierarchy)
				{
					return go; 
				}
			}
		}

		for (int i = 0; i < m_poolItems.Count; i++)
		{
			// I may decide not to expand pool for some categories, e.g. sound or unreliable fx
			if (m_poolItems[i].uniqueID.ID == poolID && m_poolItems[i].CanExpand)
			{
				PoolableObject obj = null;
				// Warn to review design
				for (int j = 0; j < m_poolItems[i].PoolExpandSize && m_poolItems[i].CurrentCount < m_poolItems[i].MaxPoolSize; j++)
				{
					obj = CreateNewObject(m_poolItems[i]);
				}
				return obj ? obj.gameObject : null;
			}
		}
		return null;
	}

	public int GetPoolSize(int poolID)
	{
		for (int i = 0; i < m_poolItems.Count; i++)
		{
			if (m_poolItems[i].uniqueID.ID == poolID)
			{
				return m_poolItems[i].CurrentCount;
			}
		}
		return 0;
	}

	private PoolableObject CreateNewObject(ObjectPoolItem item)
	{
		PoolableObject prefab = item.ObjectPrefab;
		if (prefab)
		{
			PoolableObject obj = Instantiate(prefab);
            obj.transform.parent = gameObject.transform;
			obj.gameObject.SetActive(false);
			item.CurrentCount++;
			m_objectPool.Add(obj);
			return obj;
		}
		else
		{
			return null;
		}
	}
}
