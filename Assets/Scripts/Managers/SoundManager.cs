using UnityEngine;

public class SoundManager : Singleton<SoundManager>
{
   
    [SerializeField]
	private AudioSource m_playerAudioSource = null;
    public AudioClip m_playerWalkAudioClip;
    public AudioClip m_playerSlideAudioClip;
    public AudioClip m_playerClimbAudioClip;
    public AudioClip m_playerFallGaspAudioClip;


    [Space]

    [SerializeField]
	private AudioSource m_backgroundAudioSource = null;
    public AudioClip[] m_backgroundAudioClips;

    [Space]
    [SerializeField]
	private AudioSource m_finalObjectAudioSource = null;
    public AudioClip m_victorySound;



       
    public void PlayerPlaySound(AudioClip shootClip)
    {
        m_playerAudioSource.PlayOneShot(shootClip);
        
    }

    public void PlayBackgoundSound(int loopClip)
    {
        if (!m_backgroundAudioSource.loop)
        {
            m_backgroundAudioSource.loop = true;
        }

        AudioClip tempAudio = m_backgroundAudioClips[loopClip];

        if(m_backgroundAudioSource.clip != tempAudio)
        {
            StopBackgroundSound();
            m_backgroundAudioSource.clip = tempAudio;
            m_backgroundAudioSource.Play();
        }
        else
        {
            Debug.Log("ciao");
        }
        

    }

    public void StopBackgroundSound()
    {
        if (m_backgroundAudioSource.isPlaying)
        {
            m_backgroundAudioSource.Stop();
        }
    }

    public void FinalObjectPlaySound(AudioClip shootClip)
    {
        m_finalObjectAudioSource.PlayOneShot(shootClip);
    }

    
}
