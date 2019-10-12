using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    [SerializeField] private float m_playerSpeed;
    private float m_effectivePlayerSpeed;

    [SerializeField] private float m_raycastFrontDistance;
    [SerializeField] private float m_raycastDownDistance;

    private Vector3 m_movement;
    private Vector3 m_direction;
    private float m_xPlayerPosition;

    private RaycastHit m_nextFrameCollisionPoint;

    private bool m_canMove = false;
    public bool canMove
    {
        get
        {
            return m_canMove;
        }
        set
        {
            m_canMove = value;
        }
    }

    public PlayerState m_currentPlayerState { get; private set; }

    #endregion

    private void Start()
    {
        m_effectivePlayerSpeed = m_playerSpeed;
        Debug.Log(GameManager.Instance.CurrentState);
    }

    void Update()
    {
        
        if (canMove)
        {
            PlayerMovement();
        }
    }

    
    
    private void PlayerMovement()
    {
        m_direction = Vector3.right;
        m_movement = m_effectivePlayerSpeed * m_direction * Time.deltaTime;

        Vector3 nextPointPosition;

        if (!Physics.Raycast(transform.position + Vector3.right * m_raycastFrontDistance, m_movement.normalized, m_movement.magnitude))
        {
            if (Physics.Raycast(gameObject.transform.position + m_movement, Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance) || Physics.Raycast(gameObject.transform.position + m_movement + new Vector3(0.15f,0,0), Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance))
            {
                nextPointPosition = m_nextFrameCollisionPoint.point + Vector3.up * 0.5f;
                
                if(m_nextFrameCollisionPoint.transform.gameObject.layer != LayerMask.NameToLayer("Ramp"))
                {
                    SetPlayerState(PlayerState.Walk);
                    transform.position = gameObject.transform.position + m_movement;
                }
                else if(nextPointPosition.y > transform.position.y)
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
                SetPlayerState(PlayerState.Lose);
            }
        }
        else
        {
            SetPlayerState(PlayerState.Lose);
        }
    }
    
    public void SetPlayerState(PlayerState inNewState)
    {
        if(inNewState != m_currentPlayerState)
        {
            m_currentPlayerState = inNewState;
            if(inNewState == PlayerState.Climb || inNewState == PlayerState.Slide)
            {
                m_effectivePlayerSpeed = m_playerSpeed / 2;
            }
            else
            {
                m_effectivePlayerSpeed = m_playerSpeed;
            }
        }
    }

    public void EnableCollider(bool bEnable)
    {
        gameObject.GetComponent<Collider>().enabled = bEnable;
    }
    
}
