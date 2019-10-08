using UnityEngine;

public class Block : MonoBehaviour
{
	#region Variables

	//@TEMP
	[SerializeField]
	private TouchManager m_touchManager = null;

	[SerializeField]
	private Rigidbody m_rigidbody = null;

	[SerializeField]
	private Transform m_transform = null;

	private readonly RigidbodyConstraints m_moveConstraints = RigidbodyConstraints.FreezeAll;

	private readonly RigidbodyConstraints m_dropConstraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

	public float Size = 1f;

	private float m_lastCollisionTime;

	private float m_collisionTimeout;

    private bool m_isUnstable = false;
    
	#endregion

	#region Core loop

	private void OnEnable()
	{
		m_touchManager.AddBlock(this);
		m_touchManager.OnGrab += SetPhysicsInactive;
		m_touchManager.OnMovement += FreezeBlocks;

		m_collisionTimeout = Time.fixedDeltaTime * 3;
	}

	private void Start()
	{
#if UNITY_EDITOR
		NullChecks();
#endif
	}

    private void Update()
    {
		if (m_isUnstable)
		{
			CheckStability(); 
		}
    }

    public void CheckStability()
    {
        if (m_rigidbody.velocity.sqrMagnitude <= 0.1f &&
			m_rigidbody.angularVelocity.sqrMagnitude <=0.1f &&
			Time.time >= m_lastCollisionTime + m_collisionTimeout)
		{
			SetUnstable(false);
		}
    }

    private void OnDisable()
	{
		m_touchManager.RemoveBlock(this);
		m_touchManager.OnGrab -= SetPhysicsInactive;
		m_touchManager.OnMovement -= FreezeBlocks;
	}

	/// <summary>
	/// Editor only
	/// </summary>
	private void NullChecks()
	{
		if (!m_rigidbody)
		{
			Debug.LogError(name + " has no rigidbody reference!", this);
		}
		if (!m_touchManager)
		{
			Debug.LogError(name + " has no touch manager reference!", this);
		}
	}

	#endregion

	#region Physics

	public void FreezeBlocks()
	{
		SetPhysicsInactive(true);
	}

	public void SetPhysicsInactive(bool bInactive)
	{
		m_rigidbody.useGravity = !bInactive;
		m_rigidbody.constraints = !bInactive ? m_dropConstraints : m_moveConstraints;
	}

	public void SetUnstable(bool bInIsUnstable)
	{
		if (m_isUnstable == bInIsUnstable)
		{
			return;
		}

		m_isUnstable = bInIsUnstable;
		int blockCount = TouchManager.Instance.UnstableBlocks;
		TouchManager.Instance.UnstableBlocks = bInIsUnstable ? blockCount + 1 : blockCount - 1;
		// On start
		if (m_isUnstable)
		{
			m_lastCollisionTime = Time.time;
		}
		// On stop
		else
		{
			if (m_transform.rotation.eulerAngles.sqrMagnitude >= 10f)
			//if (m_transform.rotation != Quaternion.identity)
			{
				TouchManager.Instance.ResetBlock(this);
			}
			else
			{
				// RE-SNAP
			}
		}
	}

	private void OnCollisionEnter(Collision collision)
	{
		if (m_rigidbody.useGravity)
		{
			SetUnstable(true);
			Block otherBlock = collision.gameObject.GetComponent<Block>();
			if (otherBlock)
			{
				otherBlock.SetUnstable(true);
			} 
		}
	}

	#endregion
}
