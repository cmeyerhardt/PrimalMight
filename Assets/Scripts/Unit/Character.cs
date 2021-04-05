using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction { Default, A, B, Mixed }

public class Character : MonoBehaviour
{
    [Header("Configure")]
    [SerializeField] internal Faction faction = Faction.Default;
    public bool isCannibal = false;

    [Header("Reference")]
    [SerializeField] internal Movement movement = null;
    [SerializeField] internal Health health = null;
    [SerializeField] internal Hunger hunger = null;
    [SerializeField] internal Inventory inventory = null;
    [SerializeField] internal Targeting targeting = null;
    [SerializeField] internal Rigidbody rb = null;
    [SerializeField] internal Animator animator = null;
    [SerializeField] internal AudioMod audioMod = null;

    [SerializeField] GameObject hitFX = null;

    public bool onCooldown = true;
    public float attackCooldown = 1f;
    public float attackTimer = 0f;

    float aggressiveTimer = 0f;

    public virtual void Awake()
    {
        if(animator == null)
            animator = GetComponentInChildren<Animator>();

        movement = GetComponent<Movement>();
        if(animator != null)
            movement.animator = animator;

        health = GetComponent<Health>();

        hunger = GetComponent<Hunger>();
        hunger.SetCharacter(this);

        AnimationHelper a = GetComponentInChildren<AnimationHelper>();
        if(a != null)
        {
            a.SetCharacter(this);
        }

        audioMod = GetComponent<AudioMod>();

        inventory = GetComponent<Inventory>();
        rb = GetComponent<Rigidbody>();

        targeting = GetComponent<Targeting>();
        targeting.SetParentCharacter(this);
    }

    public virtual void Start()
    {
        hunger.aggressiveEvent.AddListener(BecomeCannibal);
        if (audioMod != null)
        {
            audioMod.PlayAudioClip(0);
        }
    }

    public void BecomeCannibal(bool b)
    {
        isCannibal = b;
    }

    public virtual void Update()
    {
        if(!health.isAlive)
        {
            movement.enabled = false;
            return;
        }

        if (isCannibal)
        {
            aggressiveTimer += Time.deltaTime;
            if (aggressiveTimer > 10f)
            {
                AttackTarget(FindObjectOfType<NPC>());
                aggressiveTimer = 0f;
            }
        }

        if (onCooldown)
        {
            if (attackTimer > 0f)
            {
                attackTimer -= Time.deltaTime;
            }
            else
            {
                onCooldown = false;
            }
        }
    }

    public virtual void AttackTarget()
    {
        InteractAnimation(true);
        attackTimer = attackCooldown;
        onCooldown = true;
        //CheckTargetRange();
    }

    public void InteractAnimation(bool right = true)
    {
        animator.SetBool("right", right);
        animator.SetTrigger("interact");
    }

    public virtual void AttackTarget(Character target)
    {
        if(target != null)
        {
            targeting.target = target;
            AttackTarget();
        }
    }

    public virtual void Die()
    {
        //Debug.Log("Character.Die");
        movement.enabled = false;
        if(Player.Instance != null)
        {
            Debug.Log("Removing detection for this reference: " + this);
            _ = Player.Instance.Detect(this, false);
        }

        hunger.DisableBar();
        health.Die(CauseOfDeath.Default);

        if (audioMod != null)
        {
            audioMod.PlayAudioClip(2);
        }
    }

    public void CheckTargetRange()
    {
        if (targeting.IsTargetInRange() && targeting.target.health.isAlive)
        {
            if(hitFX != null)
            {
                Instantiate(hitFX, targeting.target.transform.position + Vector3.up, transform.rotation, null);
            }

            if(audioMod != null)
            {
                audioMod.PlayAudioClip(1);
            }

            if(inventory.HasWeapon())
            {
                targeting.target.health.LoseHealth(inventory.CheckWeapon().GetDamage());
            }
            else
            {
                targeting.target.health.LoseHealth(1);
            }
        }
    }

    public void CreateCorpse()
    {
        if (health.CreateCorpse())
        {
            Destroy(gameObject);
            //transform.parent = unitDeathTransform;
        }

    }

    public bool IsInRange(Vector3 referencePosition)
    {
        return Vector3.Distance(transform.position, referencePosition) < 4f;
    }
}
