using System;
using UnityEngine;

/// <summary>
/// A helper class used to smooth a movement to a destination
/// </summary>
public class LerpMover : MonoBehaviour
{
	#region Variables

	[Tooltip("Max speed reached when moving")]
	[SerializeField]
	private float m_maxSpeed = 20f;

	[Tooltip("Rotation speed when rotating to identity")]
	[SerializeField]
	private float m_rotationSpeed = 10f;

	private Vector3 m_destination;

	private Vector3 m_currentVelocity;

	private bool m_isMoving;

	private bool m_isRotating;

	private Action m_destinationCallback;

	#endregion

	/// <summary>
	/// Set destination position and start moving and rotating
	/// </summary>
	/// <param name="inDestination">Destination position</param>
	/// <param name="inDestinationCallback">Callback when reached destination</param>
	public void SetDestination(Vector3 inDestination, System.Action inDestinationCallback = null)
	{
		m_destination = inDestination;
		m_currentVelocity = Vector3.zero;
		m_isMoving = true;
		m_isRotating = true;
		m_destinationCallback = inDestinationCallback;
	}

    void Update()
    {
		// Move to destination
        if (m_isMoving)
		{
			transform.position = Vector3.SmoothDamp(transform.position, m_destination, ref m_currentVelocity, 0.1f, m_maxSpeed);
			if ((m_destination - transform.position).sqrMagnitude <= 0.0005f)
			{
				transform.position = m_destination;
				m_isMoving = false;
				m_destinationCallback?.Invoke();
			}
		}

		// Rotate to identity
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
