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

	#endregion

	#region Core loop

	private void Start()
	{
		if (!m_gameCamera)
		{
			Debug.LogError("No game camera!");
		}
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

		// Disable rigidbody
		m_holdBlock.SetPhysicsActive(false);

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

		// Enable rigidbody
		m_holdBlock.SetPhysicsActive(true);

		// Free state
		m_holdBlock = null;
		m_isHolding = false;
	}

	#endregion
}
