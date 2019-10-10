using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerActions : MonoBehaviour
{

#region Variables
    [SerializeField] private float m_playerSpeed;
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
#endregion


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
        m_movement = m_playerSpeed * m_direction * Time.deltaTime;

        Vector3 nextPointPosition;

        if (!Physics.Raycast(transform.position + Vector3.right * m_raycastFrontDistance, m_movement.normalized, m_movement.magnitude))
        {
            if (Physics.Raycast(gameObject.transform.position + m_movement, Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance) || Physics.Raycast(gameObject.transform.position + m_movement + new Vector3(0.15f,0,0), Vector3.down, out m_nextFrameCollisionPoint, m_raycastDownDistance))
            {
                nextPointPosition = m_nextFrameCollisionPoint.point + Vector3.up * 0.5f;
                
                if(nextPointPosition.y == transform.position.y)
                {
                    transform.position = gameObject.transform.position + m_movement;
                }
                else
                {
                    transform.position = nextPointPosition;
                }
                
            }
            else
            {
                Debug.Log("no one block");
            }
        }
        else
        {
            Debug.Log("block in front of the player");
        }
    }
    
    public void EnableCollider(bool bEnable)
    {
        gameObject.GetComponent<Collider>().enabled = bEnable;
    }
    
}
