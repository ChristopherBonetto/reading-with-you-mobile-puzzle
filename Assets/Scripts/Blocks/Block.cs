using UnityEngine;

public class Block : MonoBehaviour
{
	#region Variables

	[SerializeField]
	private Rigidbody m_rigidbody = null;

	[SerializeField]
	private Transform m_transform = null;

	[SerializeField]
	private LerpMover m_lerpMover = null;

    private readonly RigidbodyConstraints m_moveConstraints = RigidbodyConstraints.FreezeAll;

	private readonly RigidbodyConstraints m_dropConstraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

	public float Size = 1f;

	public BlockShape Shape;

	private float m_lastCollisionTime;

	private float m_collisionTimeout;

    private bool m_isUnstable = false;

	private float m_inventoryX;
    
	#endregion

	#region Core loop

	private void OnEnable()
    {
        Register();
    }

	private void Start()
	{
#if UNITY_EDITOR
		NullChecks();
#endif

        m_collisionTimeout = Time.fixedDeltaTime * 20;
    }

    private void Register()
    {
		BlockManager.Instance.OnGrab += SetPhysicsInactive;
		GameManager.Instance.OnMovement += FreezeBlocks;
    }

    private void UnRegister()
    {
		BlockManager.Instance.OnGrab -= SetPhysicsInactive;
		GameManager.Instance.OnMovement -= FreezeBlocks;
    }

    private void Update()
    {
		if (m_isUnstable)
		{
			CheckStability(); 
		}
    }

    private void OnDisable()
	{
        UnRegister();
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

		if (!m_transform)
		{
			Debug.LogError(name + " has no transform reference!", this);
		}

		if (!m_lerpMover)
		{
			Debug.LogError(name + " has no lerp mover reference!", this);
		}
	}

	#endregion

	#region Physics

	public void LoadBlock(float inInventoryX, Vector3 scale)
	{
		m_inventoryX = inInventoryX;
		m_transform.localScale = scale;
		ResetBlock(true);
	}

	public void ResetBlock(bool bFast = false)
	{
		m_transform.position = new Vector3(m_transform.position.x, m_transform.position.y, BlockManager.Instance.InvZ);
		Vector3 destination = new Vector3(m_inventoryX, BlockManager.Instance.InvY, BlockManager.Instance.InvZ);
		if (bFast)
		{
			m_transform.position = destination;
		}
		else
		{
			m_lerpMover.SetDestination(destination);
		}
		m_transform.localScale *= 0.8f;
		SetPhysicsInactive(true);
		enabled = false;
	}

	private void FreezeBlocks()
	{
		SetPhysicsInactive(true);
	}

	private void SetPhysicsInactive(bool bInactive)
	{
		m_rigidbody.useGravity = !bInactive;
		m_rigidbody.constraints = !bInactive ? m_dropConstraints : m_moveConstraints;
	}

    private void CheckStability()
    {
        if (m_rigidbody.velocity.sqrMagnitude <= 0.01f &&
			m_rigidbody.angularVelocity.sqrMagnitude <=0.01f &&
			Time.time >= m_lastCollisionTime + m_collisionTimeout)
		{            
			SetUnstable(false);
		}
    }

	private void SetUnstable(bool bInIsUnstable)
	{
		if (m_isUnstable == bInIsUnstable)
		{
			return;
		}

		m_isUnstable = bInIsUnstable;
		int blockCount = BlockManager.Instance.UnstableBlocks;
		BlockManager.Instance.UnstableBlocks = bInIsUnstable ? blockCount + 1 : blockCount - 1;

		// On start
		if (m_isUnstable)
		{
			m_lastCollisionTime = Time.time;
            Resnap();
		}
		// On stop
		else
		{
			transform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
			if (angle >= 0.1f)
			{
				ResetBlock();
			}
			else
			{
                Resnap();
			}
		}
    }

    private void Resnap()
    {
        Vector3 unstablePosition = m_transform.position;

        Vector3 snapPosition = new Vector3();
        float halfXSize = Size / 2f;
        snapPosition.x = Mathf.Round(unstablePosition.x - halfXSize) + halfXSize;
        snapPosition.y = Mathf.Round(unstablePosition.y);
        snapPosition.z = BlockManager.Instance.GameZ;

        m_transform.position = snapPosition;
        m_transform.rotation = Quaternion.identity;

        m_rigidbody.velocity = new Vector3();
        m_rigidbody.angularVelocity = new Vector3();
    }

    private void OnCollisionEnter(Collision collision)
	{
		if (m_rigidbody.useGravity)
		{
			if (collision.gameObject.GetComponent<PlayerActions>() || collision.gameObject.GetComponent<FinalObjectActions>())
			{
				ResetBlock();
				return;
			}
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
