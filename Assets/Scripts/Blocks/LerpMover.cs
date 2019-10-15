using UnityEngine;

public class LerpMover : MonoBehaviour
{
	[SerializeField]
	private float m_maxSpeed = 20f;

	[SerializeField]
	private float m_rotationSpeed = 10f;

	private Vector3 m_destination;

	private Vector3 m_velocity;

	private bool m_isMoving;

	private bool m_isRotating;

	public void SetDestination(Vector3 inDestination)
	{
		m_destination = inDestination;
		m_velocity = Vector3.zero;
		m_isMoving = true;
		m_isRotating = true;
	}

    void Update()
    {
        if (m_isMoving)
		{
			transform.position = Vector3.SmoothDamp(transform.position, m_destination, ref m_velocity, 0.1f, m_maxSpeed);
			if ((m_destination - transform.position).sqrMagnitude <= 0.0005f)
			{
				transform.position = m_destination;
				m_isMoving = false;
			}
		}

		if (m_isRotating)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.identity, Time.deltaTime * m_rotationSpeed);
			if (Quaternion.Angle(transform.rotation, Quaternion.identity) <= 0.5f)
			{
				transform.rotation = Quaternion.identity;
				m_isRotating = false;
			}
		}
	}
}
