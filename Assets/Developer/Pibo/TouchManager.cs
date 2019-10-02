using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchManager : MonoBehaviour
{
	#region Variables

	private bool m_isHolding;

	private Block m_holdBlock;

	private Vector3 m_holdOffset;

	[SerializeField]
	private Camera m_gameCamera = null;

	[SerializeField]
	private float m_dragZ = -2f;

	[SerializeField]
	private float m_gameZ = 0f;

	/// <summary>
	/// Event on grabbing (true) and releasing (false)
	/// </summary>
	public Action<bool> OnGrab;

	private List<Block> m_levelBlocks = new List<Block>();

	#endregion

	#region Core loop

	private void Start()
	{
#if UNITY_EDITOR
		NullChecks();
#endif

		ResetAllBlocks();
	}

	void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(m_gameCamera.ScreenPointToRay(Input.mousePosition), out RaycastHit testHit))
			{
				Block testBlock = testHit.collider.gameObject.GetComponent<Block>();
				if (testBlock)
				{
					StartDrag(testBlock);
				}
				else
				{
					Tap();
				}
			}
		}

		if (Input.GetMouseButton(0) && m_isHolding)
		{
			Move();
		}

		if (Input.GetMouseButtonUp(0) && m_isHolding)
		{
			Release();
		}
	}

	/// <summary>
	/// Editor only
	/// </summary>
	private void NullChecks()
	{
		if (!m_gameCamera)
		{
			Debug.LogError("No game camera!");
		}
	}

	#endregion

	#region Level

	/// <summary>
	/// Subscribe to level block list
	/// </summary>
	/// <param name="newBlock">Block to add</param>
	public void AddBlock(Block newBlock)
	{
		m_levelBlocks.Add(newBlock);
	}

	/// <summary>
	/// Unsubscribe from level block list
	/// </summary>
	/// <param name="newBlock">Block to remove</param>
	public void RemoveBlock(Block oldBlock)
	{
		m_levelBlocks.Remove(oldBlock);
	}

	private void ResetBlock(Block block)
	{
		block.transform.position = new Vector3(Mathf.Round(UnityEngine.Random.Range(-4.5f, 4.5f)), -1.5f, m_dragZ);
		block.transform.localScale *= 0.8f;
		block.SetPhysicsInactive(true);
		block.enabled = false;
	}

	private void ResetAllBlocks()
	{
		Block[] blocks = m_levelBlocks.ToArray();
		for (int i = 0; i < blocks.Length; i++)
		{
			ResetBlock(blocks[i]);
		}
	}

	#endregion

	#region Dragging

	private void Tap()
	{

	}

	private void StartDrag(Block holdBlock)
	{
		// Holding state
		m_isHolding = true;
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
		Vector3 dragPosition = m_gameCamera.ScreenToWorldPoint(Input.mousePosition);
		dragPosition.z = m_holdBlock.transform.position.z;
		m_holdOffset = dragPosition - m_holdBlock.transform.position;
	}

	private void Move()
	{
		// Follow touch position maintaining grab point offset
		Vector3 dragPosition = m_gameCamera.ScreenToWorldPoint(Input.mousePosition);
		dragPosition.z = m_dragZ;
		m_holdBlock.transform.position = dragPosition - m_holdOffset;
	}

	private void Release()
	{
		// Snap to grid based on block size
		Vector3 releasePosition = m_holdBlock.transform.position;
		releasePosition.z = m_gameZ;
		float halfXSize = m_holdBlock.Size / 2f;
		releasePosition.x = Mathf.Round(releasePosition.x - halfXSize) + halfXSize;
		m_holdBlock.transform.position = releasePosition;

		if (releasePosition.x - halfXSize < -4f || releasePosition.x + halfXSize > 4f)
		{
			ResetBlock(m_holdBlock);
		}
		else
		{
			Collider[] testHits = Physics.OverlapBox(m_holdBlock.transform.position, new Vector3(m_holdBlock.Size, 1f, 3f) / 2f);
			//RaycastHit[] testHits = Physics.RaycastAll(m_gameCamera.ScreenPointToRay(Input.mousePosition));
			if (testHits.Length > 0)
			{
				for (int i = 0; i < testHits.Length; i++)
				{
					if (testHits[i].gameObject != m_holdBlock.gameObject)
					{
						ResetBlock(m_holdBlock);
						break;
					}
				}
			}
		}

		// Enable all rigidbodies
		OnGrab?.Invoke(false);

		// Free state
		m_holdBlock = null;
		m_isHolding = false;
	}

	#endregion
}
