using UnityEngine;

public class Block : MonoBehaviour
{
	#region Variables

	[SerializeField]
	private Rigidbody m_rigidbody = null;

	[SerializeField]
	private Transform m_transform = null;

	[SerializeField]
	private Transform m_meshTransform = null;

	[SerializeField]
	private LerpMover m_lerpMover = null;

    private readonly RigidbodyConstraints m_moveConstraints = RigidbodyConstraints.FreezeAll;

	private readonly RigidbodyConstraints m_dropConstraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

	public float Size = 1f;

	private float m_lastCollisionTime;

    private bool m_isUnstable = false;

	private bool m_isMovingToInventory;

	public bool IsUnstable => m_isUnstable;

	public bool IsMovingToInventory => m_isMovingToInventory;

	private float m_inventoryX;

	#endregion

	#region Core loop

	private void OnEnable()
    {
        Register();
		m_transform.localScale /= BlockManager.Instance.InvScale;
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

    private void OnDisable()
	{
        UnRegister();
		m_transform.localScale *= BlockManager.Instance.InvScale;
		m_isUnstable = false;
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

#if UNITY_EDITOR
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

		if (!m_meshTransform)
		{
			Debug.LogError(name + " has no mesh transform reference!", this);
		}

		if (!m_lerpMover)
		{
			Debug.LogError(name + " has no lerp mover reference!", this);
		}
	}
#endif

#endregion

	#region Physics

	/// <summary>
	/// Prepare block for map
	/// </summary>
	/// <param name="inInventoryX">Block position in the inventory</param>
	/// <param name="scale">Block scale for reflections</param>
	public void LoadBlock(float inInventoryX, Vector3 scale)
	{
		m_inventoryX = inInventoryX;
		scale.x = (Mathf.Round(Mathf.Abs(scale.x)) - 0.05f / Size) * Mathf.Sign(scale.x);
		m_transform.localScale = scale;
		Vector3 meshScale = new Vector3(1 / Mathf.Abs(scale.x), 1f, 1f);
		m_meshTransform.localScale = meshScale;
		m_transform.localScale *= BlockManager.Instance.InvScale;
		ResetBlock(true);
	}

	/// <summary>
	/// Move block to inventory
	/// </summary>
	/// <param name="bFast">True to move immediately, false to play animation and sound</param>
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
			// Moving animation
            m_isMovingToInventory = true;
			m_lerpMover.SetDestination(destination, () => m_isMovingToInventory = false);

			// Play reset sound
			SoundManager.Instance.BlockManagerPlaySound(SoundManager.Instance.BlockMissplacementAudioClip);
		}

		SetPhysicsInactive(true);
		enabled = false;
	}

	/// <summary>
	/// Stop all physics on player movement
	/// </summary>
	private void FreezeBlocks()
	{
		SetPhysicsInactive(true);
	}

	/// <summary>
	/// Toggle physics and stability
	/// </summary>
	/// <param name="bInactive">True to stop</param>
	private void SetPhysicsInactive(bool bInactive)
	{
		m_rigidbody.useGravity = !bInactive;
		m_rigidbody.constraints = !bInactive ? m_dropConstraints : m_moveConstraints;
		if (!bInactive)
		{
			SetUnstable(true);
		}
	}

	/// <summary>
	/// Check stability conditions
	/// </summary>
    private void CheckStability()
    {
        if (m_rigidbody.velocity.sqrMagnitude <= BlockManager.Instance.VelocityThreshold &&
			m_rigidbody.angularVelocity.sqrMagnitude <= BlockManager.Instance.AngularVelocityThreshold &&
			Time.time >= m_lastCollisionTime + Time.fixedDeltaTime * BlockManager.Instance.FixedTimeout)
		{            
			SetUnstable(false);
		}
    }

	/// <summary>
	/// Toggle stability and clear conditions
	/// </summary>
	/// <param name="bInIsUnstable">True to start physics</param>
	private void SetUnstable(bool bInIsUnstable)
	{
		if (m_isUnstable == bInIsUnstable)
		{
			return;
		}

		m_isUnstable = bInIsUnstable;

		// On physics start
		if (m_isUnstable)
		{
			m_lastCollisionTime = Time.time;
            Resnap();
		}
		// On physics stop
		else
		{
			transform.rotation.ToAngleAxis(out float angle, out Vector3 axis);
			if (angle >= BlockManager.Instance.AngleThreshold)
			{
				ResetBlock();
                
			}
			else
			{
                Resnap();
				SetPhysicsInactive(true);
			}
		}
    }

	/// <summary>
	/// Snap to grid and reset rigidbody
	/// </summary>
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
			// Reset if touched player or objective
			if (collision.gameObject.GetComponent<PlayerActions>() || collision.gameObject.GetComponent<FinalObjectActions>())
			{
				ResetBlock();
				return;
			}

			// Start stability check
			SetUnstable(true);

			// Propagate instability if touched other block
			Block otherBlock = collision.gameObject.GetComponent<Block>();
			if (otherBlock)
			{
				otherBlock.SetUnstable(true);
			} 
		}
	}
	
	#endregion
}
