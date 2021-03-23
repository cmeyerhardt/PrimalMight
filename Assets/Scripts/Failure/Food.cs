using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food : Item, IInteractable
{
    [SerializeField] Faction faction = Faction.A;
    [SerializeField] float lifeTime = 10f;
    [SerializeField] public int value = 1;
    //[SerializeField] GameObject spoiledMeatModel = null;
    float lifeTimer = 0f;

    public override void Awake()
    {
        base.Awake();
        lifeTimer = lifeTime;
    }

    private void Update()
    {
        lifeTimer -= Time.deltaTime;
        
        if(lifeTime <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public Faction GetFoodFaction()
    {
        return faction;
    }

    public void SetFoodFaction(Faction f)
    {
        faction = f;
    }

    public override CursorType GetCursorType(Character c)
    {
        if (faction == c.faction)
        {
            if (c.isCannibal)
            {
                return CursorType.FoodCannibal;
            }
        }
        //else if (faction != c.faction && !c.isCannibal)
        //{

        //}
        return CursorType.Food;
    }

    public override void Interact(Character c)
    {
        if (c != null && c.IsInRange(transform.position))
        {
            c.inventory.TakeItem(this);
        }
    }

    public void DecreaseCharge()
    {
        if(value > 0)
        {
            value--;
        }

        if(value <= 0)
        {
            Destroy(gameObject);
        }

    }
}
