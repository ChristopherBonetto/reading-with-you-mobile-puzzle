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

    private float m_startinZRotation;
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
        m_startinZRotation = gameObject.transform.localEulerAngles.z;
#endif
	}
    private void Update()
    {
        
        if(gameObject.transform.localEulerAngles.z > m_startinZRotation + 30 || gameObject.transform.localEulerAngles.z < m_startinZRotation - 30)
        {
            transform.rotation = Quaternion.FromToRotation(Vector3.up, Vector3.zero);
            TouchManager.Instance.ResetBlock(this);
        }
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
}
