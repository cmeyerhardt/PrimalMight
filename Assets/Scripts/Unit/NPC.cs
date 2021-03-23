using UnityEngine;

public class NPC : Character, IInteractable
{
    float timer = 0f;

    
    public override void Start()
    {
        base.Start();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public CursorType GetCursorType(Character c)
    {
        if (c == this || c == null) { return CursorType.Default; }

        return CursorType.Combat;

        //if (c.inventory.HasWeapon())
        //{
        //    return CursorType.Combat;
        //    //if (faction == c.faction)
        //    //{
        //    //    if (c.isCannibal)
        //    //    {
        //    //        return CursorType.Combat;
        //    //    }
        //    //    else
        //    //    {
        //    //        return CursorType.Default;
        //    //    }
        //    //}
        //    //else if (faction != c.faction && !c.isCannibal)
        //    //{
        //    //    return CursorType.Combat;
        //    //}
        //}
        //return CursorType.Default;
    }

    public void Interact(Character c)
    {
        if(c == this) { return; }

        c.AttackTarget(this);

        //if (c.inventory.HasWeapon())
        //{
        //    c.AttackTarget(this);

        //    //if (faction == c.faction)
        //    //{
        //    //    if (c.isCannibal)
        //    //    {
        //    //        c.AttackTarget(this);
        //    //    }
        //    //}
        //    //else if (faction != c.faction && !c.isCannibal)
        //    //{
        //    //    c.AttackTarget(this);
        //    //}
        //}
    }

    public override void Update()
    {
        base.Update();

        if (targeting.target != null && movement != null)
        {
            if(!targeting.IsTargetInRange())
            {
                movement.MoveToDestination(targeting.target.transform);
            }

            if (targeting.IsTargetInRange() && !onCooldown)
            {
                AttackTarget();
            }

            if (!targeting.target.health.isAlive)
            {
                targeting.target = null;
            }
        }
        else if(!movement.currentlyMoving)
        {
            if(timer <= 0f)
            {
                //idly walk 
                Vector3 newDirection = Random.insideUnitSphere;//transform.position + new Vector3(3f.PlusOrMinus(), 0f, 0f);
                newDirection.y = 0f;
                movement.MoveToDestination(transform.position + newDirection * (Random.Range(5, 10) * (isCannibal? 2f : 1f)));
                timer = Random.Range(2, 5);
            }
            else
            {
                timer -= Time.deltaTime;
            }
        }
    }
}
