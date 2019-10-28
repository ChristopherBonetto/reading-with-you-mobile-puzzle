using UnityEngine;

public class Floater : MonoBehaviour
{
	private float m_startY;

	[SerializeField]
	private float m_intensity = 0.05f;

	[SerializeField]
	private Transform m_transform = null;

	void Start()
	{
		m_transform = transform;
		m_startY = m_transform.position.y;
	}

	void Update()
	{
		Vector3 newPosition = m_transform.position;
		newPosition.y = m_startY + (Mathf.Sin(Time.time) * m_intensity * Mathf.Cos(Time.time));
		m_transform.position = newPosition;
	}

	public void ResetPosition()
	{
		Vector3 resetPosition = m_transform.position;
		resetPosition.y = m_startY;
		m_transform.position = resetPosition;
	}

	public void ResetPosition(Vector3 startPosition)
	{
		m_startY = startPosition.y;
		ResetPosition();
	}
}
