using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAt : MonoBehaviour
{
    [SerializeField] Transform target = null;

    void Update()
    {
        if(target != null)
        {
            transform.LookAt(target);
        }
    }
}
