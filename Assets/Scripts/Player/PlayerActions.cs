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

    private float m_lastCheckedDirectionTime;
    public float m_delayToCheckDirection;

    [SerializeField] private float m_raycastFrontDistance = 0.25f;
	[SerializeField] private float m_raycastDownDistance = 0.7f;

	private Vector3 m_movement;
	private Vector3 m_direction;

	private RaycastHit m_nextFrameCollisionPoint;
	private RaycastHit m_frontRaycastHit;

	private bool m_canMove;

	private bool m_endLevel;

	public PlayerState m_currentPlayerState { get; private set; }

    [SerializeField] private ParticleSystem m_walkParticle = null;
    [SerializeField] private ParticleSystem m_slideParticle = null;

    [SerializeField] private float m_timeToResetChangeLevel = 1f;

	private float m_endLevelTime;

    private float m_lastSoundShotted;

    private Animator m_playerAnimator;

	private string[] m_animatorLayers = { "Base", "W1", "W2", "W3", "W4", "W5" };

    #endregion
    
    

    private void Awake()
    {
        m_playerAnimator = GetComponentInChildren<Animator>();
    }

    private void Start()
	{
        ChangeDirection(Vector3.right);

        SetPlayerState(PlayerState.Idle);
		m_effectivePlayerSpeed = m_playerSpeed;

        
	}

	void Update()
	{
		if (m_canMove)
		{
            PlayerMovement();
            PlayerAudioDelaySystem();
            
        }
        else if (m_endLevel)
        {
            CheckAndSetWinOrLose();
        }
	}
    

    #region Character Movement

    // <summary>
    // Using raycast checks obstacles in front of the player. If the raycast find an obstacle check if that is the final object.
    // </summary>
    public bool CheckFront()
    {
        if (!Physics.Raycast(transform.position + Vector3.right * m_raycastFrontDistance, m_movement.normalized, out m_frontRaycastHit, m_movement.magnitude))
        {
            return true;
        }
        else
        {
            if (m_frontRaycastHit.transform.GetComponent<FinalObjectActions>())
            {
                SetPlayerState(PlayerState.Win);
                m_frontRaycastHit.transform.GetComponent<FinalObjectActions>().Collected();
            }
            else
            {
                SetPlayerState(PlayerState.Lose);

                m_playerAnimator.SetBool("blockInFront", true);

                SoundManager.Instance.PlayerPlaySound(SoundManager.Instance.PlayerHitObstacle);
            }
            
            m_canMove = false;
            return false;
        }
    }

    // <summary>
    // Using raycast the character checks if there is any platform under him. If there is something below, he does another check to understand which type of terrain/block he must go across, this determine
    // if him must advance straight or obliquely.
    // </summary>
    public Vector3 CheckDown()
    {
        Vector3 nextPointPosition;

        if (Physics.Raycast(gameObject.transform.position + m_movement, Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance) || Physics.Raycast(gameObject.transform.position + m_movement + new Vector3(0.15f, 0, 0), Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance))
        {

            nextPointPosition = m_nextFrameCollisionPoint.point + Vector3.up * 0.5f;


            if (m_nextFrameCollisionPoint.transform.gameObject.layer == LayerMask.NameToLayer("Ramp"))
            {
                if (nextPointPosition.y > transform.position.y)
                {
                    SetPlayerState(PlayerState.Climb);
                    return Vector3.right + Vector3.up;
                }
                else if (nextPointPosition.y < transform.position.y)
                {
                    SetPlayerState(PlayerState.Slide);
                    return Vector3.right - Vector3.up;
                }
            }
            else if (m_nextFrameCollisionPoint.transform.gameObject.layer == LayerMask.NameToLayer("Trapezoid"))
            {
                float dot = Vector3.Dot(Vector3.down, m_nextFrameCollisionPoint.normal);

                if (dot == -1)
                {
                    SetPlayerState(PlayerState.Walk);
                    return Vector3.right;
                }
                else
                {
                    if (nextPointPosition.y > transform.position.y)
                    {
                        SetPlayerState(PlayerState.Climb);
                        return Vector3.right + Vector3.up;
                    }
                    else if (nextPointPosition.y < transform.position.y)
                    {
                        SetPlayerState(PlayerState.Slide);
                        return Vector3.right - Vector3.up;
                    }
                }

            }
            else
            {
                SetPlayerState(PlayerState.Walk);
                return Vector3.right;
            }
        }
        // No floor
        else
        {
            SetPlayerState(PlayerState.Lose);
                        
            m_playerAnimator.SetBool("noGround", true);

            SoundManager.Instance.PlayerPlaySound(SoundManager.Instance.PlayerFallGaspAudioClip);

            m_canMove = false;
        }

        return transform.position;
    }

    // <summary>
    // Set the movement's player taking the parameters to the other two methods.
    // </summary>
    private void PlayerMovement()
	{
		m_movement = m_effectivePlayerSpeed * m_direction * Time.deltaTime;

        if (TimerToCheckDirection(m_delayToCheckDirection))
        {
            if (CheckFront())
            {
                ChangeDirection(CheckDown());
            }
            m_lastCheckedDirectionTime = Time.time;
        }
        
        transform.position += m_movement;


        #region old movementMethod

        //if (!Physics.Raycast(transform.position + Vector3.right * m_raycastFrontDistance, m_movement.normalized, out m_frontRaycastHit, m_movement.magnitude))
        //{

        //    if (Physics.Raycast(gameObject.transform.position + m_movement, Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance) || Physics.Raycast(gameObject.transform.position + m_movement + new Vector3(0.15f, 0, 0), Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance))
        //    {

        //        nextPointPosition = m_nextFrameCollisionPoint.point + Vector3.up * 0.5f;


        //        if (m_nextFrameCollisionPoint.transform.gameObject.layer == LayerMask.NameToLayer("Ramp"))
        //        {
        //            if (nextPointPosition.y > transform.position.y)
        //            {
        //                SetPlayerState(PlayerState.Climb);
        //                transform.position = nextPointPosition;
        //            }
        //            else if (nextPointPosition.y < transform.position.y)
        //            {
        //                SetPlayerState(PlayerState.Slide);
        //                transform.position = nextPointPosition;
        //            }
        //        }
        //        else if (m_nextFrameCollisionPoint.transform.gameObject.layer == LayerMask.NameToLayer("Trapezoid"))
        //        {
        //            float dot = Vector3.Dot(Vector3.down, m_nextFrameCollisionPoint.normal);

        //            if (dot == -1)
        //            {
        //                SetPlayerState(PlayerState.Walk);
        //                transform.position = gameObject.transform.position + m_movement;
        //            }
        //            else
        //            {
        //                if (nextPointPosition.y > transform.position.y)
        //                {
        //                    SetPlayerState(PlayerState.Climb);
        //                    transform.position = nextPointPosition;
        //                }
        //                else if (nextPointPosition.y < transform.position.y)
        //                {
        //                    SetPlayerState(PlayerState.Slide);
        //                    transform.position = nextPointPosition;
        //                }
        //            }

        //        }
        //        else
        //        {
        //            SetPlayerState(PlayerState.Walk);
        //            transform.position = gameObject.transform.position + m_movement;
        //        }
        //    }
        //    // No floor
        //    else
        //    {
        //        SetPlayerState(PlayerState.Lose);

        //        m_playerAnimator.SetBool("noGround", true);

        //        SoundManager.Instance.PlayerPlaySound(SoundManager.Instance.PlayerFallGaspAudioClip);

        //        m_canMove = false;
        //    }
        //}
        //// Wall or objective
        //else
        //{
        //    if (m_frontRaycastHit.transform.GetComponent<FinalObjectActions>())
        //    {
        //        SetPlayerState(PlayerState.Win);
        //        m_frontRaycastHit.transform.GetComponent<FinalObjectActions>().Collected();

        //    }
        //    else
        //    {
        //        SetPlayerState(PlayerState.Lose);

        //        m_playerAnimator.SetBool("blockInFront", true);

        //        SoundManager.Instance.PlayerPlaySound(SoundManager.Instance.PlayerHitObstacle);
        //    }
        //    m_canMove = false;
        //}
        #endregion
    }

    public void ChangeDirection(Vector3 newDirection)
    {
        m_direction = newDirection;
        
    }
    #endregion


    #region Player States System
    // </summary>
    /// <param name="inNewState"></param> Used to set the next PlayerState after checking if the player has currently another state. 
    private void SetPlayerState(PlayerState inNewState)
    {
        if (inNewState != m_currentPlayerState)
        {
            switch (inNewState)
            {
                case PlayerState.Idle:

                    ResetAllAnimation();
                    PlayAnimation("isInIdle");

                    StopParticles();
                    break;

                case PlayerState.Walk:

                    m_effectivePlayerSpeed = m_playerSpeed;

                    ResetAllAnimation();
                    PlayAnimation("isInWalk");

                    PlayWalkParticle();
                    break;

                case PlayerState.Climb:

                    m_effectivePlayerSpeed = m_playerSpeed / 2;

                    ResetAllAnimation();
                    PlayAnimation("isInClimb");

                    SoundManager.Instance.PlayerPlaySound(SoundManager.Instance.PlayerClimbAudioClip);

                    StopParticles();
                    break;

                case PlayerState.Slide:

                    m_effectivePlayerSpeed = m_playerSpeed / 2;

                    ResetAllAnimation();
                    PlayAnimation("isInSlide");

                    SoundManager.Instance.PlayerPlaySound(SoundManager.Instance.PlayerSlideAudioClip);

                    PlaySlideParticle();
                    break;

                case PlayerState.Win:

                    ResetAllAnimation();
                    PlayAnimation("hasWon");

                    m_endLevelTime = Time.time;
					m_endLevel = true;

                    StopParticles();
                    break;

                case PlayerState.Lose:

                    ResetAllAnimation();

                    m_endLevelTime = Time.time;
					m_endLevel = true;

                    StopParticles();
                    break;
            }

            m_currentPlayerState = inNewState;
        }
    }
    #endregion


    #region Update/Reset level Methods

    // <summary>
    // Check the current player's state. If he win or lose recall a method when the timer reach the destination time.
    // </summary>
    private void CheckAndSetWinOrLose()
    {
        bool Winned;

        if (m_currentPlayerState == PlayerState.Win)
        {
            Winned = true;

            if (EndTimer(m_timeToResetChangeLevel))
            {
                GameManager.Instance.EndLevel(Winned);
				m_endLevel = false;
            }
        }
        else if (m_currentPlayerState == PlayerState.Lose)
        {
            Winned = false;

            if (EndTimer(m_timeToResetChangeLevel))
            {
                GameManager.Instance.EndLevel(Winned);
				m_endLevel = false;
            }
        }
        
    }

    // <summary>
    // Used to set canMove's bool and the player's collider. Used to let the character go forwards or to reset him.
    // </summary>
    public void EnableMovement(bool bEnable)
    {
        gameObject.GetComponent<Collider>().enabled = !bEnable;
        m_canMove = bEnable;
    }

    // <summary>
    // Method that it will reset all the player's comporaments.
    // </summary>
    public void ResetLevel(Vector3 startPosition)
    {
        StopParticles();

        EnableMovement(false);
        transform.position = startPosition;
        SetPlayerState(PlayerState.Idle);
        ChangeDirection(Vector3.right);
        
    }
    
    private bool EndTimer(float destinationTime)
    {
        return (Time.time >= m_endLevelTime + destinationTime);
    }
    #endregion


    #region Animations Methods
    // <summary>
    // Methods to set animation's bool or set all bool to false;
    // </summary>

    public void PlayAnimation(string inNewAnimation)
    {
        m_playerAnimator.SetBool(inNewAnimation, true);
    }

    public void ResetAllAnimation()
    {
        m_playerAnimator.SetBool("blockInFront", false);
        m_playerAnimator.SetBool("noGround", false);
        m_playerAnimator.SetBool("isInSlide", false);
        m_playerAnimator.SetBool("isInClimb", false);
        m_playerAnimator.SetBool("isInWalk", false);
        m_playerAnimator.SetBool("isInIdle", false);
        m_playerAnimator.SetBool("hasWon", false);
    }

	public void SetAnimationLayer(int index)
	{
		for (int i = 0; i < m_animatorLayers.Length; i++)
		{
			m_playerAnimator.SetLayerWeight(i, (i == index + 1 ? 1 : 0));
		}
	}
    #endregion

    
    #region Particles Methods
    // <summary>
	// Methods to activate and disable particles
	// </summary>

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
    #endregion
        

    #region Sound Methods
    // <summary>
    // Check the player's state and play audioclip when the timer reach the destination time.
    // </summary>

    public void PlayerAudioDelaySystem()
    {
        if (m_currentPlayerState == PlayerState.Walk)
        {
            PlayAudioClipWithDelay(SoundManager.Instance.PlayerWalkAudioClip, SoundManager.Instance.DelayWalkSound);
        }
        else if (m_currentPlayerState == PlayerState.Climb)
        {
            PlayAudioClipWithDelay(SoundManager.Instance.PlayerClimbAudioClip, SoundManager.Instance.DelayClimbSound);
        }
    }

    private bool TimerToWalkSound(float destinationTime)
    {
        return (Time.time >= m_lastSoundShotted + destinationTime);
    }

    private void PlayAudioClipWithDelay(CustomAudioClip clipToSound, float delay)
    {
        if (TimerToWalkSound(delay))
        {
            SoundManager.Instance.PlayerPlaySound(clipToSound);
            m_lastSoundShotted = Time.time;
        }
    }
    #endregion
    private bool TimerToCheckDirection(float destinationTime)
    {
        return (Time.time >= m_lastCheckedDirectionTime + destinationTime);
    }
}
