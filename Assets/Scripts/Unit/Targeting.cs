using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeting : MonoBehaviour
{
    [SerializeField] Character myCharacter = null;
    //[SerializeField] Transform lookTarget = null;
    [SerializeField] SphereCollider rangeCollider = null;
    [SerializeField] public Character target = null;

    private void Awake()
    {
        if (rangeCollider == null)
        {
            rangeCollider = GetComponent<SphereCollider>();
        }
    }
    
    public void SetParentCharacter(Character c)
    {
        myCharacter = c;
    }

    private void Start()
    {
        rangeCollider.radius = 10f;
    }

    private void Update()
    {
        if (!myCharacter.health.isAlive)
        {
            return;
        }

        if(target != null && Player.Instance != null)
        {
            if (Vector3.Distance(transform.position, target.transform.position) > 20f)
            {
                target = null;
                Player.Instance.Detect(myCharacter, false);
            }
        }

        //if (target != null && lookTarget != null)
        //{
        //    lookTarget.position = target.transform.position;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        if(target == null)
        {
            Character c = other.GetComponent<Character>();
            if (c != null && c != myCharacter)
            {
                if (myCharacter.isCannibal)
                {
                    if (c.faction == myCharacter.faction)
                    {
                        target = c;
                        //Debug.Log(name + " is Cannibal and found target" + target.name);
                    }
                }
                else if (c.faction != myCharacter.faction)
                {
                    target = c;
                    //Debug.Log(name + " found target" + target.name);
                }
            }
        }

        if (target != null)
        {
            DetectPlayer();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (target == null)
        {
            Character c = other.GetComponent<Character>();
            if (c != null && c != myCharacter)
            {
                if (myCharacter.isCannibal)
                {
                    if (c.faction == myCharacter.faction)
                    {
                        target = c;
                        //Debug.Log(name + " is Cannibal and found target" + target.name);
                    }
                }
                else if (c.faction != myCharacter.faction)
                {
                    target = c;
                    //Debug.Log(name + " found target" + target.name);
                }
            }
        }

        if (target != null)
        {
            DetectPlayer();
        }
    }

    private void DetectPlayer()
    {
        if(target == null) { return; }
        if(Player.Instance == null) { return; }
        if (target.gameObject == Player.Instance.gameObject)
        {
            //Debug.Log("Targeting player");
            Player.Instance.Detect(myCharacter, true);
        }
    }

    public bool IsTargetInRange()
    {
        if(target != null)
        {
            return Vector3.Distance(target.transform.position, transform.position) < myCharacter.inventory.GetRange();
        }
        else
        {
            return false;
        }

    }
}
