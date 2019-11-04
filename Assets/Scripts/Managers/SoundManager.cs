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
    #region Player Sounds
    [Header("Player Sound Options"), Space]
    
    public CustomAudioClip PlayerWalkAudioClip;
    public float DelayWalkSound;

    [Space,Space]

    public CustomAudioClip PlayerClimbAudioClip;
    public float DelayClimbSound;

    [Space, Space]

    public CustomAudioClip PlayerSlideAudioClip;
    public CustomAudioClip PlayerFallGaspAudioClip;
    public CustomAudioClip PlayerHitObstacle;

    [Space, Space, Space]

    [SerializeField] private AudioSource m_playerAudioSource = null;
    #endregion

    

    #region Background Sounds
    [Header("Backgound Sound Options"), Space, Space, Space, Space, Space]
        
    public CustomAudioClip StartSceneAudioClip;

    [Space, Space]

    public CustomAudioClip[] BackgroundAudioClips;

    [Space, Space,Space]

    [SerializeField] private AudioSource m_backgroundAudioSource = null;
    #endregion



    #region Other Sounds (End level, block placement, UI taps)

    [Header("Victory Sound Options"), Space, Space, Space, Space, Space]

    public CustomAudioClip VictoryAudioClip;

    [Space, Space]

    [SerializeField] private AudioSource m_finalObjectAudioSource = null;



    [Header("Blocks Sound Options"), Space, Space, Space, Space, Space]

    public CustomAudioClip BlockMissplacementAudioClip;
    public CustomAudioClip PickUpBlockAudioClip;

    [Space, Space]

    [SerializeField] private AudioSource m_blockManagerAudioSource = null;

    

    [Header("UI Sound Options"), Space, Space, Space, Space, Space]

    public CustomAudioClip TapOneAudioClip;
    public CustomAudioClip TapTwoAudioClip;

    [Space, Space]

    [SerializeField] private AudioSource m_UIAudioSource = null;
    #endregion


    /// <summary>
    /// This manages all the player sounds. Check if the player is playing a clip (if the bool returns true, stop that), after that play another clip.
    /// </summary>
    /// <param name="inClip"></param> the input's clip to reproduce.
    public void PlayerPlaySound(CustomAudioClip inClip)
    {
        if (m_playerAudioSource.isPlaying)
        {
            m_playerAudioSource.Stop();
        }
        m_playerAudioSource.PlayOneShot(inClip.ClipAudio, inClip.Intensity);
    }

    /// <summary>
    /// This manages all the start menu sounds.
    /// </summary>
    public void PlayStartMenuAudio()
    {
        if (m_backgroundAudioSource.clip != StartSceneAudioClip.ClipAudio)
        {
            m_backgroundAudioSource.Stop();

            if (!m_backgroundAudioSource.loop)
            {
                m_backgroundAudioSource.loop = true;
            }

            m_backgroundAudioSource.clip = StartSceneAudioClip.ClipAudio;
            m_backgroundAudioSource.volume = StartSceneAudioClip.Intensity;
            m_backgroundAudioSource.Play();
        }  
    }

    /// <summary>
    /// This manages all the backgound sounds. Check if the audio source have the loop's bool setted true, after this it reproduces the input's clip.
    /// </summary>
    public void PlayBackgroundSound(int loopClip)
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

    /// <summary>
    /// This manages all the final object sounds.
    /// </summary>
    /// <param name="inClip"></param> the input's clip to reproduce.
    public void FinalObjectPlaySound(CustomAudioClip inClip)
    {
        m_finalObjectAudioSource.PlayOneShot(inClip.ClipAudio,inClip.Intensity);
    }

    /// <summary>
    /// This manages all the block manager sounds.
    /// </summary>
    /// <param name="inClip"></param> the input's clip to reproduce.
    public void BlockManagerPlaySound(CustomAudioClip inClip)
    {
        m_blockManagerAudioSource.PlayOneShot(inClip.ClipAudio, inClip.Intensity);
    }

    /// <summary>
    /// This manages all the ui sounds.
    /// </summary>
    /// <param name="inClip"></param> the input's clip to reproduce.
    public void UIPlaySound(CustomAudioClip inClip)
    {
        m_UIAudioSource.PlayOneShot(inClip.ClipAudio, inClip.Intensity);
    }

    /// <summary>
    /// This manages all the audio sources.
    /// </summary>
    public void StopAllSounds()
    {
        m_backgroundAudioSource.Stop();
        m_finalObjectAudioSource.Stop();
        m_playerAudioSource.Stop();
    }

    /// <summary>
    /// Stop the current background clip.
    /// </summary>
    public void StopBackgroundSound()
    {
        if (m_backgroundAudioSource.isPlaying)
        {
            m_backgroundAudioSource.Stop();
            m_backgroundAudioSource.clip = null;
        }
    }
}
