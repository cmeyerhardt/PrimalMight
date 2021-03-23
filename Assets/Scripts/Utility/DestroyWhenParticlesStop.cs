using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyWhenParticlesStop : MonoBehaviour
{
    [SerializeField] ParticleSystem ps = null;

    void Update()
    {
        if(ps == null || ps.isStopped)
        {
            Destroy(gameObject);
        }
    }
}
