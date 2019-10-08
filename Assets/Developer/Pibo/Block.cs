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

    [SerializeField]
    private float m_dragZ = -2f;

    [SerializeField]
    private float m_gameZ = 0f;

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
            Debug.Log(this + " vel "+ m_rigidbody.velocity.sqrMagnitude.ToString());
            Debug.Log(this + " ang "+ m_rigidbody.angularVelocity.sqrMagnitude.ToString());
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

    public void ResetBlock()
    {
        m_transform.position = new Vector3(Mathf.Round(UnityEngine.Random.Range(-4.5f, 4.5f)), -1.5f, m_dragZ);
        m_transform.rotation = Quaternion.identity;
        m_transform.localScale *= 0.8f;
        SetPhysicsInactive(true);
        enabled = false;
    }

    private void Resnap()
    {
        Vector3 unstablePosition = m_transform.position;

        Vector3 snapPosition = new Vector3(0f, 0f, m_gameZ);
        float halfXSize = Size / 2f;
        float halfYSize = 1f / 2f;
        snapPosition.x = Mathf.Round(unstablePosition.x - halfXSize) + halfXSize;
        snapPosition.y = Mathf.Round(unstablePosition.y - halfYSize) + halfYSize;

        m_transform.position = snapPosition;
        m_transform.rotation = Quaternion.identity;
    }

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
            Debug.Log(this + " rotation " + m_transform.rotation.eulerAngles.sqrMagnitude);
			if (m_transform.rotation.eulerAngles.sqrMagnitude >= 10f)
			//if (m_transform.rotation != Quaternion.identity)
			{
				ResetBlock();
			}
			else
			{
                Resnap();
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
