using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTriggerVolume : MonoBehaviour
{
    [SerializeField] UnityEvent onEnter = new UnityEvent();

    //private void OnTriggerEnter(Collider other)

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision");
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            //Debug.Log("Player Collision");
            onEnter.Invoke();
        }
        Destroy(gameObject);
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.GetComponent<Player>() != null)
    //    {
    //        onEnter.Invoke();
    //    }
    //    Destroy(gameObject);
    //}
}
