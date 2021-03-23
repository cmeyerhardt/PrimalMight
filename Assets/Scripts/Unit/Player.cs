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
        detectors.RemoveNullReferences();
        inCombat = detectors.Count > 0;
    }

    public void Detect(Character detector, bool b)
    {
        if(b && !detectors.Contains(detector))
        {
            detectors.Add(detector);
        }

        else if(!b && detectors.Contains(detector))
        {
            detectors.Remove(detector);
        }

        inCombat = detectors.Count > 0;
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
            c.movement.CancelMovement();
            c.movement.nvm.enabled = false;
            c.movement.enabled = false;

            rb.isKinematic = false;
            rb.useGravity = true;

            GetComponentInChildren<AnimationHelper>().SetCharacter(this);

            Instance = this;
            c.enabled = false;

            detectors = new List<Character>();

            // dont destroy c:  destroying c will result in missing references
            Destroy(c, Time.deltaTime);

        }

        if(health != null)
        {
            health.myCharacter = this;
        }
        else
        {
            Debug.Log("health null");
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
