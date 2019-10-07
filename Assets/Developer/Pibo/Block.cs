using UnityEngine;

public class Block : MonoBehaviour
{
	#region Variables

	//@TEMP
	[SerializeField]
	private TouchManager m_touchManager = null;

	[SerializeField]
	private Rigidbody m_rigidbody = null;

	private readonly RigidbodyConstraints m_moveConstraints = RigidbodyConstraints.FreezeAll;

	private readonly RigidbodyConstraints m_dropConstraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezePositionZ;

	public float Size = 1f;

    private bool m_isDropped = false;
    private bool m_hasCollided = false;
    [SerializeField] private float m_checkRotationTimer;
    private float m_timer = 0;
    private Quaternion m_startingRotation;

    public bool m_isInTheRightPosition { get; private set; } = false;
    
	#endregion

	#region Core loop

	private void OnEnable()
	{
		m_touchManager.AddBlock(this);
		m_touchManager.OnGrab += SetPhysicsInactive;
	}

	private void Start()
	{
#if UNITY_EDITOR
		NullChecks();
        m_startingRotation = gameObject.transform.rotation;
#endif
	}
    private void Update()
    {
        CheckRotation();

    }

    public void CheckRotation()
    {
        if (!m_hasCollided) return;

        m_timer += Time.deltaTime;
        if (m_timer > m_checkRotationTimer)
        {
            if (gameObject.transform.rotation != m_startingRotation)
            {
                gameObject.transform.rotation = m_startingRotation;
                TouchManager.Instance.ResetBlock(this);
                m_timer = 0;
                m_hasCollided = false;
            }
            else
            {
                m_rigidbody.isKinematic = true;
                m_timer = 0;
                m_isInTheRightPosition = true;
                m_hasCollided = false;

            }
        }
    }

    public void SetBoolRightPosition(bool newValue)
    {
        m_isInTheRightPosition = newValue;
    }

    public void SetIsDropped(bool newValue)
    {
        m_isDropped = newValue;
    }

    private void OnDisable()
	{
		m_touchManager.RemoveBlock(this);
		m_touchManager.OnGrab -= SetPhysicsInactive;
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

	public void SetPhysicsInactive(bool bInactive)
	{
		m_rigidbody.useGravity = !bInactive;
		m_rigidbody.constraints = !bInactive ? m_dropConstraints : m_moveConstraints;
	}

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        if (!m_isDropped) return;

        m_hasCollided = true;
        m_isDropped = false;
    }
}
