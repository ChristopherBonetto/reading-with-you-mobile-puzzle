using UnityEngine;

public class FinalObjectActions : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_particleEndLevel;
    [SerializeField] private Transform m_objectIcon;
    private Vector3 m_startingIconPosition;

    private void Start()
    {
        m_startingIconPosition = m_objectIcon.transform.localPosition;
    }

    public void ResetLevel(Vector3 startPosition)
	{
        transform.parent.transform.position = startPosition;
        m_objectIcon.transform.localPosition = m_startingIconPosition;
        DisableEndLevelParticle();
	}


    public void Collected()
    {
        EnableEndLevelParticle();
        m_objectIcon.transform.position = new Vector3(m_objectIcon.transform.position.x, m_objectIcon.transform.position.y + 1, m_objectIcon.transform.position.z);
    }


    private void DisableEndLevelParticle()
    {
        m_particleEndLevel.Stop(true);        
    }

    private void EnableEndLevelParticle()
    {        
        m_particleEndLevel.Play(true);
    }

}
