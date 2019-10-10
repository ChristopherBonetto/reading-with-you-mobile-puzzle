using UnityEngine;

public class Block : MonoBehaviour
{
	#region Variables

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

    private bool m_start = true;
    
	#endregion

	#region Core loop

	private void OnEnable()
    {
        if (!m_start)
        {
            Register();
        }
    }

	private void Start()
	{
#if UNITY_EDITOR
		NullChecks();
#endif
        Register();
        m_start = false;

        m_collisionTimeout = Time.fixedDeltaTime * 30;
    }

    private void Register()
    {
        m_touchManager = TouchManager.Instance;
        m_touchManager.AddBlock(this);
        m_touchManager.OnGrab += SetPhysicsInactive;
        m_touchManager.OnMovement += FreezeBlocks;
    }

    private void UnRegister()
    {
        m_touchManager.RemoveBlock(this);
        m_touchManager.OnGrab -= SetPhysicsInactive;
        m_touchManager.OnMovement -= FreezeBlocks;
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
	}

	#endregion

	#region Physics

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
		int blockCount = TouchManager.Instance.UnstableBlocks;
		TouchManager.Instance.UnstableBlocks = bInIsUnstable ? blockCount + 1 : blockCount - 1;

		// On start
		if (m_isUnstable)
		{
			m_lastCollisionTime = Time.time;
            Resnap();
		}
		// On stop
		else
		{
			if (m_transform.rotation.eulerAngles.sqrMagnitude >= 0.01f)
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

        Vector3 snapPosition = new Vector3();
        float halfXSize = Size / 2f;
        snapPosition.x = Mathf.Round(unstablePosition.x - halfXSize) + halfXSize;
        snapPosition.y = Mathf.Round(unstablePosition.y);
        snapPosition.z = m_gameZ;

        m_transform.position = snapPosition;
        m_transform.rotation = Quaternion.identity;

        m_rigidbody.velocity = new Vector3();
        m_rigidbody.angularVelocity = new Vector3();
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
