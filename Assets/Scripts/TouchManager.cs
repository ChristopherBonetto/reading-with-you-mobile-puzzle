using UnityEngine;

public class TouchManager : MonoBehaviour
{
    private bool m_isHolding;

	private Block m_holdBlock;

	private Vector3 m_holdOffset;

	private float m_lastTapTime;

	[SerializeField]
	private Camera m_gameCamera = null;

	[SerializeField]
	private float m_dragZ = -2f;

	[SerializeField]
	private float m_gameZ = 0f;

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
					m_holdBlock = testBlock;
					StartDrag();
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

	private void Tap()
	{

	}

	private void StartDrag()
	{
		m_isHolding = true;
		m_lastTapTime = Time.time;

		Vector3 dragPosition = m_gameCamera.ScreenToWorldPoint(Input.mousePosition);
		dragPosition.z = m_holdBlock.transform.position.z;
		m_holdOffset = dragPosition - m_holdBlock.transform.position;
	}

	private void Move()
	{
		Vector3 dragPosition = m_gameCamera.ScreenToWorldPoint(Input.mousePosition);
		dragPosition.z = m_dragZ;
		m_holdBlock.transform.position = dragPosition - m_holdOffset;
	}

	private void Release()
	{
		Vector3 releasePosition = m_holdBlock.transform.position;
		releasePosition.z = m_gameZ;
		m_holdBlock.transform.position = releasePosition;

		m_holdBlock = null;
		m_isHolding = false;
	}
}
