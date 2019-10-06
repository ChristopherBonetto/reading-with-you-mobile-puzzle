using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private LayerMask m_obstacleLayer;
    [SerializeField] private float m_raycastFrontDistance;
    [SerializeField] private float m_raycastDownDistance;
    private Vector3 m_playerFeet;

    [SerializeField] private float m_playerSpeed;
    private Vector3 m_movement;
    private Vector3 direction;

    private bool m_isOnTheBack = false;
    private bool m_isClimbing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if (CheckFrontPlayer() && CheckGroundPLayer())
        {
            MovePlayer();
        }
        Debug.Log("is climbing :" + m_isClimbing);
        Debug.Log("is on the back :" + m_isClimbing);
    }
    
    public void MovePlayer()
    {
        if (!m_isOnTheBack)
        {
            direction = transform.right;
        }
        else
        {
            direction = -transform.up;
        }
        m_movement = m_playerSpeed * direction * Time.deltaTime;
        transform.position = gameObject.transform.position + m_movement;
    }

    public bool CheckFrontPlayer()
    {
        RaycastHit hit;
        Vector3 blockNormal;
        m_playerFeet = transform.position + new Vector3(0f, -0.8f, 0f);
                
        if (!Physics.Raycast(m_playerFeet, Vector3.right, out hit, m_raycastFrontDistance, m_obstacleLayer))
        {            
            return true;
        }
        else
        {
            float dot = Vector3.Dot(Vector3.right, hit.normal);
            blockNormal = hit.normal;

            if (dot > -1 && dot < 0)
            {
                m_isClimbing = true;
                transform.rotation = Quaternion.FromToRotation(Vector3.down, -blockNormal);
                return true;
            }
        }
        return false;
    }

    public bool CheckGroundPLayer()
    {
        RaycastHit hit;
        Vector3 blockNormal;

        if (Physics.Raycast(gameObject.transform.position, -transform.up, out hit, m_raycastDownDistance, m_obstacleLayer))
        {
            return true;
        }
        else
        {
            if (Physics.Raycast(gameObject.transform.position, -transform.up, out hit, m_raycastDownDistance + 0.5f, m_obstacleLayer))
            {
                blockNormal = hit.normal;
                float dot = Vector3.Dot(Vector3.down, hit.normal);
                
                
                if (m_isClimbing && dot == -1)
                {
                    transform.position = new Vector3(hit.point.x - 0.5f, hit.point.y + 1f, hit.point.z);
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, blockNormal);
                    m_isClimbing = false;
                    return true;
                }
                else if(!m_isClimbing && dot > -1 && dot < 0)
                {
                    m_isClimbing = true;
                    m_isOnTheBack = true;
                    transform.position = new Vector3(hit.point.x - 0.1f, hit.point.y + 1f, hit.point.z);
                    transform.right = hit.normal;
                    return true;
                }
            }
        }
        
        return false;

    }


    private void OnDrawGizmos()
    {
        Debug.DrawRay(m_playerFeet, Vector3.right * m_raycastFrontDistance, Color.red);

        Debug.DrawRay(gameObject.transform.position, -transform.up * m_raycastDownDistance, Color.red);
    }
}
