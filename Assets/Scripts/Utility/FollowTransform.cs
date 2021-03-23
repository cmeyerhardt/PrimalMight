using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTransform : MonoBehaviour
{
    [SerializeField] Transform target = null;

    void LateUpdate()
    {
        if (target != null)
        {
            transform.position = target.position;
        }
    }
}
