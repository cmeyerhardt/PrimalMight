using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [SerializeField] public Vector3 rotationVector = new Vector3();

    void Update()
    {
        transform.Rotate(rotationVector * Time.deltaTime);
    }
}
