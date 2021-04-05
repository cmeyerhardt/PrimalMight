using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationHelper : MonoBehaviour
{
    [Header("Helper")]
    [SerializeField] Character myCharacter = null;
    [SerializeField] ParticleSystem footstep = null;
    [SerializeField] AudioMod audioMod = null;
    static PlayerController playerController = null;

    private void Awake()
    {
        if(playerController == null)
        {
            playerController = FindObjectOfType<PlayerController>();
        }
    }

    public void SetCharacter(Character c)
    {
        myCharacter = c;
    }

    public void DisableControl()
    {
        if (myCharacter == Player.Instance)
        {
            playerController.inControl = false;
        }
    }

    public void AttackHelper()
    {
        myCharacter.CheckTargetRange();
    }

    public void NewUnit()
    {
        if(myCharacter == Player.Instance)
        {
            playerController.CheckForNewUnitAndVictoryCondition();
        }
    }

    public void Footstep()
    {
        footstep.Play();
        audioMod.PlayAudioClip(0);
    }

    public void CreateCorpse()
    {
        if(myCharacter != null)
        {
            myCharacter.CreateCorpse();
        }
    }
}
