using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Lives on gameObject somewhere within player gameObject hierarchy
/// </summary>
public class AudioMix : MonoBehaviour
{
    [SerializeField] public AudioMixerSnapshot main = null;
    [SerializeField] public AudioMixerSnapshot movement = null;
    [SerializeField] public AudioMixerSnapshot combat = null;
    [SerializeField] public AudioMixerSnapshot paused = null;
    public AudioMixerSnapshot last = null;
    public AudioMixer _Mixer = null;


    [SerializeField] public AudioMixerGroup master = null;
    [SerializeField] public AudioMixerGroup music = null;
    [SerializeField] public AudioMixerGroup sfx = null;
    [SerializeField] public AudioMixerGroup uiSFX = null;
    [SerializeField] public AudioMixerGroup voice = null;




    public void Start()
    {
        _Mixer.updateMode = AudioMixerUpdateMode.UnscaledTime;

        Transition(false, false);
    }

    public void TransitionPaused()
    {
        TransitionToSnapshot(paused, 1f);
    }

    public void TransitionBack()
    {
        
        Debug.Log("Transition back to: " + last.name);
        TransitionToSnapshot(last, 1f);
    }

    public void Transition(bool isMoving, bool isCombat)
    {
        if (SceneLoader.paused)
        {
            last = paused;
            TransitionToSnapshot(paused, 1f);
        }
        else if (isCombat)
        {
            last = combat;
            TransitionToSnapshot(combat, 1f);
        }
        else if(isMoving)
        {
            last = movement;
            TransitionToSnapshot(movement, 1.5f);
        }
        else 
        {
            last = main;
            TransitionToSnapshot(main, 5f);
        }
    }

    public void TransitionToSnapshot(AudioMixerSnapshot snapshot, float time = 3f)
    {
        if (snapshot != null)
        {
            snapshot.TransitionTo(time);
        }
    }


    //Volume sliders

    public void SetMasterVolume(float value)
    {
        _Mixer.SetFloat("masterVol", value);
    }

    public void SetMusicVolume(float value)
    {
        _Mixer.SetFloat("musicVol", value);
    }

    public void SetSFXVolume(float value)
    {
        _Mixer.SetFloat("sfxVol", value);
    }

    public void SetUIVolume(float value)
    {
        _Mixer.SetFloat("uiVol", value);
    }

    public void SetVoiceVolume(float value)
    {
        _Mixer.SetFloat("voiceVol", value);
    }
}
