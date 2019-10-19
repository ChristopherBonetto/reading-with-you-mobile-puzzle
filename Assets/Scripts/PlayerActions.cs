using UnityEngine;

public enum PlayerState
{
	Idle,
	Walk,
	Climb,
	Slide,
	Lose,
	Win
}

public class PlayerActions : MonoBehaviour
{

	#region Variables
	[SerializeField] private float m_playerSpeed = 2f;
	private float m_effectivePlayerSpeed;

	[SerializeField] private float m_raycastFrontDistance = 0.25f;
	[SerializeField] private float m_raycastDownDistance = 0.7f;

	private Vector3 m_movement;
	private Vector3 m_direction;
	private float m_xPlayerPosition;

	private RaycastHit m_nextFrameCollisionPoint;
	private RaycastHit m_frontRaycastHit;

	private bool m_canMove;

	public PlayerState m_currentPlayerState { get; private set; }

    [SerializeField] private ParticleSystem m_walkParticle;
    [SerializeField] private ParticleSystem m_slideParticle;

	#endregion

	private void Start()
	{
        SetPlayerState(PlayerState.Idle);
		m_effectivePlayerSpeed = m_playerSpeed;
	}

	void Update()
	{
        
		if (m_canMove)
		{
			PlayerMovement();
		}
	}

	private void PlayerMovement()
	{
		m_direction = Vector3.right;
		m_movement = m_effectivePlayerSpeed * m_direction * Time.deltaTime;

		Vector3 nextPointPosition;

		if (!Physics.Raycast(transform.position + Vector3.right * m_raycastFrontDistance, m_movement.normalized, out m_frontRaycastHit, m_movement.magnitude))
		{
			if (Physics.Raycast(gameObject.transform.position + m_movement, Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance) || Physics.Raycast(gameObject.transform.position + m_movement + new Vector3(0.15f, 0, 0), Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance))
			{
				nextPointPosition = m_nextFrameCollisionPoint.point + Vector3.up * 0.5f;

				if (m_nextFrameCollisionPoint.transform.gameObject.layer != LayerMask.NameToLayer("Ramp"))
				{
					SetPlayerState(PlayerState.Walk);
					transform.position = gameObject.transform.position + m_movement;
				}
				else if (nextPointPosition.y > transform.position.y)
				{
					SetPlayerState(PlayerState.Climb);
					transform.position = nextPointPosition;
				}
				else if (nextPointPosition.y < transform.position.y)
				{
					SetPlayerState(PlayerState.Slide);
					transform.position = nextPointPosition;
				}
			}
			else
			{
				GameManager.Instance.EndLevel(false);
				SetPlayerState(PlayerState.Lose);
				m_canMove = false;
			}
		}
		else
		{
			if (m_frontRaycastHit.transform.GetComponent<FinalObjectActions>())
			{
				GameManager.Instance.EndLevel(true);
				SetPlayerState(PlayerState.Win);
			}
			else
			{
				GameManager.Instance.EndLevel(false);
				SetPlayerState(PlayerState.Lose);
			}
			m_canMove = false;
		}
	}

	public void ResetLevel(Vector3 startPosition)
	{
        StopParticles();
		EnableMovement(false);
		transform.position = startPosition;
        SetPlayerState(PlayerState.Idle);
        
	}

	private void SetPlayerState(PlayerState inNewState)
	{
		if (inNewState != m_currentPlayerState)
		{
			m_currentPlayerState = inNewState;

			if (inNewState == PlayerState.Climb)
			{
                StopParticles();
				m_effectivePlayerSpeed = m_playerSpeed / 2;
			}
            else if (inNewState == PlayerState.Slide)
            {
                m_effectivePlayerSpeed = m_playerSpeed / 2;
                PlaySlideParticle();
            }
            else if (inNewState == PlayerState.Walk)
            {
                m_effectivePlayerSpeed = m_playerSpeed;
                PlayWalkParticle();
            }
            else if (inNewState == PlayerState.Idle)
			{
                StopParticles();
			}
		}
	}

	public void EnableMovement(bool bEnable)
	{
		gameObject.GetComponent<Collider>().enabled = !bEnable;
		m_canMove = bEnable;
	}

    public void StopParticles()
    {
        m_slideParticle.Stop(true);
        m_walkParticle.Stop(true);
    }

    public void PlaySlideParticle()
    {
        m_walkParticle.Pause(true);
        m_slideParticle.Play(true);
    }

    public void PlayWalkParticle()
    {
        m_slideParticle.Pause(true);
        m_walkParticle.Play(true);
    }
}
