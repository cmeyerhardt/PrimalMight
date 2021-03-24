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
}
