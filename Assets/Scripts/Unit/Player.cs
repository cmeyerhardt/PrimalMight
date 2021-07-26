using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Player : Character
{
    public static Player Instance;
    public bool inCombat = false;
    public List<Character> detectors = new List<Character>();
    static public PlayerController playerController = null;

    public override void Start()
    {
        base.Awake();
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        //detectors.RemoveNullReferences();
        inCombat = detectors.Count > 0;
    }

    public bool Detect(Character detector, bool b)
    {
        if(detector != null && detector.gameObject != gameObject)
        {
            if (!b && detectors.Contains(detector))
            {
                detectors.Remove(detector);
            }
            else if (b && !detectors.Contains(detector))
            {
                detectors.Add(detector);
            }

            //todo -- fix
            detectors.RemoveNullReferences();

            inCombat = detectors.Count > 0;
        }
        return true;
    }

    public override void Die()
    {
        base.Die();
        //Debug.Log("Player.Die");
        playerController.PlayerDied();

    }

    private void OnEnable()
    {
        NPC c = GetComponent<NPC>();
        if(c != null)
        {
            try { c.movement.CancelMovement(); }
            catch(Exception e)
            {
                Debug.LogWarning(e.ToString());
            }

            c.movement.nvm.enabled = false;
            c.movement.enabled = false;

            rb.isKinematic = false;
            rb.useGravity = true;

            AnimationHelper ah = GetComponentInChildren<AnimationHelper>();
            if(ah!= null)
            {
                ah.SetCharacter(this);
            }


            Instance = this;
            c.enabled = false;

            detectors = new List<Character>();

            Destroy(c, Time.deltaTime);
        }

        if(health == null)
        {
            health = GetComponent<Health>();
        }

        if(health != null)
        {
            health.myCharacter = this;
        }
        else
        {
            Debug.LogWarning(name + " has no Health.cs");
        }

    }

    //public bool isGrounded = true;
    //private void OnCollisionStay(Collision collision)
    //{
    //    if (collision.gameObject.tag == "Terrain")
    //    {
    //        isGrounded = true;
    //    }
    //}

    //private void OnCollisionExit(Collision collision)
    //{
    //    //Debug.Log(collision.gameObject);
    //    //if (collision.collider.GetType() == typeof(TerrainCollider))
    //    if (collision.gameObject.tag == "Terrain")
    //    {
    //        //Debug.Log("Terrain");
    //        isGrounded = false;
    //    }
    //}
}
