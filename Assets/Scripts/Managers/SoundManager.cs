using UnityEngine;

[System.Serializable]
public struct CustomAudioClip
{
    public AudioClip ClipAudio;

    [Range(0, 1)]
    public float Intensity;

    public CustomAudioClip(AudioClip inClipAudio, float inIntensity)
    {
        ClipAudio = inClipAudio;
        Intensity = inIntensity;
    }
}


public class SoundManager : Singleton<SoundManager>
{
    [SerializeField] private AudioSource m_playerAudioSource = null;

    [Space]
    public CustomAudioClip PlayerWalkAudioClip;
    public float DelayWalkSound;
    [Space]

    public CustomAudioClip PlayerSlideAudioClip;
    public CustomAudioClip PlayerClimbAudioClip;
    public CustomAudioClip PlayerFallGaspAudioClip;


    [Space]


    [SerializeField] private AudioSource m_backgroundAudioSource = null;

    public CustomAudioClip[] BackgroundAudioClips;


    [Space]
    

    [SerializeField] private AudioSource m_finalObjectAudioSource = null;

    public CustomAudioClip VictoryAudioClip;


    [Space]


    [SerializeField] private AudioSource m_blockManagerAudioSource = null;

    public CustomAudioClip BlockMissplacementAudioClip;
    public CustomAudioClip PickUpBlockAudioClip;


    [Space]


    [SerializeField] private AudioSource m_UIAudioSource = null;

    public CustomAudioClip TapOneAudioClip;
    public CustomAudioClip TapTwoAudioClip;



    public void PlayerPlaySound(CustomAudioClip inClip)
    {
        m_playerAudioSource.PlayOneShot(inClip.ClipAudio, inClip.Intensity);
    }
        
    public void PlayBackgoundSound(int loopClip)
    {
        if (!m_backgroundAudioSource.loop)
        {
            m_backgroundAudioSource.loop = true;
        }

        AudioClip tempAudio = BackgroundAudioClips[loopClip].ClipAudio;

        if(m_backgroundAudioSource.clip != tempAudio)
        {
            StopBackgroundSound();
            m_backgroundAudioSource.clip = tempAudio;
            m_backgroundAudioSource.volume = BackgroundAudioClips[loopClip].Intensity;
            m_backgroundAudioSource.Play();
        }
    }

    public void StopAllSounds()
    {
        m_backgroundAudioSource.Stop();
        m_finalObjectAudioSource.Stop();
        m_playerAudioSource.Stop();
    }

    public void StopBackgroundSound()
    {
        if (m_backgroundAudioSource.isPlaying)
        {
            m_backgroundAudioSource.Stop();
            m_backgroundAudioSource.clip = null;
        }
    }

    public void FinalObjectPlaySound(CustomAudioClip inClip)
    {
        m_finalObjectAudioSource.PlayOneShot(inClip.ClipAudio,inClip.Intensity);
    }


    public void BlockManagerPlaySound(CustomAudioClip inClip)
    {
        m_blockManagerAudioSource.PlayOneShot(inClip.ClipAudio, inClip.Intensity);
    }
    

    public void UIPlaySound(CustomAudioClip inClip)
    {
        m_UIAudioSource.PlayOneShot(inClip.ClipAudio, inClip.Intensity);
    }
}
