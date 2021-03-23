using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Item : MonoBehaviour, IInteractable
{
    [SerializeField] internal GameObject itemGlow = null;
    [SerializeField] Rigidbody rb = null;
    [SerializeField] AudioMod audioMod = null;
    bool grounded = false;

    [SerializeField] Collider col = null;

    public virtual void Awake()
    {
        if(audioMod == null) { audioMod = GetComponent<AudioMod>(); }

        InitializeRigidbody();
        col = GetComponent<Collider>();

        if (rb != null)
        {
            rb.useGravity = true;
        }

        if (transform.parent == null)
        {
            BeDropped();
        }

    }

    private void InitializeRigidbody()
    {
        rb = GetComponent<Rigidbody>();

        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }

        if(rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }

    public virtual void BeDropped()
    {
        transform.parent = null;
        
        grounded = false;
        rb.useGravity = true;
        rb.isKinematic = false;

        if (col != null)
        {
            col.enabled = true;
        }

        //if (Physics.Raycast(new Ray(transform.position + Vector3.up, Vector3.down), out RaycastHit info, 10f, 8))
        //{
        //    Debug.Log("info.point: " + info.point);
        //    transform.position = info.point;
        //}

        if (itemGlow != null)
            itemGlow.SetActive(true);
    }

    public virtual void BePickedUp()
    {
        if (rb == null)
        {
            InitializeRigidbody();
        }

        if(rb != null)
        {
            rb.isKinematic = true;
        }

        if(col != null)
        {
            col.enabled = false;
        }

        if(audioMod != null)
        {
            audioMod.PlayAudioClip(1);
        }

        if (itemGlow != null)
        {
            itemGlow.SetActive(false);
        }
    }

    public virtual CursorType GetCursorType(Character c)
    {
        return CursorType.PickUp;
    }

    public virtual void Interact(Character c)
    {
        if (c != null && c.IsInRange(transform.position))
        {
            c.inventory.TakeItem(this);
        }
    }


    private void Land()
    {
        grounded = true;
        if(rb == null)
        {
            InitializeRigidbody();
        }
        if (audioMod != null)
        {
            audioMod.PlayAudioClip(0);
        }
        rb.isKinematic = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!grounded && collision.gameObject.GetComponent<Terrain>() != null || collision.gameObject.isStatic)
        {
            Land();
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!grounded && collision.gameObject.GetComponent<Terrain>() != null || collision.gameObject.isStatic)
        {
            Land();
        }

    }
}
