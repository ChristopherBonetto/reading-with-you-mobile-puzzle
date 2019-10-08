using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private float m_raycastFrontDistance;
    [SerializeField] private float m_raycastDownDistance;
    private Vector3 m_playerFeet;

    [SerializeField] private float m_playerSpeed;
    private Vector3 m_movement;
    private Vector3 direction;

    private bool m_isOnTheBack = false;
    private bool m_isClimbing = false;

    private bool m_CanMove = false;

    private float m_xPlayerPosition;


    private void Awake()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        m_xPlayerPosition = gameObject.transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_CanMove)
        {
            MovementManagement();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            SetCanMove(true);
        }
        
    }
    
    public void SetCanMove(bool newValue)
    {
        m_CanMove = newValue;
    }

    public void MovementManagement()
    {
        if (CheckNextCell())
        {
            if (!m_isOnTheBack)
            {
                if ((CheckFrontPlayer() && CheckGroundPLayer()))
                {
                    MovementForwards();
                }
                else
                {
                    Debug.Log("lose");
                }
            }
        }
        else
        {
            if (CheckWallWhenSlip() && CheckGroundWhenSlips())
            {
                Slips();
            }
        }
    }

    public void EnableCollider(bool bEnable)
    {
        gameObject.GetComponent<Collider>().enabled = bEnable;
    }    
    

    private bool CheckNextCell()
    {
        if (gameObject.transform.position.x <= m_xPlayerPosition + 0.75)
        {
            return true;
        }
        m_xPlayerPosition = gameObject.transform.position.x;
        return false;
    }

    public void MovementForwards()
    {
        direction = transform.right;        
        m_movement = m_playerSpeed * direction * Time.deltaTime;
        transform.position = gameObject.transform.position + m_movement;
    }

    public void Slips()
    {
        direction = -transform.up;
        m_movement = m_playerSpeed * direction * Time.deltaTime;
        transform.position = gameObject.transform.position + m_movement;
    }

    public bool CheckFrontPlayer()
    {
        RaycastHit hit;
        Vector3 blockNormal;
        m_playerFeet = transform.position + new Vector3(0f, -0.8f, 0f);
                
        if (!Physics.Raycast(m_playerFeet, Vector3.right, out hit, m_raycastFrontDistance))
        {
            
            return true;
        }
        else
        {
            if(hit.transform.name != "FinalObject")
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
                 
        }
        return false;
    }

    public bool CheckGroundPLayer()
    {
        RaycastHit hit;
        Vector3 blockNormal;
        m_playerFeet = transform.position + new Vector3(0f, -0.8f, 0f);

        
        if (Physics.Raycast(gameObject.transform.position, -transform.up, out hit, m_raycastDownDistance) || Physics.CheckSphere(m_playerFeet, 0.3f))
        {
            return true;
        }
        else
        {
            if (Physics.Raycast(gameObject.transform.position, -transform.up, out hit, m_raycastDownDistance + 0.5f))
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
                    transform.position = new Vector3(hit.point.x, hit.point.y + 0.9f, hit.point.z);
                    transform.right = hit.normal;
                    return true;
                }
                
            }
        }
        
        return false;
    }

    public bool CheckWallWhenSlip()
    {
        RaycastHit hit;
        m_playerFeet = transform.position + new Vector3(0f, -0.8f, 0f);

        if (Physics.Raycast(m_playerFeet, Vector3.left, out hit, m_raycastFrontDistance))
        {
            Debug.Log("wall" + hit.transform.name);
            return true;
        }
        return false;
    }

    public bool CheckGroundWhenSlips()
    {
        RaycastHit hit;
        Vector3 blockNormal;

        if (!Physics.Raycast(gameObject.transform.position, -transform.up, out hit, m_raycastDownDistance))
        {
            return true;
        }
        else
        {
            if (Vector3.Distance(gameObject.transform.position, hit.point) <= 1.3f)
            {
                float dot = Vector3.Dot(Vector3.down, hit.normal);
                blockNormal = hit.normal;

                if (m_isClimbing && dot == -1)
                {
                    transform.position = new Vector3(hit.point.x - 0.5f, hit.point.y + 1f, hit.point.z);
                    transform.rotation = Quaternion.FromToRotation(Vector3.up, blockNormal);
                    m_isOnTheBack = false;
                    m_isClimbing = false;
                    return true;
                }
            }
        }
        return false;
    }

    private void CheckVictory()
    {
        if(Physics.CheckSphere(gameObject.transform.position, 1f, 11))
        {
            GameManager.Instance.AdvanceToNextScene();
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(m_playerFeet, Vector3.right * m_raycastFrontDistance, Color.red);

        Debug.DrawRay(gameObject.transform.position, -transform.up * m_raycastDownDistance, Color.red);

        Gizmos.DrawWireSphere(m_playerFeet, 0.5f);

        Gizmos.DrawWireSphere(gameObject.transform.position, 1f);
    }
}
