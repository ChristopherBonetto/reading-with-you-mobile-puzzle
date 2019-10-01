using UnityEngine;

public class Block : MonoBehaviour
{
	#region Variables

	[SerializeField]
	private Rigidbody m_rigidbody = null;

	private readonly RigidbodyConstraints m_moveConstraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;

	private readonly RigidbodyConstraints m_dropConstraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

	public float Size = 1f;

	#endregion

	#region Core loop

	private void Start()
	{
		if (m_rigidbody)
		{
			Debug.LogError(name + " has no rigidbody reference!"); 
		}
	}

	#endregion

	#region Physics

	public void SetPhysicsActive(bool bActive)
	{
		m_rigidbody.useGravity = bActive;
		m_rigidbody.constraints = bActive ? m_dropConstraints : m_moveConstraints;
	}

	#endregion
}
