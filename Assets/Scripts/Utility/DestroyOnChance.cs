using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnChance : MonoBehaviour
{ 
    void Start()
    {
        if(Random.Range(0, 40) > 5)
        {
            Destroy(gameObject);
        }
    }
}
