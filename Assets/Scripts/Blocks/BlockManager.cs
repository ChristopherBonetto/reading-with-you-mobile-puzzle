using System;
using System.Collections.Generic;
using UnityEngine;

public enum BlockShape
{
	Void = 0,
	Cube = 1,
	Ramp = 2,
	Rectangle = 3,
	TrapezoidA = 4,
	TrapezoidB = 5,
	TrapezoidAVar = 6,
	TrapezoidBVar = 7
}

public class BlockManager : Singleton<BlockManager>
{
    #region Variables

	private Block m_holdBlock;

	private Vector3 m_holdOffset;

	[SerializeField]
	private float m_InvY = -0.4f;

	[SerializeField]
	private float m_InvZ = -1.7f;

	[SerializeField]
	private float m_GameZ = 0f;

	public float InvY => m_InvY;

	public float InvZ => m_InvZ;

	public float GameZ => m_GameZ;
	
	/// <summary>
	/// Event on grabbing (true) and releasing (false)
	/// </summary>
	public Action<bool> OnGrab;

	private List<Block> m_levelBlocks = new List<Block>();

	public int UnstableBlocks;

    #endregion

	#region Level

    /// <summary>
    /// Reset level blocks to inventory
    /// </summary>
	public void ResetAllBlocks()
	{
		foreach (Block block in m_levelBlocks)
		{
			block.ResetBlock(true);
		}
	}

	public void LoadBlocks(Level.BlockInfo[] blockInfo)
	{
		foreach (Level.BlockInfo block in blockInfo)
		{
			Block newBlock = ObjectPooler.Instance.GetPooledObject(block.ID).GetComponent<Block>();
			if (newBlock)
			{
				newBlock.LoadBlock(block.XCoord, block.Scale);
				newBlock.gameObject.SetActive(true);
				m_levelBlocks.Add(newBlock);
			}
		}
		UnstableBlocks = 0;
	}

	public void UnloadBlocks()
	{
		foreach (Block block in m_levelBlocks)
		{
			block.gameObject.SetActive(false);
		}
		m_levelBlocks.Clear();
	}

	#endregion

	#region Dragging

	public void StartDrag(Block holdBlock)
	{
		// Holding state
		m_holdBlock = holdBlock;
		if (!m_holdBlock.enabled)
		{
			m_holdBlock.transform.localScale *= 1.25f;
			m_holdBlock.enabled = true;
		}

        // Disable all rigidbodies
        OnGrab?.Invoke(true);

		// Reset rotation
		m_holdBlock.transform.rotation = Quaternion.identity;

		// Save grab point offset
		Vector3 dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		dragPosition.z = m_holdBlock.transform.position.z;
		m_holdOffset = dragPosition - m_holdBlock.transform.position;
	}

	public void Move()
	{
		// Follow touch position maintaining grab point offset
		Vector3 dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		dragPosition.z = m_InvZ;
		m_holdBlock.transform.position = dragPosition - m_holdOffset;
	}

	public void Release()
	{
		// Snap to grid based on block size
		Vector3 releasePosition = m_holdBlock.transform.position;
		releasePosition.z = m_GameZ;
		float halfXSize = m_holdBlock.Size / 2f;
		releasePosition.x = Mathf.Round(releasePosition.x - halfXSize) + halfXSize;
		m_holdBlock.transform.position = releasePosition;

		// Check out of grid
		if (releasePosition.x - halfXSize < -4f || releasePosition.x + halfXSize > 4f)
		{
            m_holdBlock.ResetBlock();
		}
		// Check collisions
		else
		{
			Collider[] testHits = Physics.OverlapBox(m_holdBlock.transform.position, new Vector3(m_holdBlock.Size * 0.95f, 1f, 3f) / 2f);
			if (testHits.Length > 0)
			{
				for (int i = 0; i < testHits.Length; i++)
				{
					if (testHits[i].gameObject != m_holdBlock.gameObject)
					{
                        m_holdBlock.ResetBlock();
						break;
					}
				}
			}
		}

		// Enable all rigidbodies
		OnGrab?.Invoke(false);

        // Free state
		m_holdBlock = null;
	}

	#endregion
}
