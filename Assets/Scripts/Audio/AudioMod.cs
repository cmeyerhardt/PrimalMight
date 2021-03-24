using UnityEngine;
using System.Collections;

public class AudioMod : MonoBehaviour
{
    [SerializeField] AudioSource audioSource = null;

    [Header("Awake Audio")]
    [SerializeField] AudioClipGroup awakeAudio = null;
    [SerializeField] bool playAwakeOnEnable = false;

    [Header("Event Audio")]
    [SerializeField] AudioClipGroup[] audioClipGroups = null;

    [Header("Testing")]
    [SerializeField] int index = 0;
    public bool testClip = false;

    private void Awake()
    {
        if(audioSource != null)
        {
            audioSource.maxDistance = 5f;
        }

        PlayAudioClip(awakeAudio);
    }

    private void Update()
    {
        if (testClip)
        {
            testClip = false;
            PlayAudioClip(index);
        }
    }

    private void OnEnable()
    {
        if(playAwakeOnEnable)
        {
            PlayAudioClip(awakeAudio);
        }
    }

    public void PlayAudioClip(int index)
    {
        PlayAudioClip(index, null);
    }


    public void PlayAudioClip(int index, Vector3? positionOverride = null)
    {
        if(index < 0)
        {
            PlayAudioClip(awakeAudio);
        }
        else
        {
            PlayAudioClip(audioClipGroups[Mathf.Clamp(index, 0, audioClipGroups.Length - 1)], positionOverride);
        }
    }

    public void PlayAudioClip(AudioClipGroup group = null, Vector3? positionOverride = null)
    {
        Vector3 position = new Vector3();
        if(positionOverride != null)
        {
            position = (Vector3)positionOverride;
        }
        else if(group.playSoundAtPlayer && (Camera.main != null || Player.Instance != null))
        {
            if(Player.Instance != null)
            {
                position = Player.Instance.transform.position;
            }
            else
            {
                position = Camera.main.transform.position;
            }
        }
        else
        {
            position = transform.position;
        }

        if(Player.Instance == null || (Player.Instance != null && Vector3.Distance(Player.Instance.transform.position, position) < 10f))
        {
            if (group == null)
            {
                group = audioClipGroups[Mathf.Min(index, audioClipGroups.Length)];
            }

            if (group != null && group.clips.Length > 0)
            {
                bool useSettings = false;

                // check if modulation settings exist
                AudioSettings settings = group.GetSettings();
                useSettings = group.settings.Length > 0 && settings != null;

                if (audioSource != null)
                {
                    //play sound through referenced audioSource

                    audioSource.clip = group.GetAudioClip();

                    if (useSettings)
                    {
                        audioSource.pitch = settings.GetPitch();
                        audioSource.volume = settings.maxVolume;
                    }
                    else
                    {
                        audioSource.pitch = 1f;
                        audioSource.volume = .5f;
                    }

                    audioSource.Play();
                }
                else
                {
                    //Create sound at point

                    if (useSettings)
                    {
                        AudioUtilities.PlayClipAtPoint(
                            position: positionOverride != null ? (Vector3)positionOverride : transform.position,
                            clip: group.GetAudioClip(),
                            pitch: settings.GetPitch(),
                            maxDistance: 5f,
                            volume: settings.maxVolume);
                    }
                    else
                    {
                        AudioUtilities.PlayClipAtPoint(
                        position: positionOverride != null ? (Vector3)positionOverride : transform.position,
                        clip: group.GetAudioClip(),
                        pitch: 1f,
                        maxDistance: 5f,
                        volume: 1f);
                    }
                }
            }
        }

        
    }


    [System.Serializable]
    public class AudioSettings
    {
        public string label;
        public float maxVolume = 1;
        public float basePitch = 1;
        public bool modulate = true;
        public float modulationLimits = 0f;


        public AudioSettings() { }

        public AudioSettings(string label = "", float maxVolume = 1f, Vector3? positionOverride = null,
            float basePitch = 1f, bool modulate = false, float modulationLimits = 0f)
        {
            this.label = label;

            //Volume & Spatial 
            this.maxVolume = maxVolume;

            //Pitch Modulation
            this.basePitch = basePitch;
            this.modulate = modulate;
            this.modulationLimits = modulationLimits;
        }

        public float GetPitch()
        {
            if(modulate)
            {
                return basePitch + Random.Range(-modulationLimits, modulationLimits);
            }
            else
            {
                return basePitch;
            }
        }

    }
    [System.Serializable]
    public class AudioClipGroup
    {
        public string label = "";
        public AudioClip[] clips = null;
        public AudioSettings[] settings = null;
        public bool playSoundAtPlayer = false;

        public AudioClipGroup(string label = "", AudioClip[] clips = null, AudioSettings[] settings = null)
        {
            this.label = label;
            this.clips = clips;
            this.settings = settings;
        }

        public AudioSettings GetSettings()
        {
            if (settings.Length > 0)
            {
                return settings[Random.Range(0, settings.Length)];
            }
            return null;
        }

        public AudioClip GetAudioClip()
        {
            if (clips.Length > 0)
            {
                return clips[Random.Range(0, clips.Length)];
            }
            return null;

        }
    }

}
