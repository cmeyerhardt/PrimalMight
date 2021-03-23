using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour
{
    [SerializeField] ParticleSystem footstep = null;
    [SerializeField] AudioMod audioMod = null;
    //[SerializeField] Transform leftFoot = null;
    //[SerializeField] Transform rightFoot = null;

    //public void CreateLeftSoundEvent()
    //{
    //    CreateFootstep(leftFoot.transform.position);
    //}

    //public void CreateRightSoundEvent()
    //{
    //    CreateFootstep(rightFoot.transform.position);
    //}

    private void Awake()
    {
        //if(footstep == null)
        //{
        //    GameObject[] load = Resources.LoadAll<GameObject>("Footstep");
        //    //Debug.Log(load.Length);
        //    if(load.Length > 0)
        //    {
        //        footstep = load[0];
        //    }
        //    else
        //    {
        //        Debug.Log("Footstep could not be loaded");
        //    }
        //}
    }

    public void CreateFootstep()
    {
        //Debug.Log("Footstep");
        footstep.Play();
        audioMod.PlayAudioClip(0);
        //CreateFootstep(transform.position);
    }

    public void CreateFootstep(Vector3? positionOverride = null)
    {
        ////Debug.Log("Creating Footstep");
        //if (footstep == null) { return; }

        //Vector3 positionCheck = transform.position;
        //if (positionOverride != null)
        //{
        //    positionCheck = (Vector3)positionOverride;
        //}

        ////Debug.Log("Creating Footstep, not null");

        ////if in range of player
        //if (Vector3.Distance(positionCheck, Player.Instance.transform.position) < 30f)
        //{
        //    if (footstep != null)
        //    {
        //        GameObject f = Instantiate(footstep, positionCheck, transform.rotation, null);
        //        Destroy(f, 1f);
        //    }
        //}

    }
}
