using System;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : Singleton<BlockManager>
{
	#region Variables

	/*** Current map references */

	private Block m_holdBlock;

	private Vector3 m_holdOffset;

	private List<Block> m_levelBlocks = new List<Block>();

	public int UnstableBlocks => GetUnstableCount();

	/*** General geometry parameters */

	[SerializeField]
	private float m_InvY = -0.2f;

	[SerializeField]
	private float m_InvZ = -1.7f;

	[SerializeField]
	private float m_GameZ = 0f;

	[SerializeField]
	private float m_XScale = 0.95f;

	[SerializeField]
	private float m_InvScale = 0.8f;

	public float InvY => m_InvY;

	public float InvZ => m_InvZ;

	public float GameZ => m_GameZ;

	public float XScale => m_XScale;

	public float InvScale => m_InvScale;

	/*** General physics parameters */

	[SerializeField]
	private float m_GravityMultiplier = 2f;

	[SerializeField]
	private float m_VelocityThreshold = 0.2f;

	[SerializeField]
	private float m_AngularVelocityThreshold = 0.01f;

	[SerializeField]
	private float m_AngleThreshold = 10f;

	[SerializeField]
	private int m_FixedTimeout = 5;

	public float GravityMultiplier => m_GravityMultiplier;

	public float VelocityThreshold => m_VelocityThreshold;

	public float AngularVelocityThreshold => m_AngularVelocityThreshold;

	public float AngleThreshold => m_AngleThreshold;

	public int FixedTimeout => m_FixedTimeout;

	/// <summary>
	/// Event on grabbing (true) and releasing (false)
	/// </summary>
	public Action<bool> OnGrab;

	#endregion

	private void Start()
	{
		Physics.gravity *= GravityMultiplier;
	}

	#region Level

	/// <summary>
	/// Get count of unstable blocks
	/// </summary>
	/// <returns>Number of unstable blocks</returns>
	private int GetUnstableCount()
	{
		int unstableCount = 0;
		{
			foreach (Block block in m_levelBlocks)
			{
				if (block.IsUnstable)
				{
					unstableCount++;
				}
			}
		}
		return unstableCount;
	}

	/// <summary>
	/// Get blocks from pool for current map
	/// </summary>
	/// <param name="blockInfo">Current map block info</param>
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
	}

	/// <summary>
	/// Return blocks to pool
	/// </summary>
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

	/// <summary>
	/// Try and pick a block to drag
	/// </summary>
	/// <param name="holdBlock">Touched block</param>
	/// <returns>True if block can be dragged</returns>
	public bool StartDrag(Block holdBlock)
	{
		// Holding state
		m_holdBlock = holdBlock;
		if (!m_holdBlock.enabled)
		{
			// Don't pick if block is moving to inventory
			if (m_holdBlock.IsMovingToInventory)
			{
				m_holdBlock = null;
				return false;
			}
			//m_holdBlock.transform.localScale *= 1.25f;
			m_holdBlock.enabled = true;
		}

		// Play pick up sound
		SoundManager.Instance.BlockManagerPlaySound(SoundManager.Instance.PickUpBlockAudioClip);

		// Disable all rigidbodies
		OnGrab?.Invoke(true);

		// Reset rotation
		m_holdBlock.transform.rotation = Quaternion.identity;

		// Save grab point offset
		Vector3 dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		dragPosition.z = m_holdBlock.transform.position.z;
		m_holdOffset = dragPosition - m_holdBlock.transform.position;

		return true;
	}

	/// <summary>
	/// Move a dragged block to follow input position
	/// </summary>
	public void Move()
	{
		// Follow touch position maintaining grab point offset
		Vector3 dragPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		dragPosition.z = m_InvZ;
		m_holdBlock.transform.position = dragPosition - m_holdOffset;
	}

	/// <summary>
	/// Release a dragged block resolving collisions
	/// </summary>
	public void Release()
	{
		// Snap to grid based on block size
		Vector3 releasePosition = m_holdBlock.transform.position;
		float halfXSize = m_holdBlock.Size / 2f;
		releasePosition.x = Mathf.Round(releasePosition.x - halfXSize) + halfXSize;
		releasePosition.y = Mathf.Round(releasePosition.y);
		releasePosition.z = m_GameZ;
		m_holdBlock.transform.position = releasePosition;

		// Check out of grid horizontal misplacement
		if (releasePosition.x - halfXSize < -4f || releasePosition.x + halfXSize > 4f)
		{
			// Return block to inventory if out of grid
            m_holdBlock.ResetBlock();
		}
		// Check collisions (reduced collider for a 5% allowance)
		else
		{
			Collider[] testHits = Physics.OverlapBox(m_holdBlock.transform.position, new Vector3(m_holdBlock.Size * 0.95f, 0.9f, 3f) / 2f);
			if (testHits.Length > 0)
			{
				for (int i = 0; i < testHits.Length; i++)
				{
					// Return block to inventory if any collider different from self
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
