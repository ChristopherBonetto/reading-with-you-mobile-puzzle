using UnityEngine;

public class FinalObjectActions : MonoBehaviour
{
    [SerializeField] private ParticleSystem m_particleEndLevel = null;
    [SerializeField] private Transform m_objectIcon = null;
    private Vector3 m_startingIconPosition;

    [SerializeField] private Collider m_finalObjectCollider = null;

    [SerializeField] private SpriteRenderer m_finalObjectSprite = null;

    private void Start()
    {
        m_startingIconPosition = m_objectIcon.transform.localPosition;
        DisableEndLevelParticle();
    }

    public void ResetLevel(Vector3 startPosition, Sprite newFinalObjectSprite)
	{
        EnableDisableCollider(true);
        m_finalObjectSprite.sprite = newFinalObjectSprite;
        transform.parent.transform.position = startPosition;
        m_objectIcon.transform.localPosition = m_startingIconPosition;
        DisableEndLevelParticle();
	}


    public void Collected()
    {
        EnableEndLevelParticle();
        m_objectIcon.transform.position = new Vector3(m_objectIcon.transform.position.x, m_objectIcon.transform.position.y + 1.5f, m_objectIcon.transform.position.z);
    }


    private void DisableEndLevelParticle()
    {
        m_particleEndLevel.Stop(true);        
    }

    private void EnableEndLevelParticle()
    {        
        m_particleEndLevel.Play(true);
    }

    public void EnableDisableCollider(bool newValue)
    {
        m_finalObjectCollider.enabled = newValue;
    }

}
