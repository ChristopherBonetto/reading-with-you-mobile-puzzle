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

    [SerializeField] private ParticleSystem m_walkParticle = null;
    [SerializeField] private ParticleSystem m_slideParticle = null;

    [SerializeField] private float m_timeToResetChangeLevel = 1f;

	private float m_endLevelTime;

	private Animator m_playerAnimator;

    #endregion


    private void Awake()
    {
        m_playerAnimator = GetComponentInChildren<Animator>();
    }
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
        Debug.Log(m_currentPlayerState);
        if(m_currentPlayerState == PlayerState.Win)
        {
            if (EndTimer(m_timeToResetChangeLevel))
            {
                GameManager.Instance.EndLevel(true);
            }
        }
        else if(m_currentPlayerState == PlayerState.Lose)
        {
            if (EndTimer(m_timeToResetChangeLevel))
            {

                GameManager.Instance.EndLevel(false);
            }
        }
	}

	private void PlayerMovement()
	{
		m_direction = Vector3.right;
		m_movement = m_effectivePlayerSpeed * m_direction * Time.deltaTime;

		Vector3 nextPointPosition;

		// If no collider in front
		if (!Physics.Raycast(transform.position + Vector3.right * m_raycastFrontDistance, m_movement.normalized, out m_frontRaycastHit, m_movement.magnitude))
		{
			// If on a floor || 0.15f behind a floor
			if (Physics.Raycast(gameObject.transform.position + m_movement, Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance) || Physics.Raycast(gameObject.transform.position + m_movement + new Vector3(0.15f, 0, 0), Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance))
			{
				// Move 0.5f above the found floor
				nextPointPosition = m_nextFrameCollisionPoint.point + Vector3.up * 0.5f;


                if (m_nextFrameCollisionPoint.transform.gameObject.layer == LayerMask.NameToLayer("Ramp"))
                {
                    if (nextPointPosition.y > transform.position.y)
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
                else if(m_nextFrameCollisionPoint.transform.gameObject.layer == LayerMask.NameToLayer("Trapezoid"))
                {
                    float dot = Vector3.Dot(Vector3.down, m_nextFrameCollisionPoint.normal);
                    
                    if(dot == -1)
                    {
                        SetPlayerState(PlayerState.Walk);
                        transform.position = gameObject.transform.position + m_movement;
                    }
                    else
                    {
                        if (nextPointPosition.y > transform.position.y)
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

                }
                else
                {
                    SetPlayerState(PlayerState.Walk);
                    transform.position = gameObject.transform.position + m_movement;
                }
            }
			// No floor
			else
			{
				SetPlayerState(PlayerState.Lose);
                m_playerAnimator.SetBool("noGround", true);
                m_canMove = false;
			}
		}
		// Wall or objective
		else
		{
			if (m_frontRaycastHit.transform.GetComponent<FinalObjectActions>())
			{
				SetPlayerState(PlayerState.Win);
                m_frontRaycastHit.transform.GetComponent<FinalObjectActions>().Collected();
                
            }
            else
			{
                m_playerAnimator.SetBool("blockInFront", true);
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

            switch (inNewState)
            {
                case PlayerState.Idle:
                    m_playerAnimator.SetBool("blockInFront", false);
                    m_playerAnimator.SetBool("noGround", false);
                    m_playerAnimator.SetBool("hasWon", false);
                    m_playerAnimator.SetBool("isInSlide", false);
                    m_playerAnimator.SetBool("isInClimb", false);
                    m_playerAnimator.SetBool("isInWalk", false);
                    m_playerAnimator.SetBool("isInIdle", true);
                    StopParticles();
                    break;

                case PlayerState.Walk:
                    m_playerAnimator.SetBool("blockInFront", false);
                    m_playerAnimator.SetBool("noGround", false);
                    m_playerAnimator.SetBool("hasWon", false);
                    m_playerAnimator.SetBool("isInIdle", false);
                    m_playerAnimator.SetBool("isInSlide", false);
                    m_playerAnimator.SetBool("isInClimb", false);
                    m_playerAnimator.SetBool("isInWalk", true);

                    m_effectivePlayerSpeed = m_playerSpeed;
                    PlayWalkParticle();
                    break;

                case PlayerState.Climb:
                    m_playerAnimator.SetBool("blockInFront", false);
                    m_playerAnimator.SetBool("noGround", false);
                    m_playerAnimator.SetBool("hasWon", false);
                    m_playerAnimator.SetBool("isInIdle", false);
                    m_playerAnimator.SetBool("isInWalk", false);
                    m_playerAnimator.SetBool("isInSlide", false);
                    m_playerAnimator.SetBool("isInClimb", true);
                    StopParticles();
                    m_effectivePlayerSpeed = m_playerSpeed / 2;
                    break;

                case PlayerState.Slide:
                    m_playerAnimator.SetBool("blockInFront", false);
                    m_playerAnimator.SetBool("noGround", false);
                    m_playerAnimator.SetBool("hasWon", false);
                    m_playerAnimator.SetBool("isInIdle", false);
                    m_playerAnimator.SetBool("isInWalk", false);
                    m_playerAnimator.SetBool("isInClimb", false);
                    m_playerAnimator.SetBool("isInSlide", true);
                    m_effectivePlayerSpeed = m_playerSpeed / 2;
                    PlaySlideParticle();
                    break;

                case PlayerState.Win:
                    m_playerAnimator.SetBool("blockInFront", false);
                    m_playerAnimator.SetBool("noGround", false);
                    m_playerAnimator.SetBool("isInSlide", false);
                    m_playerAnimator.SetBool("isInClimb", false);
                    m_playerAnimator.SetBool("isInWalk", false);
                    m_playerAnimator.SetBool("isInIdle", false);
                    m_playerAnimator.SetBool("hasWon", true);
                    StopParticles();
					m_endLevelTime = Time.time;
                    break;

                case PlayerState.Lose:
                    m_playerAnimator.SetBool("hasWon", false);
                    m_playerAnimator.SetBool("isInSlide", false);
                    m_playerAnimator.SetBool("isInClimb", false);
                    m_playerAnimator.SetBool("isInWalk", false);
                    m_playerAnimator.SetBool("isInIdle", false);
                    StopParticles();
					m_endLevelTime = Time.time;
                    break;
            }

			m_currentPlayerState = inNewState;
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
        m_walkParticle.Stop(true);
        m_slideParticle.Play(true);
    }

    public void PlayWalkParticle()
    {
        m_slideParticle.Stop(true);
        m_walkParticle.Play(true);
    }

    private bool EndTimer(float destinationTime)
    {
		return (Time.time >= m_endLevelTime + destinationTime);
    }
}
