using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActions : MonoBehaviour
{
    [SerializeField] private LayerMask m_obstacleLayer;
    [SerializeField] private float m_raycastDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CheckFrontPlayer();
    }

    public void CheckFrontPlayer()
    {
        RaycastHit hit;
        Vector3 blockNormal;

        if (Physics.Raycast(gameObject.transform.position, Vector3.right, out hit, m_raycastDistance, m_obstacleLayer))
        {
            blockNormal = hit.normal;
            Debug.DrawRay(hit.point, blockNormal, Color.blue, 100f);
        }
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(gameObject.transform.position, Vector3.right * m_raycastDistance, Color.red);
    }
}
